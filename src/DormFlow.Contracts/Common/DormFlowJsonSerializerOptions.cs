using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace DormFlow.Contracts.Common;

/// <summary>
/// ການຕັ້ງຄ່າການແປງ JSON ແບ່ງປັນໃນທຸກຈຸດທີ່ລະບົບຂຽນ Response
/// </summary>
/// <remarks>
/// ຮວບຮວມຢູ່ໃນສະຖານທີ່ຜຶ່ງດັ່ງນັ້ນ Response ຈາກ Controller, Middleware, Health Check ແລະ Rate Limiter
/// ພວກເຂົາທັງຝົດມີຮູບແບບດຽວກັນ
/// </remarks>
public static class DormFlowJsonSerializerOptions
{
    /// <summary>ມີການຕັ້ງຄ່າມາດຕະຖານ</summary>
    public static JsonSerializerOptions Default { get; } = CreateDefault();

    /// <summary>ສ້າງຊຸດການຕັ້ງຄ່າມາດຕະຖານໃຝ່ທີ່ຍັງສາມາດແກ້ໄຂໄດ້</summary>
    public static JsonSerializerOptions CreateDefault()
    {
        JsonSerializerOptions options = new(JsonSerializerDefaults.Web);
        Configure(options);
        options.MakeReadOnly(populateMissingResolver: true);
        return options;
    }

    /// <summary>ກະລຸນາໃສ່ການຕັ້ງຄ່າເລີ່ມຕົ້ນສໍາລັບ DormFlow ເຂົ້າໄປໃນວັດຖຸທີ່ມີຢູ່ແລ້ວ</summary>
    /// <param name="options">ການຕັ້ງຄ່າທີ່ທ່ານຕ້ອງການທີ່ຈະປັບ, ເຊັ່ນ: ASP.NET Core MVC</param>
    public static void Configure(JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

        // ປ່ອຍຕົວໜັງສືລາວອອກເປັນຕົວໜັງສືແທ້ແທນ ກ ເຮັດ Response ອ່ານງ່າຍ ແລະຂະໜາດນ້ອຍກວ່າ
        // ມັນປອດໄພທີ່ຈະໃຊ້ເພາະວ່າ API ຄໍາຕອບແມ່ນ application/json ສະເຝີ, ບໍ່ເຄີຍຝັງຜົນໄດ້ຮັບເຂົ້າໄປໃນ HTML
        options.Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Lao);

        if (!options.Converters.Any(converter => converter is JsonStringEnumConverter))
        {
            options.Converters.Add(new JsonStringEnumConverter());
        }
    }
}
