using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>
/// สัญญาเช่าห้องพักหนึ่งฉบับ
/// </summary>
/// <remarks>
/// ระบบยังไม่มี Entity แยกสำหรับ CheckIn/CheckOut ในขั้นตอนนี้
/// เพราะเอกสารต้นฉบับไม่ได้ระบุรายละเอียดของสองโมดูลนี้ไว้ (ต่างจาก Phase 1 ที่ระบุไว้ครบ)
/// จึงรวมเป็นการกระทำบนสัญญาแทน: เปิดใช้งานสัญญา (<see cref="ContractStatus.Active"/>) คือจุดที่เช็คอิน
/// และสิ้นสุดสัญญา (<see cref="ContractStatus.Terminated"/>) คือจุดที่เช็คเอาท์
/// หากภายหลังต้องการบันทึกรายละเอียดการตรวจสภาพห้องหรือรูปถ่ายตอนเข้า-ออก
/// ค่อยแยกเป็น Entity ของตัวเองในภายหลังได้โดยไม่กระทบโครงสร้างนี้
/// </remarks>
public class Contract : TenantEntity
{
    /// <summary>ห้องที่ทำสัญญา</summary>
    public Guid RoomId { get; set; }

    /// <summary>ผู้เช่าตามสัญญา</summary>
    public Guid TenantId { get; set; }

    /// <summary>การจองที่นำมาทำสัญญานี้ เป็น null เมื่อทำสัญญาโดยไม่ผ่านการจองล่วงหน้า</summary>
    public Guid? ReservationId { get; set; }

    /// <summary>เลขที่สัญญา ไม่ซ้ำกันภายในองค์กรเดียวกัน</summary>
    public required string ContractNumber { get; set; }

    /// <summary>วันที่เริ่มสัญญา</summary>
    public DateOnly StartDate { get; set; }

    /// <summary>วันที่สิ้นสุดสัญญาตามกำหนด</summary>
    public DateOnly EndDate { get; set; }

    /// <summary>ค่าเช่ารายเดือนตามสัญญานี้ คัดลอกมาจากห้องตอนทำสัญญา ไม่เปลี่ยนตามค่าเช่าห้องในภายหลัง</summary>
    public decimal MonthlyRent { get; set; }

    /// <summary>ค่ามัดจำตามสัญญานี้</summary>
    public decimal DepositAmount { get; set; }

    /// <summary>สถานะของสัญญา</summary>
    public ContractStatus Status { get; set; } = ContractStatus.Draft;

    /// <summary>เวลาที่เปิดใช้งานสัญญา (เช็คอิน) เป็น null เมื่อยังเป็นร่าง</summary>
    public DateTimeOffset? SignedAt { get; set; }

    /// <summary>เวลาที่สิ้นสุดสัญญา (เช็คเอาท์) เป็น null เมื่อยังไม่สิ้นสุด</summary>
    public DateTimeOffset? TerminatedAt { get; set; }

    /// <summary>สาเหตุที่สิ้นสุดสัญญาก่อนกำหนดหรือครบกำหนด</summary>
    public string? TerminationReason { get; set; }

    /// <summary>หมายเหตุเพิ่มเติม</summary>
    public string? Notes { get; set; }
}
