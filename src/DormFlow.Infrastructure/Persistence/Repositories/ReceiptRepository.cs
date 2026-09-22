using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Receipts;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Persistence.Repositories;

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນໃບຮັບເງິນຢູ່ PostgreSQL ຜ່ານ EF Core</summary>
public sealed class ReceiptRepository(DormFlowDbContext dbContext) : IReceiptRepository
{
    private readonly DormFlowDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public Task<Receipt?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _dbContext.Receipts.FirstOrDefaultAsync(receipt => receipt.Id == id, cancellationToken);

    /// <inheritdoc />
    public Task<bool> ExistsByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default)
        => _dbContext.Receipts.AnyAsync(receipt => receipt.PaymentId == paymentId, cancellationToken);

    /// <inheritdoc />
    public Task<bool> ExistsByReceiptNumberAsync(
        Guid organizationId,
        string receiptNumber,
        CancellationToken cancellationToken = default)
        => _dbContext.Receipts.AnyAsync(
            receipt => receipt.OrganizationId == organizationId && receipt.ReceiptNumber == receiptNumber,
            cancellationToken);

    /// <inheritdoc />
    public Task<PagedResult<Receipt>> GetPagedAsync(
        ReceiptListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        IQueryable<Receipt> receipts = _dbContext.Receipts.AsNoTracking();

        if (filter.OrganizationId.HasValue)
        {
            receipts = receipts.Where(receipt => receipt.OrganizationId == filter.OrganizationId.Value);
        }

        if (filter.DormitoryId.HasValue)
        {
            receipts = receipts.Where(receipt => receipt.DormitoryId == filter.DormitoryId.Value);
        }

        if (filter.PaymentId.HasValue)
        {
            receipts = receipts.Where(receipt => receipt.PaymentId == filter.PaymentId.Value);
        }

        if (filter.RestrictToDormitoryIds is not null)
        {
            receipts = receipts.Where(receipt =>
                receipt.DormitoryId != null && filter.RestrictToDormitoryIds.Contains(receipt.DormitoryId.Value));
        }

        if (query.HasSearch)
        {
            receipts = receipts.Where(receipt => EF.Functions.ILike(receipt.ReceiptNumber, $"%{query.Search}%"));
        }

        receipts = ApplySort(receipts, query);

        return receipts.ToPagedResultAsync(query, cancellationToken);
    }

    /// <inheritdoc />
    public Task AddAsync(Receipt receipt, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(receipt);

        _dbContext.Receipts.Add(receipt);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _dbContext.SaveChangesAsync(cancellationToken);

    private static IQueryable<Receipt> ApplySort(IQueryable<Receipt> receipts, PagedQuery query)
        => query.SortBy?.ToLowerInvariant() switch
        {
            "receiptnumber" => receipts.OrderByDirection(receipt => receipt.ReceiptNumber, query.SortDirection),
            "issuedat" => receipts.OrderByDirection(receipt => receipt.IssuedAt, query.SortDirection),
            _ => receipts.OrderByDirection(receipt => receipt.CreatedAt, query.SortDirection),
        };
}
