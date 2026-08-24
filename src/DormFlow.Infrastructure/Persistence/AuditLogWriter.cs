using DormFlow.Application.Common.Abstractions;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Persistence;

/// <summary>
/// บันทึกเหตุการณ์ลงตาราง audit_logs
/// </summary>
/// <remarks>
/// เติมข้อมูลบริบทของคำขอให้อัตโนมัติ ผู้เรียกจึงระบุแค่ว่าเกิดอะไรขึ้นกับอะไร
/// การบันทึกใช้ ExecuteUpdate ไม่ได้เพราะเป็นการเพิ่มแถวใหม่
/// แต่ใช้ DbContext ตัวเดียวกับ Use Case จึงอยู่ในธุรกรรมเดียวกันเมื่อผู้เรียกครอบธุรกรรมไว้
/// </remarks>
public sealed class AuditLogWriter(
    DormFlowDbContext dbContext,
    ICurrentUser currentUser,
    IDateTimeProvider dateTimeProvider) : IAuditLogWriter
{
    /// <summary>ความยาวสูงสุดของ User Agent ที่เก็บได้</summary>
    private const int MaxUserAgentLength = 512;

    /// <summary>ความยาวสูงสุดของเส้นทางคำขอที่เก็บได้</summary>
    private const int MaxRequestPathLength = 512;

    private readonly DormFlowDbContext _dbContext = dbContext;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;

    /// <inheritdoc />
    public async Task WriteAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(auditLog);

        _dbContext.AuditLogs.Add(auditLog);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task WriteAsync(
        AuditAction action,
        string entityName,
        string? entityId = null,
        Guid? userId = null,
        Guid? organizationId = null,
        string? oldValues = null,
        string? newValues = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entityName);

        AuditLog auditLog = new()
        {
            Action = action,
            EntityName = entityName,
            EntityId = entityId,

            // ตอนเข้าสู่ระบบยังไม่มี Claim ผู้เรียกจึงส่งรหัสผู้ใช้เข้ามาเอง
            UserId = userId ?? _currentUser.UserId,
            OrganizationId = organizationId ?? _currentUser.OrganizationId,
            OldValues = oldValues,
            NewValues = newValues,
            IpAddress = _currentUser.IpAddress,
            UserAgent = Truncate(_currentUser.UserAgent, MaxUserAgentLength),
            RequestPath = Truncate(_currentUser.RequestPath, MaxRequestPathLength),
            CreatedAt = _dateTimeProvider.UtcNow,
        };

        return WriteAsync(auditLog, cancellationToken);
    }

    private static string? Truncate(string? value, int maxLength)
        => value is null || value.Length <= maxLength ? value : value[..maxLength];
}
