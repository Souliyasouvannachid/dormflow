namespace DormFlow.Contracts.Expenses;

/// <summary>ຂໍ້ມູນຄ່າໃຊ້ຈ່າຍຜຶ່ງ</summary>
public class ExpenseResponse
{
    public Guid? MaintenanceRequestId { get; init; }

    /// <summary>ລະຫັດອ້າງອີງລາຍການ</summary>
    public Guid Id { get; init; }

    /// <summary>ອົງກອນທີ່ກ່ຽວຂ້ອງ</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>ຫໍພັກທີ່ກ່ຽວຂ້ອງ</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>ປະເພດຂອງຄ່າໃຊ້ຈ່າຍ</summary>
    public string Category { get; init; } = string.Empty;

    /// <summary>ຈໍານວນເງິນ</summary>
    public decimal Amount { get; init; }

    /// <summary>ວັນທີຈ່າຍຄ່າໃຊ້ຈ່າຍ</summary>
    public DateOnly ExpenseDate { get; init; }

    /// <summary>ລາຍລະອຽດລາຍການ</summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>ຜູ້ຮັບເງິນ/ຮ້ານ/ຜູ້ໃຫ້ບໍລິການ</summary>
    public string? PaidTo { get; init; }

    /// <summary>ບັນທຶກເພີ່ມເຕີມ</summary>
    public string? Notes { get; init; }

    /// <summary>ເວລາສ້າງຂໍ້ມູນ</summary>
    public DateTimeOffset CreatedAt { get; init; }
}
