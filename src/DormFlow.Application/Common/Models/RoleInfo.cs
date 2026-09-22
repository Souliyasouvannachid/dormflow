namespace DormFlow.Application.Common.Models;

/// <summary>
/// ຂໍ້ມູນຜຶ່ງບົດບາດແມ່ນໃຊ້ແທນ Entity ຂອງ ASP.NET Core Identity ໃນຫ້ອງຮຽນ Application
/// </summary>
/// <param name="Id">ລະຫັດການສົ່ງຕໍ່ບົດບາດ</param>
/// <param name="Name">ຊື່ບົດບາດ</param>
/// <param name="Description">ລາຍລະອຽດບົດບາດ</param>
/// <param name="IsSystemRole">ນີ້ແມ່ນບົດບາດມາດຕະຖານສໍາລັບລະບົບບໍ?</param>
/// <param name="Permissions">ລະຫັດການອະນຸຍາດທັງໝົດທີ່ບົດບາດນີ້ມີ</param>
public sealed record RoleInfo(
    Guid Id,
    string Name,
    string? Description,
    bool IsSystemRole,
    IReadOnlyList<string> Permissions);
