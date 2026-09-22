namespace DormFlow.Contracts.Authentication;

/// <summary>ຂໍ້ມູນຜູ້ໃຊ້ເຂົ້າສູ່ລະບົບໃນປັດຈຸບັນ</summary>
public class CurrentUserResponse
{
    /// <summary>ID ຜູ້ໃຊ້</summary>
    public Guid Id { get; init; }

    /// <summary>ອີເມວ</summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>ຊື່</summary>
    public string FirstName { get; init; } = string.Empty;

    /// <summary>ນາມສະກຸນ</summary>
    public string LastName { get; init; } = string.Empty;

    /// <summary>ຊື່ເຕັມສໍາລັບການສະແດງ</summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>ເບີໂທລະສັບ</summary>
    public string? Phone { get; init; }

    /// <summary>ທີ່ຢູ່ຮູບໂປຣໄຟລ໌</summary>
    public string? ProfileImage { get; init; }

    /// <summary>ອົງກອນທີ່ກ່ຽວຂ້ອງແມ່ນ null ສໍາລັບຜູ້ບໍລິຫານທີ່ບໍ່ຕິດກັບອົງກອນໃດໆ</summary>
    public Guid? OrganizationId { get; init; }

    /// <summary>ສະຖານະບັນຊີ</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>ເຂົ້າສູ່ລະບົບຄັ້ງສຸດທ້າຍ</summary>
    public DateTimeOffset? LastLoginAt { get; init; }

    /// <summary>ບົດບາດຂອງຜູ້ໃຊ້</summary>
    public IReadOnlyList<string> Roles { get; init; } = [];

    /// <summary>ລະຫັດສິດພິເສດທີ່ຜູ້ໃຊ້ມີ</summary>
    public IReadOnlyList<string> Permissions { get; init; } = [];

    /// <summary>ຫໍພັກທີ່ຜູ້ໃຊ້ຖືກມອບຝາຍ</summary>
    public IReadOnlyList<Guid> DormitoryIds { get; init; } = [];
}
