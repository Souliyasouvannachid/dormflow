using DormFlow.Contracts.Common;
using DormFlow.Contracts.Dormitories;

namespace DormFlow.Application.Features.Dormitories;

/// <summary>Use Case ทั้งหมดของโมดูลหอพัก</summary>
public interface IDormitoryService
{
    /// <summary>ดึงรายการหอพักแบบแบ่งหน้า</summary>
    /// <param name="request">พารามิเตอร์การค้นหา</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    Task<PagedResult<DormitoryResponse>> GetPagedAsync(
        DormitoryListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>อ่านข้อมูลหอพักจากรหัสอ้างอิง</summary>
    /// <param name="id">รหัสหอพัก</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    Task<DormitoryResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>สร้างหอพักใหม่</summary>
    /// <param name="request">ข้อมูลหอพักที่ต้องการสร้าง</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    Task<DormitoryResponse> CreateAsync(
        CreateDormitoryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>แก้ไขข้อมูลหอพัก</summary>
    /// <param name="id">รหัสหอพักที่ต้องการแก้ไข</param>
    /// <param name="request">ข้อมูลใหม่</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    Task<DormitoryResponse> UpdateAsync(
        Guid id,
        UpdateDormitoryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>เปลี่ยนสถานะหอพัก</summary>
    /// <param name="id">รหัสหอพักที่ต้องการเปลี่ยนสถานะ</param>
    /// <param name="status">สถานะใหม่ ส่งเป็นชื่อของ enum เช่น Active</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    Task<DormitoryResponse> ChangeStatusAsync(
        Guid id,
        string status,
        CancellationToken cancellationToken = default);

    /// <summary>ลบหอพัก</summary>
    /// <param name="id">รหัสหอพักที่ต้องการลบ</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
