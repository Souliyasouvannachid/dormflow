namespace DormFlow.Contracts.Payments;

/// <summary>คำขอคืนเงิน</summary>
public class RefundPaymentRequest
{
    /// <summary>สาเหตุการคืนเงิน</summary>
    public string? Reason { get; set; }
}
