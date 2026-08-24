namespace DormFlow.Contracts.Deposits;

/// <summary>คำขอบันทึกการรับเงินมัดจำ</summary>
public class CollectDepositRequest
{
    /// <summary>สัญญาเช่าที่รับเงินมัดจำ</summary>
    public Guid ContractId { get; set; }

    /// <summary>จำนวนเงินมัดจำที่รับไว้ ถ้าไม่ระบุใช้ค่า DepositAmount ของสัญญา</summary>
    public decimal? Amount { get; set; }

    /// <summary>หมายเหตุเพิ่มเติม</summary>
    public string? Notes { get; set; }
}
