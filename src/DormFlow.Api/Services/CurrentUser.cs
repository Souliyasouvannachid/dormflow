using System.Globalization;
using System.Security.Claims;
using DormFlow.Application.Common.Abstractions;

namespace DormFlow.Api.Services;

/// <summary>
/// ອ່ານຂໍ້ມູນຜູ້ໃຊ້ປະຈຸບັນຈາກ Claim ຂອງການຮ້ອງຂໍທີ່ກໍາລັງດໍາເນີນການໃນປັດຈຸບັນ
/// </summary>
/// <remarks>
/// ຄ່າທັງຝົດມາຈາກ Access Token ສະເພາະຜູ້ທີ່ຜ່ານການຢັ້ງຢືນລາຍເຊັນເທົ່ານັ້ນ
/// ດັ່ງນັ້ນ, ພວກເຮົາບໍ່ຍອມຮັບຄ່າທໍານຽມຂອງອົງກອນຫຼືຫໍພັກຈາກ Header ຫຼື Body ຢ່າງແທ້ຈິງຂອງຜູ້ໂທ
/// </remarks>
public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

    /// <inheritdoc />
    public Guid? UserId => ParseGuid(FindFirstValue(DormFlowClaimTypes.UserId) ?? FindFirstValue(ClaimTypes.NameIdentifier));

    /// <inheritdoc />
    public Guid? OrganizationId => ParseGuid(FindFirstValue(DormFlowClaimTypes.OrganizationId));

    /// <inheritdoc />
    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated == true;

    /// <inheritdoc />
    public bool IsSuperAdmin => Principal?.IsInRole(DormFlowRoles.SuperAdmin) == true;

    /// <inheritdoc />
    public IReadOnlyCollection<string> Roles =>
        Principal?.FindAll(ClaimTypes.Role)
            .Select(claim => claim.Value)
            .ToArray() ?? [];

    /// <inheritdoc />
    public IReadOnlyCollection<Guid> DormitoryIds =>
        Principal?.FindAll(DormFlowClaimTypes.DormitoryId)
            .Select(claim => ParseGuid(claim.Value))
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .ToArray() ?? [];

    /// <inheritdoc />
    public IReadOnlyCollection<string> Permissions =>
        Principal?.FindAll(DormFlowClaimTypes.Permission)
            .Select(claim => claim.Value)
            .ToArray() ?? [];

    /// <inheritdoc />
    public string? IpAddress
    {
        get
        {
            HttpContext? context = _httpContextAccessor.HttpContext;
            return context?.Connection.RemoteIpAddress?.ToString();
        }
    }

    /// <inheritdoc />
    public string? UserAgent
    {
        get
        {
            HttpContext? context = _httpContextAccessor.HttpContext;
            string? userAgent = context?.Request.Headers.UserAgent.ToString();
            return string.IsNullOrWhiteSpace(userAgent) ? null : userAgent;
        }
    }

    /// <inheritdoc />
    public string? RequestPath
    {
        get
        {
            HttpContext? context = _httpContextAccessor.HttpContext;
            return context?.Request.Path.Value;
        }
    }

    /// <inheritdoc />
    public bool HasPermission(string permissionCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(permissionCode);

        if (!IsAuthenticated)
        {
            return false;
        }

        // SuperAdmin ຖືວ່າມີສິດທຸກຢ່າງ ສະນັ້ນເຈົ້າບໍ່ຕ້ອງໃສ່ມັນ Permission Claim ທັງຝົດເຂົ້າໄປໃນ tokens
        return IsSuperAdmin
            || Principal?.HasClaim(DormFlowClaimTypes.Permission, permissionCode) == true;
    }

    /// <inheritdoc />
    public bool CanAccessDormitory(Guid dormitoryId)
    {
        if (!IsAuthenticated)
        {
            return false;
        }

        if (IsSuperAdmin || Principal?.IsInRole(DormFlowRoles.Owner) == true)
        {
            return true;
        }

        return DormitoryIds.Contains(dormitoryId);
    }

    private string? FindFirstValue(string claimType) => Principal?.FindFirstValue(claimType);

    private static Guid? ParseGuid(string? value)
        => Guid.TryParse(value, CultureInfo.InvariantCulture, out Guid parsed) ? parsed : null;
}
