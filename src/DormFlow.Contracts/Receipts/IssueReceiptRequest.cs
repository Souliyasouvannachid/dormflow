namespace DormFlow.Contracts.Receipts;

/// <summary>ຮ້ອງຂໍການອອກໃບຮັບເງິນ</summary>
public class IssueReceiptRequest
{
    /// <summary>ລາຍການຈ່າຍເງິນທີ່ຕ້ອງການໃບຮັບເງິນຕ້ອງເປັນລາຍການທີ່ຢືນຢັນ (Confirmed) ເທົ່ານັ້ນ</summary>
    public Guid PaymentId { get; set; }

    /// <summary>ບັນທຶກເພີ່ມເຕີມ</summary>
    public string? Notes { get; set; }
}
