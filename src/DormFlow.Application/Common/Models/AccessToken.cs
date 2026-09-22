namespace DormFlow.Application.Common.Models;

/// <summary>Access Token ອອກໃຫ້ຜູ້ໃຊ້ທີ່ມີຂໍ້ມູນປະກອບ</summary>
/// <param name="Value">ຄ່າໂທເຄັນ JWT</param>
/// <param name="JwtId">ມູນຄ່າ jti ຂອງ tokens ໃຊ້ເພື່ອຜູກມັດກັບ Refresh Token ທີ່ອອກມາຮ່ວມກັນ</param>
/// <param name="ExpiresAt">ເວລາໝົດອາຍຸ</param>
/// <param name="ExpiresInSeconds">ຊີວິດທີ່ຍັງເຫຼືອຢູ່ໃນວິນາທີ</param>
public sealed record AccessToken(
    string Value,
    string JwtId,
    DateTimeOffset ExpiresAt,
    int ExpiresInSeconds);
