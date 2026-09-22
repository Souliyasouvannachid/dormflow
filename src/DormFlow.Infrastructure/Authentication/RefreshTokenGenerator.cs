using System.Security.Cryptography;

namespace DormFlow.Infrastructure.Authentication;

/// <summary>
/// ສ້າງແລະ hash ມູນຄ່າຂອງ Refresh Token
/// </summary>
/// <remarks>
/// ມູນຄ່າຕົວຈິງຂອງ token ຖືກສົ່ງໄປຫາຜູ້ໂທຄັ້ງດຽວເທົ່ານັ້ນ. ຖານຂໍ້ມູນເກັບຮັກສາພຽງແຕ່ຄ່າ hash
/// ການກວດສອບແມ່ນເຮັດໄດ້ໂດຍການ hashing ຄ່າທີ່ສົ່ງໂດຍຜູ້ໂທແລະຊອກຫາຄ່າ hash ນັ້ນ
/// ຖ້າຖານຂໍ້ມູນຖືກຮົ່ວ, ຜູ້ທີ່ໄດ້ຮັບຂໍ້ມູນຈະແລກປ່ຽນມູນຄ່າ hash Access Token ບໍ່ສາມາດສືບຕໍ່ໄດ້
/// </remarks>
public static class RefreshTokenGenerator
{
    /// <summary>ຈຳນວນ bytes ແບບສຸ່ມທີ່ໃຊ້ເພື່ອສ້າງ token</summary>
    private const int TokenByteLength = 64;

    /// <summary>ສ້າງຄ່າ token ໃໝ່ແບບສຸ່ມດ້ວຍເຄື່ອງສ້າງຕົວເລກແບບສຸ່ມ cryptographic</summary>
    public static string CreateToken()
        => Base64UrlEncode(RandomNumberGenerator.GetBytes(TokenByteLength));

    /// <summary>ຄິດໄລ່ຄ່າ hash ຂອງ token ໃນຮູບແບບເລກຖານສິບຫົກຕົວນ້ອຍ</summary>
    /// <param name="token">ຄ່າ token ທີ່ແທ້ຈິງ</param>
    /// <exception cref="ArgumentException">ເມື່ອຄ່າ token ຫວ່າງເປົ່າ</exception>
    public static string ComputeHash(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        byte[] hash = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(token));
        return Convert.ToHexStringLower(hash);
    }

    /// <summary>
    /// ການເຂົ້າລະຫັດ byte-to-text ສາມາດນຳໃຊ້ໄດ້ທັງໃນ URL ແລະໃນ JSON
    /// ຕັດຕົວອັກສອນທີ່ຕ້ອງໄດ້ຮັບການ maneuvered ໃນ URL ທັງໝົດ
    /// </summary>
    private static string Base64UrlEncode(byte[] bytes)
        => Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
}
