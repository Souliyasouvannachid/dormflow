using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.MeterReadings;

/// <summary>พารามิเตอร์การค้นหารายการค่ามิเตอร์</summary>
public class MeterReadingListRequest : PagedQueryRequest
{
    /// <summary>กรองเฉพาะรายการของหอพักที่ระบุ</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>กรองเฉพาะรายการของห้องที่ระบุ</summary>
    public Guid? RoomId { get; set; }

    /// <summary>กรองเฉพาะประเภทมิเตอร์ที่ระบุ</summary>
    public string? MeterType { get; set; }

    /// <summary>กรองเฉพาะเดือนที่นำไปออกบิลที่ระบุ</summary>
    public DateOnly? PeriodMonth { get; set; }
}
