namespace DormFlow.Contracts.Users;

/// <summary>ຮ້ອງຂໍແກ້ໄຂຂໍ້ມູນໂປຣໄຟລ໌ຜູ້ໃຊ້</summary>
public class UpdateUserRequest
{
    /// <summary>ຊື່</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>ນາມສະກຸນ</summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>ເບີໂທລະສັບ</summary>
    public string? Phone { get; set; }

    /// <summary>ທີ່ຢູ່ຮູບໂປຣໄຟລ໌</summary>
    public string? ProfileImage { get; set; }
}
