using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Common.Exceptions;
using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Dormitories;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Expenses;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using FluentValidation;

namespace DormFlow.Application.Features.Expenses;

/// <summary>Use Case ทั้งหมดของโมดูลค่าใช้จ่าย</summary>
public sealed class ExpenseService(
    IExpenseRepository repository,
    IDormitoryRepository dormitoryRepository,
    ICurrentUser currentUser,
    IAuditLogWriter auditLogWriter,
    IValidator<CreateExpenseRequest> createValidator) : IExpenseService
{
    private const string AuditEntityName = "Expense";
    private static readonly string[] AllowedSortFields = ["amount", "expenseDate", "category", "createdAt"];

    private readonly IExpenseRepository _repository = repository;
    private readonly IDormitoryRepository _dormitoryRepository = dormitoryRepository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IAuditLogWriter _auditLogWriter = auditLogWriter;
    private readonly IValidator<CreateExpenseRequest> _createValidator = createValidator;

    /// <inheritdoc />
    public async Task<PagedResult<ExpenseResponse>> GetPagedAsync(
        ExpenseListRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        PagedQuery query = PagedQueryNormalizer.Normalize(request, AllowedSortFields, defaultSortBy: "expenseDate");

        ExpenseListFilter filter = new(
            TenantScope.GetOrganizationRestrictionForListing(_currentUser),
            request.DormitoryId,
            ParseCategoryOrNull(request.Category),
            request.ExpenseDateFrom,
            request.ExpenseDateTo,
            TenantScope.GetDormitoryRestrictionForListing(_currentUser));

        PagedResult<Expense> result = await _repository.GetPagedAsync(filter, query, cancellationToken);

        return PagedResult.Create(
            (IReadOnlyList<ExpenseResponse>)[.. result.Items.Select(ToResponse)],
            result.Page,
            result.PageSize,
            result.TotalItems);
    }

    /// <inheritdoc />
    public async Task<ExpenseResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => ToResponse(await FindAccessibleAsync(id, cancellationToken));

    /// <inheritdoc />
    public async Task<ExpenseResponse> CreateAsync(
        CreateExpenseRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        Dormitory dormitory = await _dormitoryRepository.GetByIdAsync(request.DormitoryId, cancellationToken)
            ?? throw new NotFoundException("ไม่พบหอพักที่ระบุ");

        TenantScope.EnsureDormitoryAccess(_currentUser, dormitory.OrganizationId, dormitory.Id, "ไม่พบหอพักที่ระบุ");

        Expense expense = new()
        {
            OrganizationId = dormitory.OrganizationId,
            DormitoryId = dormitory.Id,
            Category = Enum.Parse<ExpenseCategory>(request.Category, ignoreCase: true),
            Amount = request.Amount,
            ExpenseDate = request.ExpenseDate,
            Description = request.Description.Trim(),
            PaidTo = request.PaidTo,
            Notes = request.Notes,
        };

        await _repository.AddAsync(expense, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Create,
            AuditEntityName,
            expense.Id.ToString(),
            organizationId: expense.OrganizationId,
            cancellationToken: cancellationToken);

        return ToResponse(expense);
    }

    private async Task<Expense> FindAccessibleAsync(Guid id, CancellationToken cancellationToken)
    {
        Expense expense = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("ไม่พบข้อมูลค่าใช้จ่ายที่ต้องการ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            expense.OrganizationId,
            expense.DormitoryId.GetValueOrDefault(),
            "ไม่พบข้อมูลค่าใช้จ่ายที่ต้องการ");

        return expense;
    }

    private static ExpenseCategory? ParseCategoryOrNull(string? category)
        => !string.IsNullOrWhiteSpace(category) && Enum.TryParse(category, ignoreCase: true, out ExpenseCategory parsed)
            ? parsed
            : null;

    private static ExpenseResponse ToResponse(Expense expense) => new()
    {
        Id = expense.Id,
        OrganizationId = expense.OrganizationId,
        DormitoryId = expense.DormitoryId.GetValueOrDefault(),
        Category = expense.Category.ToString(),
        Amount = expense.Amount,
        ExpenseDate = expense.ExpenseDate,
        Description = expense.Description,
        PaidTo = expense.PaidTo,
        Notes = expense.Notes,
        CreatedAt = expense.CreatedAt,
    };
}
