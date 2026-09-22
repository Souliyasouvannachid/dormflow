using DormFlow.Domain.Entities;

namespace DormFlow.Application.Features.Permissions;

/// <summary>ເຂົ້າເຖິງບັນຊີລາຍຊື່ເຕັມຂອງການອະນຸຍາດລະບົບໃນຖານຂໍ້ມູນ</summary>
public interface IPermissionRepository
{
    /// <summary>ດຶງເອົາບັນຊີລາຍຊື່ຂອງການອະນຸຍາດທັງຝົດຈັດຮຽງຕາມໂມດູນແລະຫຼັງຈາກນັ້ນໂດຍລະຫັດໃບອະນຸຍາດ</summary>
    Task<IReadOnlyList<Permission>> GetAllAsync(CancellationToken cancellationToken = default);
}
