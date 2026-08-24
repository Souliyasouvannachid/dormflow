using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>
/// การจดทะเบียนที่จอดรถของผู้เช่าหนึ่งคันหนึ่งครั้ง
/// </summary>
/// <remarks>
/// ต่างจาก <see cref="Parcel"/>/<see cref="Visitor"/> ที่ผู้เช่าเป็นทางเลือก — ที่นี่ผู้เช่าเป็นข้อมูลบังคับ
/// เพราะรถหนึ่งคันต้องมีเจ้าของที่ระบุตัวตนได้เสมอ ส่วนห้องยังคงเก็บไว้เพื่อขอบเขตองค์กร/หอพักเช่นเดิม
/// ไม่มี Entity แม่ของ "ช่องจอดรถ" แยกต่างหาก (<see cref="ParkingSpotLabel"/> เป็นข้อความอิสระ) เพราะไม่มี
/// ข้อกำหนดเรื่องการจัดการช่องจอดเป็นระบบจากเอกสารต้นฉบับ
/// </remarks>
public class ParkingRegistration : TenantEntity
{
    /// <summary>ห้องของผู้เช่าเจ้าของรถ</summary>
    public Guid RoomId { get; set; }

    /// <summary>ผู้เช่าเจ้าของรถ</summary>
    public Guid TenantId { get; set; }

    /// <summary>ทะเบียนรถ</summary>
    public required string LicensePlate { get; set; }

    /// <summary>ประเภทของยานพาหนะ</summary>
    public VehicleType VehicleType { get; set; }

    /// <summary>ยี่ห้อ/รุ่นรถ</summary>
    public string? Brand { get; set; }

    /// <summary>สีรถ</summary>
    public string? Color { get; set; }

    /// <summary>ป้ายช่องจอดรถ (ถ้ามีการกำหนด) เก็บเป็นข้อความอิสระ</summary>
    public string? ParkingSpotLabel { get; set; }

    /// <summary>สถานะของการจดทะเบียน</summary>
    public ParkingRegistrationStatus Status { get; set; } = ParkingRegistrationStatus.Active;

    /// <summary>เวลาที่จดทะเบียน</summary>
    public DateTimeOffset RegisteredAt { get; set; }

    /// <summary>เวลาที่สิ้นสุดการจดทะเบียน เป็น null เมื่อยังจดทะเบียนอยู่</summary>
    public DateTimeOffset? EndedAt { get; set; }

    /// <summary>หมายเหตุเพิ่มเติม</summary>
    public string? Notes { get; set; }
}
