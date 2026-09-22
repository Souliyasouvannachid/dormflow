using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>
/// ບັນທຶກເຫດການສໍາຄັນທີ່ເກີດຂື້ນໃນລະບົບ
/// </summary>
/// <remarks>
/// ຕາຕະລາງນີ້ແມ່ນຂຽນເທົ່ານັ້ນ. ບໍ່ມີການແກ້ໄຂແລະບໍ່ມີ Soft Delete
/// ເພາະສະນັ້ນມັນບໍ່ໄດ້ຖືກສືບທອດມາຈາກ <see cref="BaseEntity"/> ເພື່ອຫຼີກເວັ້ນການມີຖັນທີ່ບໍ່ເຄີຍຖືກນໍາໃຊ້
/// ແລະໃນຄໍາສັ່ງທີ່ຈະບໍ່ Global Query Filter ທ່ານສະເຝີສາມາດກັ່ນຕອງຂໍ້ມູນທີ່ຕ້ອງໄດ້ຮັບການກວດສອບຄືນ
/// </remarks>
public class AuditLog : IEntity
{
    /// <inheritdoc />
    public Guid Id { get; init; } = Guid.CreateVersion7();

    /// <summary>ອົງກອນທີ່ກ່ຽວຂ້ອງແມ່ນ null ສໍາລັບເຫດການລະດັບລະບົບ</summary>
    public Guid? OrganizationId { get; init; }

    /// <summary>ຫໍພັກທີ່ກ່ຽວຂ້ອງ</summary>
    public Guid? DormitoryId { get; init; }

    /// <summary>ຜູ້ໃຊ້ທີ່ເຮັດໃຫ້ເກີດເຫດການແມ່ນ null ເມື່ອຕົວຕົນຂອງທ່ານຍັງບໍ່ທັນໄດ້ຮັບການຢັ້ງຢືນ</summary>
    public Guid? UserId { get; init; }

    /// <summary>ປະເພດເຫດການ</summary>
    public required AuditAction Action { get; init; }

    /// <summary>ຊື່ Entity ໄດ້ເຮັດແລ້ວ</summary>
    public required string EntityName { get; init; }

    /// <summary>ລະຫັດຂອງ Entity ໄດ້ເຮັດແລ້ວ</summary>
    public string? EntityId { get; init; }

    /// <summary>ຄ່າຕົ້ນສະບັບກ່ອນການແກ້ໄຂຈະຖືກເກັບໄວ້ເປັນ jsonb</summary>
    public string? OldValues { get; init; }

    /// <summary>ຄ່າໃຝ່ຫຼັງຈາກການດັດແກ້ຈະຖືກເກັບໄວ້ເປັນ jsonb</summary>
    public string? NewValues { get; init; }

    /// <summary>ເລກ IP ຕົ້ນກໍາເນີດ</summary>
    public string? IpAddress { get; init; }

    /// <summary>User Agent ຂອງຄໍາຮ້ອງສະຝັກ</summary>
    public string? UserAgent { get; init; }

    /// <summary>ເສັ້ນທາງຂອງການຮ້ອງຂໍທີ່ເຮັດໃຫ້ເກີດເຫດການ</summary>
    public string? RequestPath { get; init; }

    /// <summary>ເວລາຂອງເຫດການ</summary>
    public DateTimeOffset CreatedAt { get; init; }
}
