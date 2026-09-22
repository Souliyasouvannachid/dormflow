namespace DormFlow.Contracts.Reports;

/// <summary>ຕົວກໍານົດການຂອງບົດລາຍງານສະຫຼຸບລາຍຮັບ</summary>
public class RevenueReportRequest
{
    /// <summary>ກັ່ນຕອງສະເພາະຫໍພັກທີ່ລະບຸເປັນ null ເມື່ອເຈົ້າຕ້ອງການທຸກຫໍພັກທີ່ຜູ້ໂທມີການເຂົ້າເຖິງ</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>ເດືອນເລີ່ມຕົ້ນຂອງໄລຍະເວລາທີ່ຈະສະຫຼຸບ (ນັບຈາກ PeriodMonth ຂອງໃບເກັບເງິນ)</summary>
    public DateOnly PeriodFrom { get; set; }

    /// <summary>ເດືອນສຸດທ້າຍຂອງໄລຍະເວລາທີ່ຈະສະຫຼຸບ</summary>
    public DateOnly PeriodTo { get; set; }
}
