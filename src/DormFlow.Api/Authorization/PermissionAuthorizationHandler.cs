using DormFlow.Application.Common.Abstractions;
using Microsoft.AspNetCore.Authorization;

namespace DormFlow.Api.Authorization;

/// <summary>
/// ກວດສອບວ່າຜູ້ໃຊ້ມີສິດຕາມທີ່ລະບຸໄວ້ <see cref="PermissionRequirement"/> ລະບຸຫຼືບໍ່
/// </summary>
/// <remarks>
/// ອ່ານການອະນຸຍາດຈາກ Claim ຂອງ Access Token ທັງຝົດ, ບໍ່ມີໄຟຖານຂໍ້ມູນເພີ່ມເຕີມ
/// SuperAdmin ສະເຝີຜ່ານຄວາມຕ້ອງການທັງຝົດໂດຍຜ່ານ logic in <see cref="ICurrentUser.HasPermission"/>
/// </remarks>
public sealed class PermissionAuthorizationHandler(ICurrentUser currentUser)
    : AuthorizationHandler<PermissionRequirement>
{
    private readonly ICurrentUser _currentUser = currentUser;

    /// <inheritdoc />
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(requirement);

        if (_currentUser.HasPermission(requirement.PermissionCode))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
