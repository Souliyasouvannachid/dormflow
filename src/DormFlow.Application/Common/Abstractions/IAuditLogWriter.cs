using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Common.Abstractions;

/// <summary>ບັນທຶກເຫດການ Audit Log</summary>
public interface IAuditLogWriter
{
    /// <summary>ບັນທຶກເຫດການທີ່ກຽມໄວ້</summary>
    /// <param name="auditLog">ເຫດການທີ່ທ່ານຕ້ອງການທີ່ຈະບັນທຶກ</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    Task WriteAsync(AuditLog auditLog, CancellationToken cancellationToken = default);

    /// <summary>
    /// ການບັນທຶກເຫດການອັດຕະໂນມັດເພີ່ມບໍລິບົດຂອງຄໍາຮ້ອງຂໍໃນປະຈຸບັນ
    /// ລວມທັງຜູ້ໃຊ້, ອົງກອນ, ຈໍານວນ IP, User Agent, ຮ້ອງຂໍເສັ້ນທາງແລະເວລາ
    /// </summary>
    /// <param name="action">ປະເພດເຫດການ</param>
    /// <param name="entityName">ຊື່ Entity ໄດ້ເຮັດແລ້ວ</param>
    /// <param name="entityId">ລະຫັດຂອງ Entity ໄດ້ເຮັດແລ້ວ</param>
    /// <param name="userId">ຜູ້ໃຊ້ທີ່ເຮັດໃຫ້ເກີດເຫດການ ລະບຸໃນເວລາທີ່ບໍ່ມີຂໍ້ມູນໃນ Claim ຕົວຢ່າງ, ເມື່ອເຂົ້າສູ່ລະບົບ</param>
    /// <param name="organizationId">ອົງກອນທີ່ກ່ຽວຂ້ອງລະບຸໃນເວລາທີ່ບໍ່ມີຂໍ້ມູນໃນ Claim</param>
    /// <param name="oldValues">ຄ່າຕົ້ນສະບັບກ່ອນທີ່ຈະແກ້ໄຂໃນຮູບແບບ JSON</param>
    /// <param name="newValues">ຄ່າໃຝ່ຫຼັງຈາກການແກ້ໄຂໃນຮູບແບບ JSON</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    Task WriteAsync(
        AuditAction action,
        string entityName,
        string? entityId = null,
        Guid? userId = null,
        Guid? organizationId = null,
        string? oldValues = null,
        string? newValues = null,
        CancellationToken cancellationToken = default);
}
