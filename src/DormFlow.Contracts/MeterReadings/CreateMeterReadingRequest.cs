namespace DormFlow.Contracts.MeterReadings;

/// <summary>คำขอบันทึกค่ามิเตอร์</summary>
public class CreateMeterReadingRequest
{
    /// <summary>ห้องที่จดค่ามิเตอร์</summary>
    public Guid RoomId { get; set; }

    /// <summary>ประเภทมิเตอร์ ("Water" หรือ "Electricity")</summary>
    public string MeterType { get; set; } = string.Empty;

    /// <summary>เดือนที่นำค่านี้ไปออกบิล ระบบจะปรับให้เป็นวันที่ 1 ของเดือนโดยอัตโนมัติ</summary>
    public DateOnly PeriodMonth { get; set; }

    /// <summary>วันที่จดมิเตอร์จริง</summary>
    public DateOnly ReadingDate { get; set; }

    /// <summary>ค่ามิเตอร์ที่จดได้ในรอบนี้ ต้องไม่น้อยกว่าค่าครั้งก่อนหน้า</summary>
    public decimal CurrentReading { get; set; }

    /// <summary>หมายเหตุเพิ่มเติม</summary>
    public string? Notes { get; set; }
}
