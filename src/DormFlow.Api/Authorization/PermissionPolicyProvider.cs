using DormFlow.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace DormFlow.Api.Authorization;

/// <summary>
/// ສ້າງ Authorization Policy ກັບລະຫັດການອະນຸຍາດອັດຕະໂນມັດ
/// </summary>
/// <remarks>
/// ເຮັດໃຫ້ມັນບໍ່ຈໍາເປັນຕ້ອງລົງທະບຽນ Policy ລ່ວງໜ້າການອະນຸຍາດຄັ້ງດຽວໃນໂປຣແກຣມເລີ່ມຕົ້ນ
/// ເມື່ອເພີ່ມການອະນຸຍາດໃຝ່ໃນ PermissionCodes ຂ້ອຍໃຊ້ມັນ [RequirePermission("...")] ທັນທີ
/// </remarks>
public sealed class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallbackProvider;

    /// <summary>ສ້າງ Provider</summary>
    /// <param name="options">ການຕັ້ງຄ່າ Authorization</param>
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
