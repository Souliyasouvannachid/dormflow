using DormFlow.Application.Common.Models;
using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Users;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using DormFlow.Infrastructure.Persistence;
using DormFlow.Infrastructure.Persistence.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Identity;

/// <summary>เข้าถึงและจัดการบัญชีผู้ใช้บน ASP.NET Core Identity สำหรับหน้าจอดูแลผู้ใช้งาน</summary>
public sealed class UserAdministrationRepository(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    DormFlowDbContext dbContext) : IUserAdministrationRepository
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly DormFlowDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await _userManager.FindByEmailAsync(email) is not null;

    /// <inheritdoc />
    public async Task<UserSecurityProfile?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        ApplicationUser? user = await _dbContext.Users
            .FirstOrDefaultAsync(candidate => candidate.Id == userId, cancellationToken);

        return user is null ? null : await BuildProfileAsync(user, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PagedResult<UserSecurityProfile>> GetPagedAsync(
        UserListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        IQueryable<ApplicationUser> users = _dbContext.Users.AsNoTracking();

        if (filter.OrganizationId.HasValue)
        {
            users = users.Where(user => user.OrganizationId == filter.OrganizationId.Value);
        }

        if (filter.Status.HasValue)
        {
            users = users.Where(user => user.Status == filter.Status.Value);
        }

        if (filter.DormitoryId.HasValue)
        {
            users = users.Where(user => _dbContext.UserDormitories
                .Any(assignment => assignment.UserId == user.Id && assignment.DormitoryId == filter.DormitoryId.Value));
        }

        if (filter.RestrictToDormitoryIds is not null)
        {
            IReadOnlyCollection<Guid> allowedDormitoryIds = filter.RestrictToDormitoryIds;
            users = users.Where(user => _dbContext.UserDormitories
                .Any(assignment => assignment.UserId == user.Id && allowedDormitoryIds.Contains(assignment.DormitoryId)));
        }

        if (query.HasSearch)
        {
            users = users.Where(user =>
                EF.Functions.ILike(user.FirstName, $"%{query.Search}%")
                || EF.Functions.ILike(user.LastName, $"%{query.Search}%")
                || (user.Email != null && EF.Functions.ILike(user.Email, $"%{query.Search}%")));
        }

        users = ApplySort(users, query);

        long totalItems = await users.LongCountAsync(cancellationToken);

        if (totalItems == 0)
        {
            return PagedResult.Empty<UserSecurityProfile>(query.Page, query.PageSize);
        }

        List<ApplicationUser> pageOfUsers = await users.ApplyPaging(query).ToListAsync(cancellationToken);

        List<UserSecurityProfile> profiles = [];
        foreach (ApplicationUser user in pageOfUsers)
        {
            profiles.Add(await BuildProfileAsync(user, cancellationToken));
        }

        return PagedResult.Create((IReadOnlyList<UserSecurityProfile>)profiles, query.Page, query.PageSize, totalItems);
    }

    /// <inheritdoc />
    public async Task<UserCreationResult> CreateAsync(
        Guid? organizationId,
        string email,
        string password,
        string firstName,
        string lastName,
        string? phone,
        CancellationToken cancellationToken = default)
    {
        ApplicationUser user = new()
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phone,
            OrganizationId = organizationId,
            Status = UserStatus.Active,
        };

        IdentityResult result = await _userManager.CreateAsync(user, password);

        return result.Succeeded
            ? UserCreationResult.Success(user.Id)
            : UserCreationResult.Failed([.. result.Errors.Select(error => error.Description)]);
    }

    /// <inheritdoc />
    public async Task<bool> UpdateProfileAsync(
        Guid userId,
        string firstName,
        string lastName,
        string? phone,
        string? profileImage,
        CancellationToken cancellationToken = default)
    {
        int affected = await _dbContext.Users
            .Where(user => user.Id == userId)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(user => user.FirstName, firstName)
                    .SetProperty(user => user.LastName, lastName)
                    .SetProperty(user => user.PhoneNumber, phone)
                    .SetProperty(user => user.ProfileImage, profileImage),
                cancellationToken);

        return affected > 0;
    }

    /// <inheritdoc />
    public async Task<bool> ChangeStatusAsync(
        Guid userId,
        UserStatus status,
        CancellationToken cancellationToken = default)
    {
        int affected = await _dbContext.Users
            .Where(user => user.Id == userId)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(user => user.Status, status),
                cancellationToken);

        return affected > 0;
    }

    /// <inheritdoc />
    public async Task<bool> SetRolesAsync(
        Guid userId,
        IReadOnlyList<string> roleNames,
        CancellationToken cancellationToken = default)
    {
        ApplicationUser? user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return false;
        }

        foreach (string roleName in roleNames)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                return false;
            }
        }

        IList<string> currentRoles = await _userManager.GetRolesAsync(user);

        IdentityResult removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded)
        {
            return false;
        }

        IdentityResult addResult = await _userManager.AddToRolesAsync(user, roleNames);
        return addResult.Succeeded;
    }

    /// <inheritdoc />
    public async Task<bool> AssignDormitoryAsync(
        Guid userId,
        Guid dormitoryId,
        bool isPrimary,
        Guid? assignedBy,
        DateTimeOffset assignedAt,
        CancellationToken cancellationToken = default)
    {
        bool alreadyAssigned = await _dbContext.UserDormitories.AnyAsync(
            assignment => assignment.UserId == userId && assignment.DormitoryId == dormitoryId,
            cancellationToken);

        if (alreadyAssigned)
        {
            return false;
        }

        if (isPrimary)
        {
            // มีหอพักหลักได้เพียงแห่งเดียวต่อผู้ใช้หนึ่งคน ยกเลิกหอพักหลักเดิมก่อนตั้งใหม่
            await _dbContext.UserDormitories
                .Where(assignment => assignment.UserId == userId && assignment.IsPrimary)
                .ExecuteUpdateAsync(
                    setters => setters.SetProperty(assignment => assignment.IsPrimary, false),
                    cancellationToken);
        }

        _dbContext.UserDormitories.Add(new UserDormitory
        {
            UserId = userId,
            DormitoryId = dormitoryId,
            IsPrimary = isPrimary,
            AssignedAt = assignedAt,
            AssignedBy = assignedBy,
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> RemoveDormitoryAsync(
        Guid userId,
        Guid dormitoryId,
        CancellationToken cancellationToken = default)
    {
        int affected = await _dbContext.UserDormitories
            .Where(assignment => assignment.UserId == userId && assignment.DormitoryId == dormitoryId)
            .ExecuteDeleteAsync(cancellationToken);

        return affected > 0;
    }

    private async Task<UserSecurityProfile> BuildProfileAsync(
        ApplicationUser user,
        CancellationToken cancellationToken)
    {
        IList<string> roles = await _userManager.GetRolesAsync(user);

        UserDormitoryAssignmentInfo[] assignments = await _dbContext.UserDormitories
            .Where(assignment => assignment.UserId == user.Id)
            .Select(assignment => new UserDormitoryAssignmentInfo(
                assignment.DormitoryId,
                assignment.IsPrimary,
                assignment.AssignedAt))
            .ToArrayAsync(cancellationToken);

        return new UserSecurityProfile
        {
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Phone = user.PhoneNumber,
            ProfileImage = user.ProfileImage,
            OrganizationId = user.OrganizationId,
            Status = user.Status,
            LastLoginAt = user.LastLoginAt,
            CreatedAt = user.CreatedAt,
            Roles = [.. roles],
            DormitoryIds = [.. assignments.Select(assignment => assignment.DormitoryId)],
            DormitoryAssignments = assignments,
        };
    }

    private static IQueryable<ApplicationUser> ApplySort(IQueryable<ApplicationUser> users, PagedQuery query)
        => query.SortBy?.ToLowerInvariant() switch
        {
            "email" => users.OrderByDirection(user => user.Email, query.SortDirection),
            "lastname" => users.OrderByDirection(user => user.LastName, query.SortDirection),
            "status" => users.OrderByDirection(user => user.Status, query.SortDirection),
            "createdat" => users.OrderByDirection(user => user.CreatedAt, query.SortDirection),
            _ => users.OrderByDirection(user => user.FirstName, query.SortDirection),
        };
}
