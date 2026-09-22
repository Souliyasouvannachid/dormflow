namespace DormFlow.Domain.Common;

/// <summary>
/// ຊັ້ນພື້ນຖານຂອງ Entity ທຸກໆດ້ານທຸລະກິດທີ່ຕ້ອງການແຍກຂໍ້ມູນໂດຍອົງກອນແລະຫໍພັກ
/// ທຸກໆ Query ທີ່ອ່ານ Entity ປະເພດນີ້ຍັງຕ້ອງໄດ້ຮັບການກັ່ນຕອງ OrganizationId ສະເໝີ
/// </summary>
public abstract class TenantEntity : BaseEntity, ITenantScoped
{
    /// <inheritdoc />
    public Guid OrganizationId { get; set; }

    /// <inheritdoc />
    public Guid? DormitoryId { get; set; }
}
