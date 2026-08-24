namespace DormFlow.Domain.Enums;

/// <summary>สถานะของสัญญาเช่า</summary>
public enum ContractStatus
{
    /// <summary>ร่างสัญญา ยังไม่ได้เปิดใช้งาน ห้องถูกกันไว้รอการเข้าพัก</summary>
    Draft = 1,

    /// <summary>เปิดใช้งานแล้ว ผู้เช่าเข้าพักอยู่</summary>
    Active = 2,

    /// <summary>สิ้นสุดสัญญาแล้ว ผู้เช่าย้ายออกแล้ว</summary>
    Terminated = 3,

    /// <summary>หมดอายุตามกำหนดในสัญญาโดยไม่มีการต่ออายุ</summary>
    Expired = 4,
}
