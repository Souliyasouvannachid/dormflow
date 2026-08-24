using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>
/// ค่ามิเตอร์ของห้องหนึ่งห้องในรอบบิลหนึ่งเดือน
/// </summary>
/// <remarks>
/// ไม่มี Endpoint แก้ไขหรือลบโดยตั้งใจ (มีเฉพาะสิทธิ์ View และ Create)
/// เพราะถือเป็นบันทึกเชิงประวัติที่ใช้คำนวณค่าน้ำ-ไฟในใบแจ้งหนี้ หากจดผิดต้องจดรายการใหม่แก้ไขแทน
/// </remarks>
public class MeterReading : TenantEntity
{
    /// <summary>ห้องที่จดค่ามิเตอร์</summary>
    public Guid RoomId { get; set; }

    /// <summary>ประเภทมิเตอร์</summary>
    public MeterType MeterType { get; set; }

    /// <summary>เดือนที่นำค่านี้ไปออกบิล ปรับให้เป็นวันที่ 1 ของเดือนเสมอ</summary>
    public DateOnly PeriodMonth { get; set; }

    /// <summary>วันที่จดมิเตอร์จริง</summary>
    public DateOnly ReadingDate { get; set; }

    /// <summary>ค่ามิเตอร์ครั้งก่อนหน้า คัดลอกมาจากค่าล่าสุดของห้อง+ประเภทนี้โดยอัตโนมัติ</summary>
    public decimal PreviousReading { get; set; }

    /// <summary>ค่ามิเตอร์ที่จดได้ในรอบนี้</summary>
    public decimal CurrentReading { get; set; }

    /// <summary>หน่วยที่ใช้ไป คำนวณและเก็บไว้ตอนบันทึก (CurrentReading - PreviousReading)</summary>
    public decimal UnitsUsed { get; set; }

    /// <summary>หมายเหตุเพิ่มเติม</summary>
    public string? Notes { get; set; }
}
