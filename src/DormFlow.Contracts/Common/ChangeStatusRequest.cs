namespace DormFlow.Contracts.Common;

/// <summary>
/// คำขอเปลี่ยนสถานะ ใช้ร่วมกันกับทุก Entity ที่มีคำสั่ง PATCH .../status
/// </summary>
/// <typeparam name="TStatus">ชนิดของ Enum สถานะของ Entity นั้น</typeparam>
public class ChangeStatusRequest<TStatus>
    where TStatus : struct, Enum
{
    /// <summary>สถานะใหม่ที่ต้องการตั้ง</summary>
    public TStatus Status { get; set; }
}
