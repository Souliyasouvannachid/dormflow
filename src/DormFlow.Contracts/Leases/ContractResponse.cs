namespace DormFlow.Contracts.Leases;

/// <summary>ข้อมูลสัญญาเช่าหนึ่งฉบับ</summary>
public class ContractResponse
{
    /// <summary>รหัสอ้างอิงของสัญญา</summary>
    public Guid Id { get; init; }

    /// <summary>องค์กรที่เกี่ยวข้อง</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>หอพักที่เกี่ยวข้อง</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>ห้องที่ทำสัญญา</summary>
    public Guid RoomId { get; init; }

    /// <summary>ผู้เช่าตามสัญญา</summary>
    public Guid TenantId { get; init; }

    /// <summary>การจองที่นำมาทำสัญญานี้ เป็น null เมื่อทำสัญญาโดยไม่ผ่านการจองล่วงหน้า</summary>
    public Guid? ReservationId { get; init; }

    /// <summary>เลขที่สัญญา</summary>
    public string ContractNumber { get; init; } = string.Empty;

    /// <summary>วันที่เริ่มสัญญา</summary>
    public DateOnly StartDate { get; init; }

    /// <summary>วันที่สิ้นสุดสัญญาตามกำหนด</summary>
    public DateOnly EndDate { get; init; }

    /// <summary>ค่าเช่ารายเดือนตามสัญญานี้</summary>
    public decimal MonthlyRent { get; init; }

    /// <summary>ค่ามัดจำตามสัญญานี้</summary>
    public decimal DepositAmount { get; init; }

    /// <summary>สถานะของสัญญา</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>เวลาที่เปิดใช้งานสัญญา (เช็คอิน)</summary>
    public DateTimeOffset? SignedAt { get; init; }

    /// <summary>เวลาที่สิ้นสุดสัญญา (เช็คเอาท์)</summary>
    public DateTimeOffset? TerminatedAt { get; init; }

    /// <summary>สาเหตุที่สิ้นสุดสัญญา</summary>
    public string? TerminationReason { get; init; }

    /// <summary>หมายเหตุเพิ่มเติม</summary>
    public string? Notes { get; init; }

    /// <summary>เวลาที่สร้างข้อมูล</summary>
    public DateTimeOffset CreatedAt { get; init; }
}
