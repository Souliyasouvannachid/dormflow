namespace DormFlow.Contracts.Expenses;

/// <summary>ຄໍາຮ້ອງສະຝັກບັນທຶກລາຍຈ່າຍ</summary>
public class CreateExpenseRequest
{
    /// <summary>ຫໍພັກທີ່ກ່ຽວຂ້ອງກັບຄ່າໃຊ້ຈ່າຍນີ້</summary>
    public Guid DormitoryId { get; set; }

    /// <summary>ປະເພດຂອງຄ່າໃຊ້ຈ່າຍ ("Utilities", "Maintenance", "Salaries", "Supplies", "Marketing", "Other")</summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>ຈໍານວນເງິນ</summary>
    public decimal Amount { get; set; }

    /// <summary>ວັນທີຈ່າຍຄ່າໃຊ້ຈ່າຍ</summary>
    public DateOnly ExpenseDate { get; set; }

    /// <summary>ລາຍລະອຽດລາຍການ</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>ຜູ້ຮັບເງິນ/ຮ້ານ/ຜູ້ໃຫ້ບໍລິການ</summary>
    public string? PaidTo { get; set; }

    /// <summary>ບັນທຶກເພີ່ມເຕີມ</summary>
    public string? Notes { get; set; }
}
