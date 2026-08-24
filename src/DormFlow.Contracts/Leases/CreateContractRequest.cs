namespace DormFlow.Contracts.Leases;

/// <summary>คำขอสร้างสัญญาเช่าใหม่</summary>
public class CreateContractRequest
{
    /// <summary>ห้องที่จะทำสัญญา</summary>
    public Guid RoomId { get; set; }

    /// <summary>ผู้เช่าตามสัญญา</summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// การจองที่นำมาทำสัญญานี้ เว้นว่างเมื่อทำสัญญาโดยไม่ผ่านการจองล่วงหน้า
    /// เมื่อระบุ ห้องและผู้เช่าของการจองต้องตรงกับที่ระบุในคำขอนี้
    /// </summary>
    public Guid? ReservationId { get; set; }

    /// <summary>วันที่เริ่มสัญญา</summary>
    public DateOnly StartDate { get; set; }

    /// <summary>วันที่สิ้นสุดสัญญาตามกำหนด</summary>
    public DateOnly EndDate { get; set; }

    /// <summary>ค่าเช่ารายเดือน เว้นว่างเพื่อใช้ค่าเช่าปัจจุบันของห้องเป็นค่าเริ่มต้น</summary>
    public decimal? MonthlyRent { get; set; }

    /// <summary>ค่ามัดจำ เว้นว่างเพื่อใช้ค่ามัดจำปัจจุบันของห้องเป็นค่าเริ่มต้น</summary>
    public decimal? DepositAmount { get; set; }

    /// <summary>หมายเหตุเพิ่มเติม</summary>
    public string? Notes { get; set; }
}
