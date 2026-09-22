using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Payments;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Persistence.Repositories;

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນການຈ່າຍເງິນໃນ PostgreSQL ຜ່ານ EF Core</summary>
public sealed class PaymentRepository(DormFlowDbContext dbContext) : IPaymentRepository
{
    private readonly DormFlowDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _dbContext.Payments.FirstOrDefaultAsync(payment => payment.Id == id, cancellationToken);

    /// <inheritdoc />
    public Task<PagedResult<Payment>> GetPagedAsync(
        PaymentListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        IQueryable<Payment> payments = _dbContext.Payments.AsNoTracking();

        if (filter.OrganizationId.HasValue)
        {
            payments = payments.Where(payment => payment.OrganizationId == filter.OrganizationId.Value);
        }

        if (filter.DormitoryId.HasValue)
        {
            payments = payments.Where(payment => payment.DormitoryId == filter.DormitoryId.Value);
        }

        if (filter.InvoiceId.HasValue)
        {
            payments = payments.Where(payment => payment.InvoiceId == filter.InvoiceId.Value);
        }

        if (filter.Status.HasValue)
        {
            payments = payments.Where(payment => payment.Status == filter.Status.Value);
        }

        if (filter.RestrictToDormitoryIds is not null)
        {
            payments = payments.Where(payment =>
                payment.DormitoryId != null && filter.RestrictToDormitoryIds.Contains(payment.DormitoryId.Value));
        }

        payments = ApplySort(payments, query);

        return payments.ToPagedResultAsync(query, cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> CountAsync(PaymentListFilter filter, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        IQueryable<Payment> payments = _dbContext.Payments.AsNoTracking();

        if (filter.OrganizationId.HasValue)
        {
            payments = payments.Where(payment => payment.OrganizationId == filter.OrganizationId.Value);
        }

        if (filter.DormitoryId.HasValue)
        {
            payments = payments.Where(payment => payment.DormitoryId == filter.DormitoryId.Value);
        }

        if (filter.InvoiceId.HasValue)
        {
            payments = payments.Where(payment => payment.InvoiceId == filter.InvoiceId.Value);
        }

        if (filter.Status.HasValue)
        {
            payments = payments.Where(payment => payment.Status == filter.Status.Value);
        }

        if (filter.RestrictToDormitoryIds is not null)
        {
            payments = payments.Where(payment =>
                payment.DormitoryId != null && filter.RestrictToDormitoryIds.Contains(payment.DormitoryId.Value));
        }

        return payments.CountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task AddAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(payment);

        _dbContext.Payments.Add(payment);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _dbContext.SaveChangesAsync(cancellationToken);

    private static IQueryable<Payment> ApplySort(IQueryable<Payment> payments, PagedQuery query)
        => query.SortBy?.ToLowerInvariant() switch
        {
            "paymentdate" => payments.OrderByDirection(payment => payment.PaymentDate, query.SortDirection),
            "amount" => payments.OrderByDirection(payment => payment.Amount, query.SortDirection),
            "status" => payments.OrderByDirection(payment => payment.Status, query.SortDirection),
            _ => payments.OrderByDirection(payment => payment.CreatedAt, query.SortDirection),
        };
}
