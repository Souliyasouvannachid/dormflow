using DormFlow.Application.Common.Abstractions;
using DormFlow.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DormFlow.Infrastructure.Persistence.Interceptors;

/// <summary>
/// เติมข้อมูล Audit และแปลงการลบให้เป็น Soft Delete ก่อนบันทึกลงฐานข้อมูล
/// </summary>
/// <remarks>
/// ทำงานกับ Entity ทุกตัวที่ประกาศ <see cref="IAuditableEntity"/> หรือ <see cref="ISoftDeletable"/>
/// ไม่ผูกกับคลาสฐานใดคลาสหนึ่ง จึงครอบคลุม Entity ของ ASP.NET Core Identity ด้วย
/// การทำที่ระดับ Interceptor ทำให้ไม่มี Use Case ใดลืมเติมค่าเหล่านี้
/// </remarks>
public sealed class AuditableEntitySaveChangesInterceptor(
    ICurrentUser currentUser,
    IDateTimeProvider dateTimeProvider) : SaveChangesInterceptor
{
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;

    /// <inheritdoc />
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ArgumentNullException.ThrowIfNull(eventData);

        ApplyAuditInformation(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    /// <inheritdoc />
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventData);

        ApplyAuditInformation(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplyAuditInformation(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        DateTimeOffset now = _dateTimeProvider.UtcNow;
        Guid? userId = _currentUser.UserId;

        foreach (EntityEntry entry in context.ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Deleted && entry.Entity is ISoftDeletable softDeletable)
            {
                ConvertToSoftDelete(entry, softDeletable, now, userId);
                continue;
            }

            if (entry.Entity is not IAuditableEntity auditable)
            {
                continue;
            }

            switch (entry.State)
            {
                case EntityState.Added:
                    auditable.CreatedAt = now;
                    auditable.CreatedBy = userId;
                    break;

                case EntityState.Modified:
                    auditable.UpdatedAt = now;
                    auditable.UpdatedBy = userId;
                    break;

                case EntityState.Detached:
                case EntityState.Unchanged:
                case EntityState.Deleted:
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// เปลี่ยนคำสั่งลบให้เป็นการอัปเดตธง IsDeleted แทน
    /// จึงไม่มีจุดใดในระบบที่ลบแถวออกจากฐานข้อมูลจริงโดยไม่ตั้งใจ
    /// </summary>
    private static void ConvertToSoftDelete(
        EntityEntry entry,
        ISoftDeletable softDeletable,
        DateTimeOffset now,
        Guid? userId)
    {
        entry.State = EntityState.Modified;

        softDeletable.IsDeleted = true;
        softDeletable.DeletedAt = now;
        softDeletable.DeletedBy = userId;

        if (entry.Entity is IAuditableEntity auditable)
        {
            auditable.UpdatedAt = now;
            auditable.UpdatedBy = userId;
        }

        // ป้องกันไม่ให้คอลัมน์อื่นถูกเขียนทับจากค่าที่ค้างอยู่ใน entity ที่ถูกสั่งลบ
        foreach (PropertyEntry property in entry.Properties)
        {
            property.IsModified = property.Metadata.Name switch
            {
                nameof(ISoftDeletable.IsDeleted) => true,
                nameof(ISoftDeletable.DeletedAt) => true,
                nameof(ISoftDeletable.DeletedBy) => true,
                nameof(IAuditableEntity.UpdatedAt) => true,
                nameof(IAuditableEntity.UpdatedBy) => true,
                _ => false,
            };
        }
    }
}
