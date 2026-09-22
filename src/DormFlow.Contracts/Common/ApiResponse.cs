using System.Text.Json.Serialization;

namespace DormFlow.Contracts.Common;

/// <summary>
/// ຮູບແບບ Response ມາດຕະຖານຂອງ DormFlow API ໂດຍບໍ່ມີຂໍ້ມູນກັບຄືນມາ
/// </summary>
/// <example>
/// {
///   "success": true,
///   "message": "ສຳເລັດແລ້ວ"
/// }
/// </example>
public class ApiResponse
{
    /// <summary>ການຮ້ອງຂໍປະສົບຜົນສໍາເລັດບໍ?</summary>
    [JsonPropertyOrder(-30)]
    public bool Success { get; init; }

    /// <summary>ຂໍ້ຄວາມສະຫຼຸບເພື່ອສະແດງໃຫ້ຜູ້ໃຊ້</summary>
    [JsonPropertyOrder(-20)]
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// ລາຍລະອຽດຄວາມຜິດພາດພາກສະຜາມໂດຍພາກສະຜາມ, ສົ່ງພຽງແຕ່ໃນກໍລະນີ Validation ບໍ່ຜ່ານ
    /// key ແມ່ນຊື່ພາກສະຜາມເຊັ່ນ camelCase, value ແມ່ນບັນຊີລາຍຊື່ຂອງຂໍ້ຄວາມຜິດພາດສໍາລັບພາກສະຜາມນັ້ນ
    /// </summary>
    [JsonPropertyOrder(10)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyDictionary<string, string[]>? Errors { get; init; }

    /// <summary>ຂໍລະຫັດອ້າງອີງທີ່ໃຊ້ໃນການຄົ້ນຫາ Log ຂ້າງເຊີບເວີສົ່ງພຽງແຕ່ໃນກໍລະນີຂອງຄວາມຜິດພາດ</summary>
    [JsonPropertyOrder(20)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TraceId { get; init; }

    /// <summary>ສ້າງ Response ສຳເລັດແລ້ວທີ່ບໍ່ມີຂໍ້ມູນກັບຄືນມາ</summary>
    /// <param name="message">ຂໍ້ຄວາມສະຫຼຸບ</param>
    public static ApiResponse Ok(string message = ApiMessages.Success)
        => new() { Success = true, Message = message };

    /// <summary>ສ້າງ Response ສໍາເລັດດ້ວຍຂໍ້ມູນ</summary>
    /// <typeparam name="TData">ປະເພດຂອງຂໍ້ມູນກັບຄືນມາ</typeparam>
    /// <param name="data">ຂໍ້ມູນກັບຄືນມາ</param>
    /// <param name="message">ຂໍ້ຄວາມສະຫຼຸບ</param>
    public static ApiResponse<TData> Ok<TData>(TData data, string message = ApiMessages.Success)
        => new() { Success = true, Message = message, Data = data };

    /// <summary>ສ້າງ Response ສໍາເລັດສໍາລັບຂໍ້ມູນ paginated</summary>
    /// <typeparam name="TItem">ປະເພດຂອງລາຍການໃນຜ້າ</typeparam>
    /// <param name="result">ຜົນໄດ້ຮັບແບບ paginated</param>
    /// <param name="message">ຂໍ້ຄວາມສະຫຼຸບ</param>
    public static ApiResponse<PagedResult<TItem>> Paged<TItem>(
        PagedResult<TItem> result,
        string message = ApiMessages.Retrieved)
        => new() { Success = true, Message = message, Data = result };

    /// <summary>ສ້າງ Response ຜິດພາດທີ່ບໍ່ມີລາຍລະອຽດຕໍ່ຊ່ອງຂໍ້ມູນ</summary>
    /// <param name="message">ຂໍ້ຄວາມຜິດພາດ</param>
    /// <param name="traceId">ຂໍລະຫັດອ້າງອີງ</param>
    public static ApiResponse Fail(string message, string? traceId = null)
        => new() { Success = false, Message = message, TraceId = traceId };

    /// <summary>ສ້າງ Response ຜິດພາດກັບລາຍລະອຽດຕໍ່ຊ່ອງຂໍ້ມູນ</summary>
    /// <param name="message">ຂໍ້ຄວາມຜິດພາດ</param>
    /// <param name="errors">ລາຍລະອຽດຄວາມຜິດພາດພາກສະຜາມໂດຍພາກສະຜາມ</param>
    /// <param name="traceId">ຂໍລະຫັດອ້າງອີງ</param>
    public static ApiResponse ValidationFailed(
        string message,
        IReadOnlyDictionary<string, string[]> errors,
        string? traceId = null)
        => new() { Success = false, Message = message, Errors = errors, TraceId = traceId };
}

/// <summary>
/// ຮູບແບບ Response ມາດຕະຖານຂອງ DormFlow API ກັບຂໍ້ມູນທີ່ສົ່ງກັບຄືນໄປບ່ອນ
/// </summary>
/// <typeparam name="TData">ປະເພດຂອງຂໍ້ມູນກັບຄືນມາ</typeparam>
/// <example>
/// {
///   "success": true,
///   "message": "ດຶງຂໍ້ມູນສຳເລັດແລ້ວ",
///   "data": { }
/// }
/// </example>
public class ApiResponse<TData> : ApiResponse
{
    /// <summary>ຂໍ້ມູນກັບຄືນມາແມ່ນ null ເມື່ອການຮ້ອງຂໍບໍ່ສໍາເລັດ</summary>
    [JsonPropertyOrder(0)]
    public TData? Data { get; init; }
}
