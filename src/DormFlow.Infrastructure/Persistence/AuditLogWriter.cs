using DormFlow.Application.Common.Abstractions;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Persistence;

/// <summary>
/// ບັນທຶກເຫດການໃນຕາຕະລາງ audit_logs
/// </summary>
/// <remarks>
/// ຕື່ມຂໍ້ມູນໃສ່ອັດຕະໂນມັດ context ຜູ້ໂທຫຼັງຈາກນັ້ນກໍານົດພຽງແຕ່ສິ່ງທີ່ເກີດຂຶ້ນແລະສິ່ງທີ່
/// ການບັນທຶກການນໍາໃຊ້ ExecuteUpdate ມັນເປັນໄປບໍ່ໄດ້ເພາະມັນເພີ່ມແຖວໃໝ່
/// ແຕ່ໃຊ້ DbContext ອັນດຽວກັນກັບ Use Case ດັ່ງນັ້ນ, ມັນຢູ່ໃນການເຮັດທຸລະກໍາດຽວກັນເມື່ອຜູ້ໂທກວມເອົາການເຮັດທຸລະກໍາ
/// </remarks>
public sealed class AuditLogWriter(
    DormFlowDbContext dbContext,
    ICurrentUser currentUser,
    IDateTimeProvider dateTimeProvider) : IAuditLogWriter
{
    /// <summary>ຄວາມຍາວສູງສຸດຂອງ User Agent ທີ່ສາມາດເກັບຮັກສາໄດ້</summary>
    private const int MaxUserAgentLength = 512;

    /// <summary>ຄວາມຍາວສູງສຸດຂອງເສັ້ນທາງການຮ້ອງຂໍທີ່ສາມາດເກັບຮັກສາໄດ້</summary>
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

            // ໃນເວລາທີ່ເຂົ້າສູ່ລະບົບບໍ່ມີ Claim ຜູ້ໂທຫຼັງຈາກນັ້ນສົ່ງລະຫັດຜູ້ໃຊ້ດ້ວຍຕົນເອງ
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
