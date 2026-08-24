namespace DormFlow.Domain.Enums;

/// <summary>สถานะของผู้มาเยือน</summary>
public enum VisitorStatus
{
    /// <summary>เข้าหอพักแล้ว ยังไม่ออก</summary>
    CheckedIn = 1,

    /// <summary>ออกจากหอพักแล้ว</summary>
    CheckedOut = 2,
}
