using System.Diagnostics;
using System.Net.Mime;
using System.Text.Json;
using DormFlow.Application.Common.Exceptions;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using FluentValidationException = FluentValidation.ValidationException;

namespace DormFlow.Api.Middleware;

/// <summary>
/// ดักจับ Exception ทุกตัวที่หลุดออกมาจากไปป์ไลน์ แล้วแปลงเป็น API Response รูปแบบเดียวกันทั้งระบบ
/// </summary>
/// <remarks>
/// วางไว้เป็น Middleware ตัวแรกสุด เพื่อให้ครอบคลุมข้อผิดพลาดจาก Middleware ตัวอื่นด้วย
/// ข้อความที่ตอบกลับผู้เรียกจะไม่มีรายละเอียดภายในระบบ ส่วนรายละเอียดจริงถูกบันทึกลง Log พร้อม TraceId
/// </remarks>
public sealed class GlobalExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionHandlingMiddleware> logger,
    IHostEnvironment environment)
{
    /// <summary>สถานะที่ใช้เมื่อผู้เรียกยกเลิกคำขอเอง ตามธรรมเนียมของ nginx</summary>
    private const int ClientClosedRequestStatusCode = 499;

    private static readonly JsonSerializerOptions SerializerOptions = DormFlowJsonSerializerOptions.Default;

    private readonly RequestDelegate _next = next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger = logger;
    private readonly IHostEnvironment _environment = environment;

    /// <summary>ประมวลผลคำขอและดักจับข้อผิดพลาดที่เกิดขึ้น</summary>
    /// <param name="context">บริบทของคำขอปัจจุบัน</param>
    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // ผู้เรียกปิดการเชื่อมต่อไปแล้ว ไม่ใช่ข้อผิดพลาดของระบบ จึงบันทึกเป็นระดับ Information
            ExceptionLog.RequestCancelledByCaller(_logger, context.Request.Method, context.Request.Path);

            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = ClientClosedRequestStatusCode;
            }
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        string traceId = Activity.Current?.Id ?? context.TraceIdentifier;
        (int statusCode, ApiResponse response) = MapException(exception, traceId);

        LogException(context, exception, statusCode, traceId);

        if (context.Response.HasStarted)
        {
            // เขียน Response ไปแล้วบางส่วน แก้อะไรไม่ได้นอกจากปล่อยให้การเชื่อมต่อจบลง
            ExceptionLog.ResponseAlreadyStarted(_logger, traceId);
            return;
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = MediaTypeNames.Application.Json;

        await context.Response.WriteAsJsonAsync(response, SerializerOptions, context.RequestAborted);
    }

    private (int StatusCode, ApiResponse Response) MapException(Exception exception, string traceId)
    {
        switch (exception)
        {
            case ValidationFailedException validationFailed:
                return (
                    StatusCodes.Status400BadRequest,
                    ApiResponse.ValidationFailed(ApiMessages.ValidationFailed, validationFailed.Errors, traceId));

            case FluentValidationException fluentValidation:
                return (
                    StatusCodes.Status400BadRequest,
                    ApiResponse.ValidationFailed(
                        ApiMessages.ValidationFailed,
                        new ValidationFailedException(fluentValidation.Errors).Errors,
                        traceId));

            case UnauthorizedAccessException unauthorized:
                // ข้อความมาจาก Use Case โดยตรง เช่น "อีเมลหรือรหัสผ่านไม่ถูกต้อง" หรือ "Refresh Token หมดอายุแล้ว"
                // ส่วนกรณีไม่ได้แนบ Token เลยหรือ Token ไม่ถูกต้อง ถูกดักไว้ที่ JwtBearerEvents.OnChallenge ก่อนถึงจุดนี้
                return (StatusCodes.Status401Unauthorized, ApiResponse.Fail(unauthorized.Message, traceId));

            case ForbiddenAccessException forbidden:
                return (StatusCodes.Status403Forbidden, ApiResponse.Fail(forbidden.Message, traceId));

            case NotFoundException notFound:
                return (StatusCodes.Status404NotFound, ApiResponse.Fail(notFound.Message, traceId));

            case ConflictException conflict:
                return (StatusCodes.Status409Conflict, ApiResponse.Fail(conflict.Message, traceId));

            case DbUpdateConcurrencyException:
                return (StatusCodes.Status409Conflict, ApiResponse.Fail(ApiMessages.ConcurrencyConflict, traceId));

            case DomainException domain:
                return (StatusCodes.Status409Conflict, ApiResponse.Fail(domain.Message, traceId));

            case BadHttpRequestException badRequest:
                return (StatusCodes.Status400BadRequest, ApiResponse.Fail(badRequest.Message, traceId));

            default:
                // ไม่เปิดเผยรายละเอียดภายในให้ผู้เรียกใน Production
                return (
                    StatusCodes.Status500InternalServerError,
                    ApiResponse.Fail(
                        _environment.IsDevelopment() ? exception.Message : ApiMessages.InternalServerError,
                        traceId));
        }
    }

    private void LogException(HttpContext context, Exception exception, int statusCode, string traceId)
    {
        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            ExceptionLog.UnhandledException(
                _logger,
                exception,
                context.Request.Method,
                context.Request.Path,
                statusCode,
                traceId);

            return;
        }

        ExceptionLog.HandledException(
            _logger,
            context.Request.Method,
            context.Request.Path,
            statusCode,
            exception.GetType().Name,
            traceId);
    }
}

/// <summary>
/// ข้อความ Log ของตัวดักจับข้อผิดพลาด
/// </summary>
/// <remarks>
/// ใช้ LoggerMessage source generator เพื่อไม่ให้มีการ box ค่าและไม่ประเมินพารามิเตอร์
/// เมื่อระดับ Log นั้นถูกปิดอยู่
/// </remarks>
internal static partial class ExceptionLog
{
    [LoggerMessage(
        EventId = 1000,
        Level = LogLevel.Information,
        Message = "คำขอ {Method} {Path} ถูกยกเลิกโดยผู้เรียก")]
    public static partial void RequestCancelledByCaller(ILogger logger, string method, PathString path);

    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Warning,
        Message = "ไม่สามารถเขียน Error Response ได้เพราะเริ่มส่ง Response ไปแล้ว TraceId {TraceId}")]
    public static partial void ResponseAlreadyStarted(ILogger logger, string traceId);

    [LoggerMessage(
        EventId = 1002,
        Level = LogLevel.Error,
        Message = "เกิดข้อผิดพลาดที่ไม่คาดคิดขณะประมวลผล {Method} {Path} สถานะ {StatusCode} TraceId {TraceId}")]
    public static partial void UnhandledException(
        ILogger logger,
        Exception exception,
        string method,
        PathString path,
        int statusCode,
        string traceId);

    [LoggerMessage(
        EventId = 1003,
        Level = LogLevel.Warning,
        Message = "คำขอ {Method} {Path} จบด้วยสถานะ {StatusCode} เพราะ {ExceptionType} TraceId {TraceId}")]
    public static partial void HandledException(
        ILogger logger,
        string method,
        PathString path,
        int statusCode,
        string exceptionType,
        string traceId);
}
