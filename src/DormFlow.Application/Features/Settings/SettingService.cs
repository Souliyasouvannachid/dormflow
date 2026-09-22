using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Common.Exceptions;
using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Settings;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using FluentValidation;

namespace DormFlow.Application.Features.Settings;

/// <summary>Use Case ການຕັ້ງຄ່າໂມດູນທັງຝົດ</summary>
public sealed class SettingService(
    ISettingRepository repository,
    ICurrentUser currentUser,
    IAuditLogWriter auditLogWriter,
    IValidator<UpsertSettingRequest> upsertValidator) : ISettingService
{
    private const int MaxKeyLength = 100;
    private const string AuditEntityName = "Setting";
    private static readonly string[] AllowedSortFields = ["key", "createdAt", "updatedAt"];

    private readonly ISettingRepository _repository = repository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IAuditLogWriter _auditLogWriter = auditLogWriter;
    private readonly IValidator<UpsertSettingRequest> _upsertValidator = upsertValidator;

    /// <inheritdoc />
    public async Task<PagedResult<SettingResponse>> GetPagedAsync(
        SettingListRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        Guid organizationId = TenantScope.ResolveOrganizationIdForCreate(_currentUser, request.OrganizationId);
        PagedQuery query = PagedQueryNormalizer.Normalize(request, AllowedSortFields, defaultSortBy: "key");

        SettingListFilter filter = new(organizationId);

        PagedResult<Setting> result = await _repository.GetPagedAsync(filter, query, cancellationToken);

        return PagedResult.Create(
            (IReadOnlyList<SettingResponse>)[.. result.Items.Select(ToResponse)],
            result.Page,
            result.PageSize,
            result.TotalItems);
    }

    /// <inheritdoc />
    public async Task<SettingResponse> GetByKeyAsync(
        string key,
        Guid? organizationId,
        CancellationToken cancellationToken = default)
    {
        Guid resolvedOrganizationId = TenantScope.ResolveOrganizationIdForCreate(_currentUser, organizationId);
        string normalizedKey = RequireValidKey(key);

        Setting setting = await _repository.GetByKeyAsync(resolvedOrganizationId, normalizedKey, cancellationToken)
            ?? throw new NotFoundException("ບໍ່ພົບການຕັ້ງຄ່າທີ່ຕ້ອງການ");

        return ToResponse(setting);
    }

    /// <inheritdoc />
    public async Task<SettingResponse> UpsertAsync(
        string key,
        UpsertSettingRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _upsertValidator.ValidateAndThrowAsync(request, cancellationToken);

        Guid organizationId = TenantScope.ResolveOrganizationIdForCreate(_currentUser, request.OrganizationId);
        string normalizedKey = RequireValidKey(key);

        Setting? setting = await _repository.GetByKeyAsync(organizationId, normalizedKey, cancellationToken);
        bool isNew = setting is null;

        if (setting is null)
        {
            setting = new Setting
            {
                OrganizationId = organizationId,
                Key = normalizedKey,
                Value = request.Value.Trim(),
            };

            await _repository.AddAsync(setting, cancellationToken);
        }
        else
        {
            setting.Value = request.Value.Trim();
        }

        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            isNew ? AuditAction.Create : AuditAction.Update,
            AuditEntityName,
            setting.Id.ToString(),
            organizationId: setting.OrganizationId,
            cancellationToken: cancellationToken);

        return ToResponse(setting);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(string key, Guid? organizationId, CancellationToken cancellationToken = default)
    {
        Guid resolvedOrganizationId = TenantScope.ResolveOrganizationIdForCreate(_currentUser, organizationId);
        string normalizedKey = RequireValidKey(key);

        Setting setting = await _repository.GetByKeyAsync(resolvedOrganizationId, normalizedKey, cancellationToken)
            ?? throw new NotFoundException("ບໍ່ພົບການຕັ້ງຄ່າທີ່ຕ້ອງການ");

        _repository.Remove(setting);
        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Delete,
            AuditEntityName,
            setting.Id.ToString(),
            organizationId: setting.OrganizationId,
            cancellationToken: cancellationToken);
    }

    private static string RequireValidKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ValidationFailedException("ກະລຸນາລະບຸຄີຂອງການຕັ້ງຄ່າ");
        }

        string trimmed = key.Trim();
        if (trimmed.Length > MaxKeyLength)
        {
            throw new ValidationFailedException($"ຄີຂອງການຕັ້ງຄ່າຕ້ອງມີຄວາມຍາວບໍ່ເກີນ {MaxKeyLength} ໂຕອັກສອນ");
        }

        return trimmed;
    }

    private static SettingResponse ToResponse(Setting setting) => new()
    {
        Id = setting.Id,
        OrganizationId = setting.OrganizationId,
        Key = setting.Key,
        Value = setting.Value,
        CreatedAt = setting.CreatedAt,
        UpdatedAt = setting.UpdatedAt,
    };
}
