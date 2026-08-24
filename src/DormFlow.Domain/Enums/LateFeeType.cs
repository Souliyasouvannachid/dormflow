namespace DormFlow.Domain.Enums;

/// <summary>รูปแบบการคิดค่าปรับชำระล่าช้าของหอพัก</summary>
public enum LateFeeType
{
    /// <summary>คิดเป็นจำนวนเงินคงที่</summary>
    Fixed = 1,

    /// <summary>คิดเป็นร้อยละของยอดค้างชำระ</summary>
    Percentage = 2,
}
