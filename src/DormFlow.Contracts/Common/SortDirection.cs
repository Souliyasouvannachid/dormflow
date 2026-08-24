using System.Text.Json.Serialization;

namespace DormFlow.Contracts.Common;

/// <summary>ทิศทางการเรียงลำดับผลลัพธ์</summary>
[JsonConverter(typeof(JsonStringEnumConverter<SortDirection>))]
public enum SortDirection
{
    /// <summary>เรียงจากน้อยไปมาก</summary>
    Asc = 0,

    /// <summary>เรียงจากมากไปน้อย</summary>
    Desc = 1,
}
