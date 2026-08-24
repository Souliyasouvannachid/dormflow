using System.Linq.Expressions;
using DormFlow.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DormFlow.Infrastructure.Persistence.Extensions;

/// <summary>
/// ข้อตกลงการแม็ปโมเดลที่ใช้กับทุก Entity ของ DormFlow
/// </summary>
/// <remarks>
/// รวมไว้ที่เดียวแทนการเขียนซ้ำในทุก EntityTypeConfiguration
/// ทำให้ Entity ที่เพิ่มเข้ามาภายหลังได้ Soft Delete Filter, Audit Column
/// และ Optimistic Concurrency ไปโดยอัตโนมัติ
/// </remarks>
public static class ModelBuilderExtensions
{
    /// <summary>ชื่อคอลัมน์ระบบของ PostgreSQL ที่ใช้ทำ Optimistic Concurrency</summary>
    private const string XminColumnName = "xmin";

    /// <summary>ความละเอียดมาตรฐานของค่าที่เป็นจำนวนเงิน</summary>
    private const int MoneyPrecision = 18;

    /// <summary>จำนวนทศนิยมมาตรฐานของค่าที่เป็นจำนวนเงิน</summary>
    private const int MoneyScale = 2;

    /// <summary>
    /// ใส่ข้อตกลงการแม็ปทั้งหมดของ DormFlow ให้กับโมเดล
    /// ต้องเรียกหลังจาก ApplyConfigurationsFromAssembly เสมอ
    /// </summary>
    /// <param name="modelBuilder">ตัวสร้างโมเดลของ EF Core</param>
    public static void ApplyDormFlowConventions(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
        {
            ApplyDecimalPrecision(entityType);

            // ตรวจจากการมี property ชื่อ Version แทนการตรวจคลาสฐาน
            // เพื่อให้ Entity ของ Identity ที่ใช้ ConcurrencyStamp อยู่แล้วถูกข้ามไปเอง
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
    /// แม็ป property Version เข้ากับคอลัมน์ระบบ xmin ของ PostgreSQL
    /// PostgreSQL อัปเดตค่านี้ให้เองทุกครั้งที่แถวถูกแก้ไข จึงไม่ต้องจัดการเวอร์ชันเอง
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

    /// <summary>กรองแถวที่ถูก Soft Delete ออกจากทุก Query โดยอัตโนมัติ</summary>
    private static void ApplySoftDeleteQueryFilter(ModelBuilder modelBuilder, Type clrType)
    {
        // สร้างนิพจน์ entity => !entity.IsDeleted แบบ dynamic เพราะรู้ชนิดตอน runtime เท่านั้น
        ParameterExpression parameter = Expression.Parameter(clrType, "entity");
        MemberExpression isDeleted = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
        UnaryExpression notDeleted = Expression.Not(isDeleted);

        modelBuilder.Entity(clrType).HasQueryFilter(Expression.Lambda(notDeleted, parameter));
    }

    /// <summary>
    /// สร้าง index บน is_deleted เพราะทุก Query ถูกกรองด้วยคอลัมน์นี้เสมอจาก Global Query Filter
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

    /// <summary>สร้าง composite index สำหรับการกรองข้อมูลตามองค์กรและหอพัก</summary>
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

    /// <summary>กำหนดความละเอียดของ decimal ทุกตัวเป็น numeric(18,2) เพื่อไม่ให้ค่าเงินถูกปัดทิ้ง</summary>
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
