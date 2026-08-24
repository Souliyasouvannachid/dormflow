namespace DormFlow.Contracts.MeterReadings;

/// <summary>ข้อมูลค่ามิเตอร์หนึ่งรายการ</summary>
public class MeterReadingResponse
{
    /// <summary>รหัสอ้างอิงของรายการ</summary>
    public Guid Id { get; init; }

    /// <summary>องค์กรที่เกี่ยวข้อง</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>หอพักที่เกี่ยวข้อง</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>ห้องที่จดค่ามิเตอร์</summary>
    public Guid RoomId { get; init; }

    /// <summary>ประเภทมิเตอร์</summary>
    public string MeterType { get; init; } = string.Empty;

    /// <summary>เดือนที่นำค่านี้ไปออกบิล</summary>
    public DateOnly PeriodMonth { get; init; }

    /// <summary>วันที่จดมิเตอร์จริง</summary>
    public DateOnly ReadingDate { get; init; }

    /// <summary>ค่ามิเตอร์ครั้งก่อนหน้า</summary>
    public decimal PreviousReading { get; init; }

    /// <summary>ค่ามิเตอร์ที่จดได้ในรอบนี้</summary>
    public decimal CurrentReading { get; init; }

    /// <summary>หน่วยที่ใช้ไป</summary>
    public decimal UnitsUsed { get; init; }

    /// <summary>หมายเหตุเพิ่มเติม</summary>
    public string? Notes { get; init; }

    /// <summary>เวลาที่สร้างข้อมูล</summary>
    public DateTimeOffset CreatedAt { get; init; }
}
