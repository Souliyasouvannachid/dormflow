namespace DormFlow.Contracts.Rooms;

/// <summary>พารามิเตอร์การสรุปจำนวนห้องแยกตามสถานะ</summary>
public class RoomSummaryRequest
{
    /// <summary>กรองเฉพาะห้องของหอพักที่ระบุ</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>กรองเฉพาะห้องของอาคารที่ระบุ</summary>
    public Guid? BuildingId { get; set; }
}
