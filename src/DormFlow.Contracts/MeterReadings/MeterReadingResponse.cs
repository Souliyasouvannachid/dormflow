namespace DormFlow.Contracts.MeterReadings;

/// <summary>ຂໍ້ມູນຄ່າໜຶ່ງແມັດ</summary>
public class MeterReadingResponse
{
    /// <summary>ລະຫັດອ້າງອີງລາຍການ</summary>
    public Guid Id { get; init; }

    /// <summary>ອົງກອນທີ່ກ່ຽວຂ້ອງ</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>ຫໍພັກທີ່ກ່ຽວຂ້ອງ</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>ຫ້ອງທີ່ບັນທຶກການອ່ານແມັດ</summary>
    public Guid RoomId { get; init; }

    /// <summary>ປະເພດແມັດ</summary>
    public string MeterType { get; init; } = string.Empty;

    /// <summary>ເດືອນທີ່ຄ່ານີ້ຖືກຮຽກເກັບ</summary>
    public DateOnly PeriodMonth { get; init; }

    /// <summary>ວັນທີລົງທະບຽນຕົວຈິງແມັດ</summary>
    public DateOnly ReadingDate { get; init; }

    /// <summary>ຄ່າແມັດກ່ອນໜ້າ</summary>
    public decimal PreviousReading { get; init; }

    /// <summary>ຄ່າແມັດທີ່ບັນທຶກໄວ້ໃນຮອບນີ້</summary>
    public decimal CurrentReading { get; init; }

    /// <summary>ຜ່ວຍງານທີ່ໃຊ້</summary>
    public decimal UnitsUsed { get; init; }

    /// <summary>ບັນທຶກເພີ່ມເຕີມ</summary>
    public string? Notes { get; init; }

    /// <summary>ເວລາສ້າງຂໍ້ມູນ</summary>
    public DateTimeOffset CreatedAt { get; init; }
}
