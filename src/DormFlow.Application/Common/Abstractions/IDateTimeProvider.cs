namespace DormFlow.Application.Common.Abstractions;

/// <summary>
/// แหล่งเวลาของระบบ แยกเป็น Abstraction เพื่อให้เขียน Unit Test ที่ควบคุมเวลาได้
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>เวลาปัจจุบันในรูปแบบ UTC เก็บลง PostgreSQL เป็น timestamptz</summary>
    DateTimeOffset UtcNow { get; }
}
