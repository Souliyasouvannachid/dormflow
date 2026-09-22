using System.Text.Json.Serialization;

namespace DormFlow.Contracts.Common;

/// <summary>ທິດທາງການຈັດຮຽງຜົນໄດ້ຮັບ</summary>
[JsonConverter(typeof(JsonStringEnumConverter<SortDirection>))]
public enum SortDirection
{
    /// <summary>ຮຽງລໍາດັບຈາກຕ່ໍາສຸດຫາສູງສຸດ</summary>
    Asc = 0,

    /// <summary>ຮຽງລໍາດັບຈາກສູງສຸດຫາຕ່ໍາສຸດ</summary>
    Desc = 1,
}
