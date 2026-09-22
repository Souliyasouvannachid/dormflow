namespace DormFlow.Application.Common.Models;

/// <summary>
/// Refresh Token ອອກໃຫ້ຜູ້ໃຊ້ບໍ່ດົນມານີ້
/// </summary>
/// <remarks>
/// <paramref name="Value"/> ແມ່ນມູນຄ່າຕົວຈິງທີ່ຖືກສົ່ງໄປຫາຜູ້ໂທແລະປາກົດພຽງແຕ່ຄັ້ງດຽວ
/// ຖານຂໍ້ມູນເກັບຮັກສາພຽງແຕ່ຄ່າ hash. ດັ່ງນັ້ນຈຶ່ງບໍ່ມີທາງທີ່ຈະອ່ານຄ່ານີ້ກັບຄືນມາອີກ
/// </remarks>
/// <param name="Value">ຄ່າ token ທີ່ແທ້ຈິງ</param>
/// <param name="TokenId">ID ຂອງແຖວໃນຕາຕະລາງ refresh_tokens</param>
/// <param name="ExpiresAt">ເວລາໝົດອາຍຸ</param>
public sealed record IssuedRefreshToken(
    string Value,
    Guid TokenId,
    DateTimeOffset ExpiresAt);
