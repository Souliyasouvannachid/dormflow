using Microsoft.AspNetCore.Authorization;

namespace DormFlow.Api.Authorization;

/// <summary>ຂໍ້ກໍານົດທີ່ຜູ້ໃຊ້ຕ້ອງມີສິດຕາມ ID ທີ່ກໍານົດໄວ້</summary>
/// <param name="PermissionCode">ລະຫັດການອະນຸຍາດທີ່ຕ້ອງການ, ເຊັ່ນ: rooms.create</param>
public sealed record PermissionRequirement(string PermissionCode) : IAuthorizationRequirement;
