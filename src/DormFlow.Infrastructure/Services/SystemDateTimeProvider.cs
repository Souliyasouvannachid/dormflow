using DormFlow.Application.Common.Abstractions;

namespace DormFlow.Infrastructure.Services;

/// <summary>
/// แหล่งเวลาจริงของระบบ คืนค่าเป็น UTC เสมอเพื่อให้เก็บลง timestamptz ได้ตรงกันทุกเครื่อง
/// </summary>
public sealed class SystemDateTimeProvider : IDateTimeProvider
{
    /// <inheritdoc />
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
