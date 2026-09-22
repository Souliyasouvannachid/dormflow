using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Exceptions;

namespace DormFlow.Application.Common.Authorization;

/// <summary>
/// ເຫດຜົນສູນກາງສໍາລັບການບັງຄັບໃຊ້ຂອບເຂດຂໍ້ມູນໂດຍອົງກອນແລະຫໍພັກ
/// </summary>
/// <remarks>
/// ໃຊ້ຮ່ວມກັນທັງຝົດ Use Case ທີ່ເຮັດວຽກກັບຂໍ້ມູນທີ່ແຍກອອກໂດຍອົງກອນ (Dormitory, Building, Floor, RoomType, Room)
/// ເພື່ອບໍ່ໃຫ້ແຕ່ລະຄົນ Service ເຫດຜົນຕ້ອງຖືກຂຽນຄືນໃຝ່ໃນທົ່ວອົງກອນ. ແລະເພື່ອຮັບປະກັນພຶດຕິກໍາທີ່ສອດຄ່ອງໃນທົ່ວລະບົບ
/// </remarks>
public static class TenantScope
{
    /// <summary>
    /// ຊອກຫາອົງກອນເປົ້າຝາຍສໍາລັບການສ້າງຂໍ້ມູນໃຝ່
    /// </summary>
    /// <remarks>
    /// SuperAdmin ບໍ່ໄດ້ຜູກມັດກັບອົງກອນໃດກໍ່ຕາມ, ສະນັ້ນຕ້ອງໄດ້ຮັບການລະບຸ OrganizationId ສະເຝີມາໃນຄໍາຮ້ອງຂໍ
    /// ຜູ້ໃຊ້ອື່ນໆທຸກຄົນຖືກບັງຄັບໃຫ້ໃຊ້ພຽງແຕ່ອົງກອນຂອງຕົນເອງ. ໂດຍບໍ່ຄໍານຶງເຖິງມູນຄ່າສິ່ງທີ່ຄໍາຮ້ອງຂໍກໍານົດ
    /// ເພື່ອປົກປ້ອງ Mass Assignment ທີ່ພະຍາຍາມສ້າງຂໍ້ມູນໃນທົ່ວອົງກອນ
    /// </remarks>
    /// <param name="currentUser">ຜູ້ໃຊ້ປະຈຸບັນ</param>
    /// <param name="requestedOrganizationId">ອົງກອນທີ່ລະບຸໄວ້ໃນຄໍາຮ້ອງຂໍມັນມີຄວາມຝາຍພຽງແຕ່ຖ້າຜູ້ໂທແມ່ນ SuperAdmin</param>
    /// <exception cref="ValidationFailedException">ເມື່ອໃດ SuperAdmin ອົງກອນເປົ້າໝາຍບໍ່ໄດ້ລະບຸ</exception>
    /// <exception cref="ForbiddenAccessException">ໃນເວລາທີ່ບໍ່ແມ່ນຜູ້ໃຊ້ SuperAdmin ບໍ່ມີສ່ວນກ່ຽວຂ້ອງກັບອົງກອນໃດໆ</exception>
    public static Guid ResolveOrganizationIdForCreate(ICurrentUser currentUser, Guid? requestedOrganizationId)
    {
        ArgumentNullException.ThrowIfNull(currentUser);

        if (currentUser.IsSuperAdmin)
        {
            return requestedOrganizationId
                ?? throw new ValidationFailedException("ກະລຸນາລະບຸ organizationId ເມື່ອສ້າງຂໍ້ມູນໃນຖານະ SuperAdmin");
        }

        return currentUser.OrganizationId
            ?? throw new ForbiddenAccessException("ບັນຊີນີ້ບໍ່ໄດ້ສັງກັດອົງກອນໃດ ຈຶ່ງສ້າງຂໍ້ມູນນີ້ບໍ່ໄດ້");
    }

    /// <summary>
    /// ກວດເບິ່ງວ່າຜູ້ໃຊ້ມີການເຂົ້າເຖິງຂໍ້ມູນຂອງອົງກອນທີ່ລະບຸ
    /// </summary>
    /// <remarks>
    /// ຄໍາຕອບແມ່ນວ່າບໍ່ພົບຂໍ້ມູນໃດໆ (404) ແທນທີ່ຈະບໍ່ມີສິດ (403) ເມື່ອຂໍ້ມູນຢູ່ນອກອົງກອນຂອງຜູ້ໃຊ້
    /// ເພື່ອປ້ອງກັນບໍ່ໃຫ້ຜູ້ໂທຄາດເດົາວ່າຂໍ້ມູນນັ້ນມີຢູ່ໃນອົງກອນອື່ນ
    /// </remarks>
    /// <param name="currentUser">ຜູ້ໃຊ້ປະຈຸບັນ</param>
    /// <param name="organizationId">ອົງກອນທີ່ເປັນເຈົ້າຂອງຂໍ້ມູນທີ່ຕ້ອງການເຂົ້າເຖິງ</param>
    /// <param name="notFoundMessage">ຂໍ້ຄວາມຖືກໃຊ້ເມື່ອບໍ່ສາມາດຕິດຕໍ່ໄດ້</param>
    /// <exception cref="NotFoundException">ໃນເວລາທີ່ອົງກອນຂອງຂໍ້ມູນບໍ່ກົງກັບອົງກອນຂອງຜູ້ໃຊ້</exception>
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
    /// ກວດເບິ່ງວ່າຜູ້ໃຊ້ສາມາດເຂົ້າເຖິງຂໍ້ມູນຂອງຫໍພັກທີ່ລະບຸ
    /// </summary>
    /// <remarks>
    /// ກວມເອົາທັງຂອບເຂດອົງກອນແລະເຂດແດນຫໍພັກເພາະວ່າ <see cref="ICurrentUser.CanAccessDormitory"/>
    /// ບໍ່ໄດ້ກວດເບິ່ງການຈັດຕັ້ງຂອງຫໍພັກນັ້ນ (ພຽງແຕ່ກວດເບິ່ງວ່າມັນຢູ່ໃນບັນຊີລາຍຊື່ຂອງຫໍພັກທີ່ໄດ້ຮັບມອບຝາຍ)
    /// </remarks>
    /// <param name="currentUser">ຜູ້ໃຊ້ປະຈຸບັນ</param>
    /// <param name="organizationId">ອົງກອນເຈົ້າຂອງຫໍພັກ</param>
    /// <param name="dormitoryId">ຫໍພັກທີ່ເຈົ້າຕ້ອງການເຂົ້າ</param>
    /// <param name="notFoundMessage">ຂໍ້ຄວາມຖືກໃຊ້ເມື່ອບໍ່ສາມາດຕິດຕໍ່ໄດ້</param>
    /// <exception cref="NotFoundException">ເມື່ອບໍ່ມີສິດເຂົ້າຫາຫໍພັກນັ້ນ</exception>
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
    /// ບັນຊີລາຍຊື່ຂອງຫໍພັກທີ່ຕ້ອງຈໍາກັດຜົນໄດ້ຮັບຂອງ List Endpoint ເພື່ອຈະໄດ້ຮັບການເຫັນພຽງແຕ່ເປັນການມອບຝາຍ
    /// </summary>
    /// <remarks>
    /// ຟື້ນຟູ null ເມື່ອຜູ້ໃຊ້ສາມາດເຫັນທຸກຫໍພັກໃນອົງກອນຂອງພວກເຂົາແລ້ວ (SuperAdmin ແລະ Owner)
    /// ສົ່ງຄືນລາຍຊື່ຫວ່າງເປົ່າເມື່ອຜູ້ໃຊ້ຍັງບໍ່ທັນໄດ້ມອບໝາຍຫໍພັກໃດໆເທື່ອ. ດັ່ງນັ້ນຜົນໄດ້ຮັບແມ່ນບັນຊີລາຍຊື່ເປົ່າແທນທີ່ຈະເຫັນຂໍ້ມູນທັງຝົດໂດຍບັງເອີນ
    /// </remarks>
    /// <param name="currentUser">ຜູ້ໃຊ້ປະຈຸບັນ</param>
    public static IReadOnlyCollection<Guid>? GetDormitoryRestrictionForListing(ICurrentUser currentUser)
    {
        ArgumentNullException.ThrowIfNull(currentUser);

        if (currentUser.IsSuperAdmin || currentUser.Roles.Contains(DormFlowRoles.Owner, StringComparer.Ordinal))
        {
            return null;
        }

        return currentUser.DormitoryIds;
    }

    /// <summary>
    /// ອົງກອນທີ່ຕ້ອງຈໍາກັດຜົນໄດ້ຮັບຂອງ List Endpoint ພຽງແຕ່ຂໍ້ມູນຂອງອົງກອນນັ້ນແມ່ນເຫັນໄດ້
    /// </summary>
    /// <remarks>
    /// ສຳຄັນ: ຕ້ອງໃຊ້ຮ່ວມກັບ <see cref="GetDormitoryRestrictionForListing"/> ສະເຝີໄປໃນທຸກ Use Case ທີ່ດຶງລາຍການ
    /// ຂໍ້ມູນແຍກໂດຍຫໍພັກ (Entity ທີ່ມີ DormitoryId) ເນື່ອງຈາກວ່າ <see cref="GetDormitoryRestrictionForListing"/>
    /// ຟື້ນຟູ null ໃຫ້ Owner (ຝາຍຄວາມວ່າ "ຫໍພັກບໍ່ຈໍາກັດ ທ່ານສາມາດເບິ່ງມັນຢູ່ໃນທຸກຫໍພັກໃນອົງກອນຂອງເຈົ້າ") ແຕ່ມັນບໍ່ໄດ້ກວມເອົາຂອບເຂດຂອງອົງກອນໂດຍຕົວມັນເອງ
    /// ຖ້າ Repository ຢ່າສືບຕໍ່ການກັ່ນຕອງໂດຍຄ່າທີ່ສົ່ງຄືນມາຈາກວິທີການນີ້ Owner ທ່ານຈະເຫັນຂໍ້ມູນຈາກທຸກໆອົງກອນໃນລະບົບໂດຍບັງເອີນ
    /// ຟື້ນຟູ null ເທົ່ານັ້ນ SuperAdmin ເທົ່ານັ້ນ (ອົງກອນທີ່ບໍ່ຈໍາກັດສາມາດເບິ່ງລະບົບທັງຝົດ)
    /// </remarks>
    /// <param name="currentUser">ຜູ້ໃຊ້ປະຈຸບັນ</param>
    public static Guid? GetOrganizationRestrictionForListing(ICurrentUser currentUser)
    {
        ArgumentNullException.ThrowIfNull(currentUser);

        return currentUser.IsSuperAdmin ? null : currentUser.OrganizationId;
    }
}
