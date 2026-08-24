using DormFlow.Contracts.Common;
using DormFlow.Contracts.Parking;

namespace DormFlow.Application.Features.Parking;

/// <summary>Use Case ทั้งหมดของโมดูลจดทะเบียนที่จอดรถ</summary>
public interface IParkingRegistrationService
{
    /// <summary>ดึงรายการจดทะเบียนที่จอดรถแบบแบ่งหน้า</summary>
    Task<PagedResult<ParkingRegistrationResponse>> GetPagedAsync(
        ParkingRegistrationListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>อ่านข้อมูลการจดทะเบียนที่จอดรถจากรหัสอ้างอิง</summary>
    Task<ParkingRegistrationResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>จดทะเบียนที่จอดรถใหม่ ทะเบียนรถเดียวกันมีการจดทะเบียนที่ยังไม่สิ้นสุดซ้อนกันไม่ได้</summary>
    Task<ParkingRegistrationResponse> RegisterAsync(
        RegisterParkingRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>สิ้นสุดการจดทะเบียนที่จอดรถ ทำได้เฉพาะรายการที่ยังจดทะเบียนอยู่เท่านั้น</summary>
    Task<ParkingRegistrationResponse> ReleaseAsync(Guid id, CancellationToken cancellationToken = default);
}
