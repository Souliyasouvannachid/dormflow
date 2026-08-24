using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.MeterReadings;

/// <summary>ตัวกรองสำหรับดึงรายการค่ามิเตอร์แบบแบ่งหน้า</summary>
public sealed record MeterReadingListFilter(
    Guid? DormitoryId,
    Guid? RoomId,
    MeterType? MeterType,
    DateOnly? PeriodMonth,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>เข้าถึงข้อมูลค่ามิเตอร์ในฐานข้อมูล</summary>
public interface IMeterReadingRepository
{
    /// <summary>อ่านค่ามิเตอร์จากรหัสอ้างอิง</summary>
    Task<MeterReading?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ตรวจว่าห้อง+ประเภทมิเตอร์นี้ถูกจดค่าของเดือนที่ระบุไปแล้วหรือยัง</summary>
    Task<bool> ExistsForPeriodAsync(
        Guid roomId,
        MeterType meterType,
        DateOnly periodMonth,
        CancellationToken cancellationToken = default);

    /// <summary>อ่านค่ามิเตอร์ล่าสุดของห้อง+ประเภทนี้ก่อนเดือนที่ระบุ ใช้เป็นค่าเริ่มต้นของ PreviousReading</summary>
    Task<MeterReading?> GetLatestBeforePeriodAsync(
        Guid roomId,
        MeterType meterType,
        DateOnly periodMonth,
        CancellationToken cancellationToken = default);

    /// <summary>อ่านค่ามิเตอร์ของห้อง+ประเภทมิเตอร์นี้ในเดือนที่ระบุ ใช้ตอนออกใบแจ้งหนี้</summary>
    Task<MeterReading?> GetForPeriodAsync(
        Guid roomId,
        MeterType meterType,
        DateOnly periodMonth,
        CancellationToken cancellationToken = default);

    /// <summary>ดึงรายการค่ามิเตอร์แบบแบ่งหน้าตามตัวกรองที่ระบุ</summary>
    Task<PagedResult<MeterReading>> GetPagedAsync(
        MeterReadingListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>เพิ่มค่ามิเตอร์ใหม่</summary>
    Task AddAsync(MeterReading meterReading, CancellationToken cancellationToken = default);

    /// <summary>บันทึกการเปลี่ยนแปลงทั้งหมดลงฐานข้อมูล</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
