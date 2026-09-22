namespace DormFlow.Contracts.Users;

/// <summary>ນຶ່ງບັນຊີຜູ້ໃຊ້</summary>
public class UserResponse
{
    /// <summary>ID ຜູ້ໃຊ້</summary>
    public Guid Id { get; init; }

    /// <summary>ອີເມວ</summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>ຊື່</summary>
    public string FirstName { get; init; } = string.Empty;

    /// <summary>ນາມສະກຸນ</summary>
    public string LastName { get; init; } = string.Empty;

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

    /// <summary>ຫໍພັກທີ່ຜູ້ໃຊ້ຖືກມອບຝາຍ</summary>
    public IReadOnlyList<UserDormitoryAssignmentResponse> Dormitories { get; init; } = [];

    /// <summary>ເວລາສ້າງບັນຊີ</summary>
    public DateTimeOffset CreatedAt { get; init; }
}

/// <summary>ການກໍານົດຫໍພັກຜຶ່ງໃຫ້ກັບຜູ້ໃຊ້</summary>
public class UserDormitoryAssignmentResponse
{
    /// <summary>ຫໍພັກທີ່ຖືກມອບໝາຍ</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>ນີ້ແມ່ນຫໍພັກຫຼັກຂອງຜູ້ໃຊ້ບໍ?</summary>
    public bool IsPrimary { get; init; }

    /// <summary>ເວລາມອບໝາຍ</summary>
    public DateTimeOffset AssignedAt { get; init; }
}
