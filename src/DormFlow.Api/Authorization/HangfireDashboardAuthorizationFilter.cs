using DormFlow.Application.Common.Abstractions;
using Hangfire.Dashboard;

namespace DormFlow.Api.Authorization;

/// <summary>ຈຳກັດ Hangfire dashboard ໃຫ້ສະເພາະ SuperAdmin ທີ່ login ແລ້ວ</summary>
public sealed class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    /// <inheritdoc />
    public bool Authorize(DashboardContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        HttpContext httpContext = context.GetHttpContext();
        return httpContext.User.Identity?.IsAuthenticated == true
            && httpContext.User.IsInRole(DormFlowRoles.SuperAdmin);
    }
}
