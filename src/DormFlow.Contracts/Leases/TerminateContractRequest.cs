namespace DormFlow.Contracts.Leases;

/// <summary>คำขอสิ้นสุดสัญญาเช่า (เช็คเอาท์)</summary>
public class TerminateContractRequest
{
    /// <summary>สาเหตุที่สิ้นสุดสัญญา</summary>
    public string? Reason { get; set; }
}
