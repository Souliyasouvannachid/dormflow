using DormFlow.Application.Common.Abstractions;
using Microsoft.AspNetCore.Authorization;

namespace DormFlow.Api.Authorization;

/// <summary>
/// ตรวจว่าผู้ใช้มีสิทธิ์ตามที่ <see cref="PermissionRequirement"/> ระบุหรือไม่
/// </summary>
/// <remarks>
/// อ่านสิทธิ์จาก Claim ของ Access Token ทั้งหมด ไม่ยิงฐานข้อมูลเพิ่ม
/// SuperAdmin ผ่านทุกข้อกำหนดเสมอผ่านตรรกะใน <see cref="ICurrentUser.HasPermission"/>
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
