using DormFlow.Application.Common.Abstractions;
using DormFlow.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DormFlow.Infrastructure.Persistence.Interceptors;

/// <summary>
/// ຕື່ມຂໍ້ມູນໃສ່ Audit ແລະປ່ຽນການລົບເປັນ Soft Delete ກ່ອນທີ່ຈະບັນທຶກໃສ່ຖານຂໍ້ມູນ
/// </summary>
/// <remarks>
/// ເຮັດວຽກກັບ Entity ທຸກໆລາຍການປະກາດ <see cref="IAuditableEntity"/> ຫຼື <see cref="ISoftDeletable"/>
/// ມັນບໍ່ໄດ້ຖືກຜູກມັດກັບຊັ້ນພື້ນຖານອັນໃດອັນໜຶ່ງ, ສະນັ້ນມັນຈຶ່ງມີຄວາມສົມບູນແບບ Entity ຂອງ ASP.NET Core Identity ຄືກັນ
/// ເຮັດມັນຢູ່ໃນລະດັບ Interceptor ເຮັດໃຫ້ບໍ່ມີ Use Case ໃຜລືມເພີ່ມຄຸນຄ່າເຫຼົ່ານີ້?
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
    /// ປ່ຽນຄໍາສັ່ງລຶບເພື່ອອັບເດດທຸງ IsDeleted ແທນ
    /// ດັ່ງນັ້ນບໍ່ມີຈຸດໃດໃນລະບົບທີ່ແຖວຖືກລຶບໂດຍບັງເອີນຈາກຖານຂໍ້ມູນຕົວຈິງ
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

        // ປ້ອງກັນບໍ່ໃຫ້ຖັນອື່ນຖືກຂຽນທັບໂດຍຄ່າທີ່ຍັງເຫຼືອຢູ່ໃນ entity ເຊິ່ງຖືກສັ່ງໃຫ້ລຶບ
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
