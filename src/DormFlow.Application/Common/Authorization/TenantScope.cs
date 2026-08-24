using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Exceptions;

namespace DormFlow.Application.Common.Authorization;

/// <summary>
/// ตรรกะกลางสำหรับบังคับขอบเขตข้อมูลตามองค์กรและหอพัก
/// </summary>
/// <remarks>
/// ใช้ร่วมกันทุก Use Case ที่ทำงานกับข้อมูลซึ่งแยกตามองค์กร (Dormitory, Building, Floor, RoomType, Room)
/// เพื่อไม่ให้แต่ละ Service ต้องเขียนตรรกะกันข้ามองค์กรซ้ำเอง และเพื่อให้พฤติกรรมสอดคล้องกันทั้งระบบ
/// </remarks>
public static class TenantScope
{
    /// <summary>
    /// หาองค์กรเป้าหมายสำหรับการสร้างข้อมูลใหม่
    /// </summary>
    /// <remarks>
    /// SuperAdmin ไม่ผูกกับองค์กรใด จึงต้องระบุ OrganizationId มาในคำขอเสมอ
    /// ผู้ใช้อื่นทุกคนถูกบังคับให้ใช้องค์กรของตนเองเท่านั้น ไม่ว่าคำขอจะระบุค่าใดมาก็ตาม
    /// เพื่อป้องกัน Mass Assignment ที่พยายามสร้างข้อมูลข้ามองค์กร
    /// </remarks>
    /// <param name="currentUser">ผู้ใช้ปัจจุบัน</param>
    /// <param name="requestedOrganizationId">องค์กรที่ระบุมาในคำขอ มีความหมายเฉพาะเมื่อผู้เรียกเป็น SuperAdmin</param>
    /// <exception cref="ValidationFailedException">เมื่อ SuperAdmin ไม่ได้ระบุองค์กรเป้าหมาย</exception>
    /// <exception cref="ForbiddenAccessException">เมื่อผู้ใช้ที่ไม่ใช่ SuperAdmin ไม่ได้สังกัดองค์กรใดเลย</exception>
    public static Guid ResolveOrganizationIdForCreate(ICurrentUser currentUser, Guid? requestedOrganizationId)
    {
        ArgumentNullException.ThrowIfNull(currentUser);

        if (currentUser.IsSuperAdmin)
        {
            return requestedOrganizationId
                ?? throw new ValidationFailedException("กรุณาระบุ organizationId เมื่อสร้างข้อมูลในฐานะ SuperAdmin");
        }

        return currentUser.OrganizationId
            ?? throw new ForbiddenAccessException("บัญชีนี้ไม่ได้สังกัดองค์กรใด จึงสร้างข้อมูลนี้ไม่ได้");
    }

    /// <summary>
    /// ตรวจว่าผู้ใช้เข้าถึงข้อมูลขององค์กรที่ระบุได้หรือไม่
    /// </summary>
    /// <remarks>
    /// ตอบเป็นไม่พบข้อมูล (404) แทนไม่มีสิทธิ์ (403) เมื่อข้อมูลอยู่นอกองค์กรของผู้ใช้
    /// เพื่อไม่ให้ผู้เรียกเดาได้ว่ามีข้อมูลนั้นอยู่จริงในองค์กรอื่น
    /// </remarks>
    /// <param name="currentUser">ผู้ใช้ปัจจุบัน</param>
    /// <param name="organizationId">องค์กรเจ้าของข้อมูลที่ต้องการเข้าถึง</param>
    /// <param name="notFoundMessage">ข้อความที่ใช้เมื่อเข้าถึงไม่ได้</param>
    /// <exception cref="NotFoundException">เมื่อองค์กรของข้อมูลไม่ตรงกับองค์กรของผู้ใช้</exception>
    public static void EnsureOrganizationAccess(ICurrentUser currentUser, Guid organizationId, string notFoundMessage)
    {
        ArgumentNullException.ThrowIfNull(currentUser);

        if (currentUser.IsSuperAdmin || currentUser.OrganizationId == organizationId)
        {
            return;
        }

        throw new NotFoundException(notFoundMessage);
    }

    /// <summary>
    /// ตรวจว่าผู้ใช้เข้าถึงข้อมูลของหอพักที่ระบุได้หรือไม่
    /// </summary>
    /// <remarks>
    /// ครอบคลุมทั้งขอบเขตองค์กรและขอบเขตหอพัก เพราะ <see cref="ICurrentUser.CanAccessDormitory"/>
    /// ไม่ได้ตรวจองค์กรของหอพักนั้นด้วย (ตรวจแค่ว่าอยู่ในรายการหอพักที่ได้รับมอบหมาย)
    /// </remarks>
    /// <param name="currentUser">ผู้ใช้ปัจจุบัน</param>
    /// <param name="organizationId">องค์กรเจ้าของหอพัก</param>
    /// <param name="dormitoryId">หอพักที่ต้องการเข้าถึง</param>
    /// <param name="notFoundMessage">ข้อความที่ใช้เมื่อเข้าถึงไม่ได้</param>
    /// <exception cref="NotFoundException">เมื่อไม่มีสิทธิ์เข้าถึงหอพักนั้น</exception>
    public static void EnsureDormitoryAccess(
        ICurrentUser currentUser,
        Guid organizationId,
        Guid dormitoryId,
        string notFoundMessage)
    {
        ArgumentNullException.ThrowIfNull(currentUser);

        bool sameOrganization = currentUser.IsSuperAdmin || currentUser.OrganizationId == organizationId;

        if (sameOrganization && currentUser.CanAccessDormitory(dormitoryId))
        {
            return;
        }

        throw new NotFoundException(notFoundMessage);
    }

    /// <summary>
    /// รายการหอพักที่ต้องจำกัดผลลัพธ์ของ List Endpoint ไว้ให้เห็นเฉพาะที่ได้รับมอบหมาย
    /// </summary>
    /// <remarks>
    /// คืนค่า null เมื่อผู้ใช้เห็นได้ทุกหอพักในองค์กรของตนอยู่แล้ว (SuperAdmin และ Owner)
    /// คืนรายการว่างเมื่อผู้ใช้ยังไม่ถูกมอบหมายหอพักใดเลย เพื่อให้ผลลัพธ์เป็นรายการว่างแทนที่จะเห็นข้อมูลทั้งหมดโดยไม่ตั้งใจ
    /// </remarks>
    /// <param name="currentUser">ผู้ใช้ปัจจุบัน</param>
    public static IReadOnlyCollection<Guid>? GetDormitoryRestrictionForListing(ICurrentUser currentUser)
    {
        ArgumentNullException.ThrowIfNull(currentUser);

        if (currentUser.IsSuperAdmin || currentUser.Roles.Contains(DormFlowRoles.Owner, StringComparer.Ordinal))
        {
            return null;
        }

        return currentUser.DormitoryIds;
    }
}
