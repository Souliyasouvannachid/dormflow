using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Expenses;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Persistence.Repositories;

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນຄ່າໃຊ້ຈ່າຍໃນ PostgreSQL ຜ່ານ EF Core</summary>
public sealed class ExpenseRepository(DormFlowDbContext dbContext) : IExpenseRepository
{
    private readonly DormFlowDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public Task<Expense?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _dbContext.Expenses.FirstOrDefaultAsync(expense => expense.Id == id, cancellationToken);

    /// <inheritdoc />
    public Task<PagedResult<Expense>> GetPagedAsync(
        ExpenseListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        IQueryable<Expense> expenses = _dbContext.Expenses.AsNoTracking();

        if (filter.OrganizationId.HasValue)
        {
            expenses = expenses.Where(expense => expense.OrganizationId == filter.OrganizationId.Value);
        }

        if (filter.DormitoryId.HasValue)
        {
            expenses = expenses.Where(expense => expense.DormitoryId == filter.DormitoryId.Value);
        }

        if (filter.Category.HasValue)
        {
            expenses = expenses.Where(expense => expense.Category == filter.Category.Value);
        }

        if (filter.ExpenseDateFrom.HasValue)
        {
            expenses = expenses.Where(expense => expense.ExpenseDate >= filter.ExpenseDateFrom.Value);
        }

        if (filter.ExpenseDateTo.HasValue)
        {
            expenses = expenses.Where(expense => expense.ExpenseDate <= filter.ExpenseDateTo.Value);
        }

        if (filter.RestrictToDormitoryIds is not null)
        {
            expenses = expenses.Where(expense =>
                expense.DormitoryId != null && filter.RestrictToDormitoryIds.Contains(expense.DormitoryId.Value));
        }

        if (query.HasSearch)
        {
            expenses = expenses.Where(expense =>
                EF.Functions.ILike(expense.Description, $"%{query.Search}%")
                || (expense.PaidTo != null && EF.Functions.ILike(expense.PaidTo, $"%{query.Search}%")));
        }

        expenses = ApplySort(expenses, query);

        return expenses.ToPagedResultAsync(query, cancellationToken);
    }

    /// <inheritdoc />
    public Task AddAsync(Expense expense, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(expense);

        _dbContext.Expenses.Add(expense);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _dbContext.SaveChangesAsync(cancellationToken);

    private static IQueryable<Expense> ApplySort(IQueryable<Expense> expenses, PagedQuery query)
        => query.SortBy?.ToLowerInvariant() switch
        {
            "amount" => expenses.OrderByDirection(expense => expense.Amount, query.SortDirection),
            "category" => expenses.OrderByDirection(expense => expense.Category, query.SortDirection),
            "expensedate" => expenses.OrderByDirection(expense => expense.ExpenseDate, query.SortDirection),
            _ => expenses.OrderByDirection(expense => expense.CreatedAt, query.SortDirection),
        };
}
