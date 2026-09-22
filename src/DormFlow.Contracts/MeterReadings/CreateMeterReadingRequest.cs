namespace DormFlow.Contracts.MeterReadings;

/// <summary>ຮ້ອງຂໍການບັນທຶກຄ່າແມັດ</summary>
public class CreateMeterReadingRequest
{
    /// <summary>ຫ້ອງທີ່ບັນທຶກການອ່ານແມັດ</summary>
    public Guid RoomId { get; set; }

    /// <summary>ປະເພດແມັດ ("Water" ຫຼື "Electricity")</summary>
    public string MeterType { get; set; } = string.Empty;

    /// <summary>ເດືອນທີ່ຄ່ານີ້ຖືກຮຽກເກັບ ລະບົບຈະປັບເປັນວັນທີ 1 ຂອງເດືອນອັດຕະໂນມັດ</summary>
    public DateOnly PeriodMonth { get; set; }

    /// <summary>ວັນທີລົງທະບຽນຕົວຈິງແມັດ</summary>
    public DateOnly ReadingDate { get; set; }

    /// <summary>ຄ່າແມັດທີ່ບັນທຶກໄວ້ໃນຮອບນີ້ຕ້ອງບໍ່ໜ້ອຍກວ່າຄ່າກ່ອນໜ້າ</summary>
    public decimal CurrentReading { get; set; }

    /// <summary>ບັນທຶກເພີ່ມເຕີມ</summary>
    public string? Notes { get; set; }
}
