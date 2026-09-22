namespace DormFlow.Contracts.Payments;

/// <summary>ການຮ້ອງຂໍຄືນເງິນ</summary>
public class RefundPaymentRequest
{
    /// <summary>ເຫດຜົນສໍາລັບການຄືນເງິນ</summary>
    public string? Reason { get; set; }
}
