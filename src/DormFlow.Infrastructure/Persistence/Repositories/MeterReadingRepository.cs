using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.MeterReadings;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using DormFlow.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Persistence.Repositories;

/// <summary>เข้าถึงข้อมูลค่ามิเตอร์บน PostgreSQL ผ่าน EF Core</summary>
public sealed class MeterReadingRepository(DormFlowDbContext dbContext) : IMeterReadingRepository
{
    private readonly DormFlowDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public Task<MeterReading?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _dbContext.MeterReadings.FirstOrDefaultAsync(reading => reading.Id == id, cancellationToken);

    /// <inheritdoc />
    public Task<bool> ExistsForPeriodAsync(
        Guid roomId,
        MeterType meterType,
        DateOnly periodMonth,
        CancellationToken cancellationToken = default)
        => _dbContext.MeterReadings.AnyAsync(
            reading => reading.RoomId == roomId
                && reading.MeterType == meterType
                && reading.PeriodMonth == periodMonth,
            cancellationToken);

    /// <inheritdoc />
    public Task<MeterReading?> GetLatestBeforePeriodAsync(
        Guid roomId,
        MeterType meterType,
        DateOnly periodMonth,
        CancellationToken cancellationToken = default)
        => _dbContext.MeterReadings
            .Where(reading => reading.RoomId == roomId
                && reading.MeterType == meterType
                && reading.PeriodMonth < periodMonth)
            .OrderByDescending(reading => reading.PeriodMonth)
            .FirstOrDefaultAsync(cancellationToken);

    /// <inheritdoc />
    public Task<MeterReading?> GetForPeriodAsync(
        Guid roomId,
        MeterType meterType,
        DateOnly periodMonth,
        CancellationToken cancellationToken = default)
        => _dbContext.MeterReadings.FirstOrDefaultAsync(
            reading => reading.RoomId == roomId
                && reading.MeterType == meterType
                && reading.PeriodMonth == periodMonth,
            cancellationToken);

    /// <inheritdoc />
    public Task<PagedResult<MeterReading>> GetPagedAsync(
        MeterReadingListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        IQueryable<MeterReading> readings = _dbContext.MeterReadings.AsNoTracking();

        if (filter.OrganizationId.HasValue)
        {
            readings = readings.Where(reading => reading.OrganizationId == filter.OrganizationId.Value);
        }

        if (filter.DormitoryId.HasValue)
        {
            readings = readings.Where(reading => reading.DormitoryId == filter.DormitoryId.Value);
        }

        if (filter.RoomId.HasValue)
        {
            readings = readings.Where(reading => reading.RoomId == filter.RoomId.Value);
        }

        if (filter.MeterType.HasValue)
        {
            readings = readings.Where(reading => reading.MeterType == filter.MeterType.Value);
        }

        if (filter.PeriodMonth.HasValue)
        {
            readings = readings.Where(reading => reading.PeriodMonth == filter.PeriodMonth.Value);
        }

        if (filter.RestrictToDormitoryIds is not null)
        {
            readings = readings.Where(reading =>
                reading.DormitoryId != null && filter.RestrictToDormitoryIds.Contains(reading.DormitoryId.Value));
        }

        readings = ApplySort(readings, query);

        return readings.ToPagedResultAsync(query, cancellationToken);
    }

    /// <inheritdoc />
    public Task AddAsync(MeterReading meterReading, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(meterReading);

        _dbContext.MeterReadings.Add(meterReading);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _dbContext.SaveChangesAsync(cancellationToken);

    private static IQueryable<MeterReading> ApplySort(IQueryable<MeterReading> readings, PagedQuery query)
        => query.SortBy?.ToLowerInvariant() switch
        {
            "periodmonth" => readings.OrderByDirection(reading => reading.PeriodMonth, query.SortDirection),
            "readingdate" => readings.OrderByDirection(reading => reading.ReadingDate, query.SortDirection),
            "metertype" => readings.OrderByDirection(reading => reading.MeterType, query.SortDirection),
            _ => readings.OrderByDirection(reading => reading.CreatedAt, query.SortDirection),
        };
}
