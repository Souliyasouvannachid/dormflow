namespace DormFlow.Application.Common.Abstractions;

/// <summary>
/// ຊື່ Claim ທີ່ DormFlow ເອົາໃສ່ໃນ Access Token
/// ໃຊ້ຊຸດດຽວກັນທັງໃນເວລາທີ່ອອກ tokens ແລະໃນເວລາທີ່ອ່ານຂໍ້ມູນຜູ້ໃຊ້ໃນປະຈຸບັນ
/// </summary>
public static class DormFlowClaimTypes
{
    /// <summary>ID ຜູ້ໃຊ້</summary>
    public const string UserId = "sub";

    /// <summary>ອົງກອນທີ່ຜູ້ໃຊ້ເປັນເຈົ້າຂອງ</summary>
    public const string OrganizationId = "org_id";

    /// <summary>ຫໍພັກທີ່ຜູ້ໃຊ້ຖືກມອບໝາຍສາມາດປາກົດຫຼາຍຄັ້ງໃນໂທເຄັນດຽວ</summary>
    public const string DormitoryId = "dorm_id";

    /// <summary>ລະຫັດການອະນຸຍາດສາມາດປາກົດຫຼາຍຄັ້ງໃນ token ດຽວ</summary>
    public const string Permission = "perm";

    /// <summary>ຊື່ຜູ້ໃຊ້</summary>
    public const string FirstName = "given_name";

    /// <summary>ນາມສະກຸນຜູ້ໃຊ້</summary>
    public const string LastName = "family_name";

    /// <summary>ລະຫັດອ້າງອີງຂອງ Access Token ໃຊ້ເພື່ອຜູກມັດກັບ Refresh Token</summary>
    public const string JwtId = "jti";
}
