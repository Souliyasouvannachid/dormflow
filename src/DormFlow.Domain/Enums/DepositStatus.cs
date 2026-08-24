namespace DormFlow.Domain.Enums;

/// <summary>สถานะของเงินมัดจำ</summary>
public enum DepositStatus
{
    /// <summary>รับเงินมัดจำไว้แล้ว ยังไม่มีการคืน</summary>
    Held = 1,

    /// <summary>คืนเงินมัดจำบางส่วนแล้ว</summary>
    PartiallyRefunded = 2,

    /// <summary>คืนเงินมัดจำครบแล้ว</summary>
    Refunded = 3,

    /// <summary>ริบเงินมัดจำทั้งหมด ไม่มีการคืน</summary>
    Forfeited = 4,
}
