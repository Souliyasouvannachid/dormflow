namespace DormFlow.Contracts.Leases;

/// <summary>ຄໍາຮ້ອງສະຝັກສໍາລັບການຍົກເລີກສັນຍາການເຊົ່າ (ກວດເບິ່ງ)</summary>
public class TerminateContractRequest
{
    /// <summary>ເຫດຜົນສໍາລັບການຍົກເລີກສັນຍາ</summary>
    public string? Reason { get; set; }
}
