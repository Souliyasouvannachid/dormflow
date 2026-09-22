using System.Linq.Expressions;
using DormFlow.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DormFlow.Infrastructure.Persistence.Extensions;

/// <summary>
/// ສົນທິສັນຍາສ້າງແບບຈໍາລອງທີ່ໃຊ້ໄດ້ກັບທຸກຄົນ Entity ຂອງ DormFlow
/// </summary>
/// <remarks>
/// ລວມເຂົ້າໄປໃນບ່ອນດຽວແທນທີ່ຈະຂຽນຄືນທຸກສິ່ງທຸກຢ່າງ EntityTypeConfiguration
/// ເຮັດ Entity ທີ່ສາມາດເພີ່ມໄດ້ໃນພາຍຫຼັງ Soft Delete Filter, Audit Column
/// ແລະ Optimistic Concurrency ໄປອັດຕະໂນມັດ
/// </remarks>
public static class ModelBuilderExtensions
{
    /// <summary>ຊື່ຖັນລະບົບຂອງ PostgreSQL ໃຊ້ເພື່ອເຮັດໃຫ້ Optimistic Concurrency</summary>
    private const string XminColumnName = "xmin";

    /// <summary>ການແກ້ໄຂມາດຕະຖານຂອງມູນຄ່າເງິນຕາ</summary>
    private const int MoneyPrecision = 18;

    /// <summary>ຈຳນວນທົດສະນິຍົມມາດຕະຖານຂອງຄ່າເງິນ</summary>
    private const int MoneyScale = 2;

    /// <summary>
    /// ປ້ອນສົນທິສັນຍາການສ້າງແຜນທີ່ທັງໝົດຂອງ DormFlow ກັບຕົວແບບ
    /// ຕ້ອງໂທຫາຫຼັງ ApplyConfigurationsFromAssembly ສະເໝີ
    /// </summary>
    /// <param name="modelBuilder">ຕົວສ້າງຕົວແບບ EF Core</param>
    public static void ApplyDormFlowConventions(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
        {
            ApplyDecimalPrecision(entityType);

            // ກວດເບິ່ງຈາກການມີ property ຊື່ Version ແທນທີ່ຈະກວດເບິ່ງຊັ້ນພື້ນຖານ
            // ດັ່ງນັ້ນ Entity ຂອງ Identity ໃຊ້ ConcurrencyStamp ຖືກຂ້າມແລ້ວ
            ApplyConcurrencyToken(entityType);

            if (entityType.ClrType.IsAssignableTo(typeof(ISoftDeletable)))
            {
                ApplySoftDeleteQueryFilter(modelBuilder, entityType.ClrType);
                ApplySoftDeleteIndex(entityType);
            }

            if (entityType.ClrType.IsAssignableTo(typeof(ITenantScoped)))
            {
                ApplyTenantIndexes(entityType);
            }
        }
    }

    /// <summary>
    /// ແຜນທີ່ property Version ເຂົ້າໄປໃນຖັນລະບົບ xmin ຂອງ PostgreSQL
    /// PostgreSQL ມັນອັບເດດຄ່ານີ້ດ້ວຍຕົນເອງທຸກຄັ້ງທີ່ມີການດັດແກ້ແຖວ. ສະນັ້ນທ່ານບໍ່ ຈຳ ເປັນຕ້ອງຈັດການສະບັບດ້ວຍຕົນເອງ
    /// </summary>
    private static void ApplyConcurrencyToken(IMutableEntityType entityType)
    {
        IMutableProperty? version = entityType.FindProperty(nameof(BaseEntity.Version));
        if (version is null || version.ClrType != typeof(uint))
        {
            return;
        }

        version.SetColumnName(XminColumnName);
        version.SetColumnType("xid");
        version.ValueGenerated = ValueGenerated.OnAddOrUpdate;
        version.IsConcurrencyToken = true;
    }

    /// <summary>ກັ່ນຕອງແຖວທີ່ຖືກຕ້ອງ Soft Delete ອອກຈາກທຸກໆ Query ອັດຕະໂນມັດ</summary>
    private static void ApplySoftDeleteQueryFilter(ModelBuilder modelBuilder, Type clrType)
    {
        // ສ້າງການສະແດງອອກ entity => !entity.IsDeleted ຕົວແບບ dynamic ເພາະວ່າຂ້ອຍຮູ້ຈັກປະເພດຂອງຕອນ runtime ເທົ່ານັ້ນ
        ParameterExpression parameter = Expression.Parameter(clrType, "entity");
        MemberExpression isDeleted = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
        UnaryExpression notDeleted = Expression.Not(isDeleted);

        modelBuilder.Entity(clrType).HasQueryFilter(Expression.Lambda(notDeleted, parameter));
    }

    /// <summary>
    /// ສ້າງ index ສຸດ is_deleted ເນື່ອງຈາກວ່າທຸກໆ Query ສະເຝີການກັ່ນຕອງໂດຍຖັນນີ້ຈາກ Global Query Filter
    /// </summary>
    private static void ApplySoftDeleteIndex(IMutableEntityType entityType)
    {
        IMutableProperty? isDeleted = entityType.FindProperty(nameof(ISoftDeletable.IsDeleted));
        if (isDeleted is null || entityType.FindIndex(new[] { isDeleted }) is not null)
        {
            return;
        }

        entityType.AddIndex(isDeleted);
    }

    /// <summary>ສ້າງ composite index ສໍາລັບການກັ່ນຕອງຂໍ້ມູນໂດຍອົງກອນແລະຫໍພັກ</summary>
    private static void ApplyTenantIndexes(IMutableEntityType entityType)
    {
        IMutableProperty? organizationId = entityType.FindProperty(nameof(ITenantScoped.OrganizationId));
        IMutableProperty? dormitoryId = entityType.FindProperty(nameof(ITenantScoped.DormitoryId));

        if (organizationId is null)
        {
            return;
        }

        if (dormitoryId is not null)
        {
            IMutableProperty[] tenantKeys = [organizationId, dormitoryId];
            if (entityType.FindIndex(tenantKeys) is null)
            {
                entityType.AddIndex(tenantKeys);
            }

            return;
        }

        if (entityType.FindIndex(new[] { organizationId }) is null)
        {
            entityType.AddIndex(organizationId);
        }
    }

    /// <summary>ກໍານົດຄວາມລະອຽດຂອງ decimal ທັງຝົດຂອງພວກເຂົາແມ່ນ numeric(18,2) ເພື່ອປ້ອງກັນບໍ່ໃຫ້ມູນຄ່າຂອງສະກຸນເງິນຖືກປະຕິເສດ</summary>
    private static void ApplyDecimalPrecision(IMutableEntityType entityType)
    {
        foreach (IMutableProperty property in entityType.GetProperties())
        {
            if (property.ClrType != typeof(decimal) && property.ClrType != typeof(decimal?))
            {
                continue;
            }

            if (property.GetPrecision() is null)
            {
                property.SetPrecision(MoneyPrecision);
                property.SetScale(MoneyScale);
            }
        }
    }
}
