using DormFlow.Application.Common.Models;

namespace DormFlow.Application.Common.Abstractions;

/// <summary>ອອກ Access Token ຕົວແບບ JWT ກັບຜູ້ໃຊ້</summary>
public interface IAccessTokenGenerator
{
    /// <summary>ສ້າງ Access Token ຈາກຂໍ້ມູນຜູ້ໃຊ້</summary>
    /// <param name="profile">ຂໍ້ມູນຜູ້ໃຊ້ທີ່ຈະຖືກປ້ອນເຂົ້າ Claim</param>
    AccessToken Generate(UserSecurityProfile profile);
}
