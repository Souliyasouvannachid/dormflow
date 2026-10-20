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
/// ກັບດັກ Exception ທຸກສິ່ງທຸກຢ່າງທີ່ອອກມາຈາກທໍ່ນັ້ນຖືກປ່ຽນເປັນ API Response ຮູບແບບດຽວກັນໃນທົ່ວລະບົບ
/// </summary>
/// <remarks>
/// ວາງໄວ້ເປັນ Middleware ອັນທໍາອິດແມ່ນເພື່ອກວມເອົາຄວາມຜິດພາດຈາກ Middleware ຄົນອື່ນຄືກັນ
/// ຂໍ້ຄວາມທີ່ຕອບກັບຫາຜູ້ໂທຈະບໍ່ມີລາຍລະອຽດພາຍໃນໃດໆ. ລາຍລະອຽດຕົວຈິງໄດ້ຖືກບັນທຶກໄວ້ Log ພ້ອມ TraceId
/// </remarks>
public sealed class GlobalExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionHandlingMiddleware> logger,
    IHostEnvironment environment)
{
    /// <summary>ສະຖານະທີ່ໃຊ້ເມື່ອຜູ້ໂທຍົກເລີກການຮ້ອງຂໍດ້ວຍຕົນເອງ. ຕາມປະເພນີຂອງ nginx</summary>
    private const int ClientClosedRequestStatusCode = 499;

    private static readonly JsonSerializerOptions SerializerOptions = DormFlowJsonSerializerOptions.Default;

    private readonly RequestDelegate _next = next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger = logger;
    private readonly IHostEnvironment _environment = environment;

    /// <summary>ຂະບວນການຮ້ອງຂໍແລະຈັບຄວາມຜິດພາດຍ້ອນວ່າເຂົາເຈົ້າເກີດຂຶ້ນ</summary>
    /// <param name="context">ສະພາບການຂອງຄໍາຮ້ອງຂໍໃນປະຈຸບັນ</param>
    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // ຜູ້ໂທໄດ້ປິດການເຊື່ອມຕໍ່. ມັນບໍ່ແມ່ນຄວາມຜິດພາດຂອງລະບົບ. ສະນັ້ນບັນທຶກເປັນລະດັບ Information
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
            // ຂຽນ Response ບາງສ່ວນຂອງມັນໄດ້ຝົດແລ້ວ. ບໍ່ມີຫຍັງສາມາດແກ້ໄຂໄດ້ຍົກເວັ້ນການປ່ອຍໃຫ້ການເຊື່ອມຕໍ່ສິ້ນສຸດລົງ
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
                // ຂໍ້ຄວາມມາຈາກ Use Case ໂດຍກົງ, ເຊັ່ນ "ອີເມວ ຫຼືລະຫັດຜ່ານບໍ່ຖືກຕ້ອງ" ຫຼື "Refresh Token ໝົດອາຍຸແລ້ວ"
                // ໃນກໍລະນີທີ່ມັນບໍ່ຕິດ Token ທັງຝົດ? Token ບໍ່ຖືກຕ້ອງ, ຕິດຢູ່ JwtBearerEvents.OnChallenge ກ່ອນຈຸດນີ້
                return (StatusCodes.Status401Unauthorized, ApiResponse.Fail(unauthorized.Message, traceId));

            case ForbiddenAccessException forbidden:
                return (StatusCodes.Status403Forbidden, ApiResponse.Fail(forbidden.Message, traceId));

            case NotFoundException notFound:
                return (StatusCodes.Status404NotFound, ApiResponse.Fail(notFound.Message, traceId));

            case ConflictException conflict:
                return (StatusCodes.Status409Conflict, ApiResponse.Fail(conflict.Message, traceId));

            case ServiceUnavailableException unavailable:
                return (StatusCodes.Status503ServiceUnavailable, ApiResponse.Fail(unavailable.Message, traceId));

            case DbUpdateConcurrencyException:
                return (StatusCodes.Status409Conflict, ApiResponse.Fail(ApiMessages.ConcurrencyConflict, traceId));

            case DomainException domain:
                return (StatusCodes.Status409Conflict, ApiResponse.Fail(domain.Message, traceId));

            case BadHttpRequestException badRequest:
                return (StatusCodes.Status400BadRequest, ApiResponse.Fail(badRequest.Message, traceId));

            default:
                // ຢ່າເປີດເຜີຍລາຍລະອຽດພາຍໃນໃຫ້ຜູ້ໂທເຂົ້າມາ Production
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
/// ຂໍ້ຄວາມ Log ຂອງຕົວຈັບຄວາມຜິດພາດ
/// </summary>
/// <remarks>
/// ໃຊ້ LoggerMessage source generator ໃນຄໍາສັ່ງທີ່ຈະບໍ່ມີ box ຄ່າແລະບໍ່ປະເມີນພາລາມິເຕີ
/// ເມື່ອລະດັບ Log ປິດ
/// </remarks>
internal static partial class ExceptionLog
{
    [LoggerMessage(
        EventId = 1000,
        Level = LogLevel.Information,
        Message = "ຄຳຮ້ອງ {Method} {Path} ຖືກຍົກເລີກໂດຍຜູ້ເອີ້ນ")]
    public static partial void RequestCancelledByCaller(ILogger logger, string method, PathString path);

    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Warning,
        Message = "ບໍ່ສາມາດຂຽນ Error Response ໄດ້ເພາະເລີ່ມສົ່ງ Response ໄປແລ້ວ TraceId {TraceId}")]
    public static partial void ResponseAlreadyStarted(ILogger logger, string traceId);

    [LoggerMessage(
        EventId = 1002,
        Level = LogLevel.Error,
        Message = "ເກີດຂໍ້ຜິດພາດທີ່ບໍ່ຄາດຄິດຂະນະປະມວນຜົນ {Method} {Path} ສະຖານະ {StatusCode} TraceId {TraceId}")]
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
        Message = "ຄຳຮ້ອງ {Method} {Path} ສິ້ນສຸດດ້ວຍສະຖານະ {StatusCode} ເພາະ {ExceptionType} TraceId {TraceId}")]
    public static partial void HandledException(
        ILogger logger,
        string method,
        PathString path,
        int statusCode,
        string exceptionType,
        string traceId);
}
