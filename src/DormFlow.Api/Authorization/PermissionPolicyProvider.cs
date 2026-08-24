using DormFlow.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace DormFlow.Api.Authorization;

/// <summary>
/// สร้าง Authorization Policy ให้กับรหัสสิทธิ์แบบอัตโนมัติ
/// </summary>
/// <remarks>
/// ทำให้ไม่ต้องลงทะเบียน Policy ล่วงหน้าทีละสิทธิ์ในโปรแกรมเริ่มต้น
/// เมื่อเพิ่มสิทธิ์ใหม่ใน PermissionCodes ก็ใช้ [RequirePermission("...")] ได้ทันที
/// </remarks>
public sealed class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallbackProvider;

    /// <summary>สร้าง Provider</summary>
    /// <param name="options">ค่าตั้งของ Authorization</param>
    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        _fallbackProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    /// <inheritdoc />
    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallbackProvider.GetDefaultPolicyAsync();

    /// <inheritdoc />
    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallbackProvider.GetFallbackPolicyAsync();

    /// <inheritdoc />
    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(policyName);

        if (!policyName.StartsWith(RequirePermissionAttribute.PolicyPrefix, StringComparison.Ordinal))
        {
            return _fallbackProvider.GetPolicyAsync(policyName);
        }

        string permissionCode = policyName[RequirePermissionAttribute.PolicyPrefix.Length..];

        AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new PermissionRequirement(permissionCode))
            .Build();

        return Task.FromResult<AuthorizationPolicy?>(policy);
    }
}
