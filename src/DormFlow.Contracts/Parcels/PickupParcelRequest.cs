namespace DormFlow.Contracts.Parcels;

/// <summary>คำขอบันทึกการรับพัสดุ</summary>
public class PickupParcelRequest
{
    /// <summary>ชื่อผู้มารับพัสดุจริง</summary>
    public string? PickedUpByName { get; set; }
}
