using System.ComponentModel.DataAnnotations;

namespace DormFlow.Api.Options;

/// <summary>ຂໍ້ມູນເອກະສານ OpenAPI ອ່ານຈາກ configuration section ຊື່ Api</summary>
public sealed class ApiOptions
{
    /// <summary>ຊື່ section ໃນ appsettings</summary>
    public const string SectionName = "Api";

    /// <summary>Explicitly publishes API documentation; defaults to off in Production.</summary>
    public bool? EnableDocumentation { get; set; }

    /// <summary>ຊື່ເອກະສານ API</summary>
    [Required]
    public string Title { get; set; } = "DormFlow API ພາສາລາວ";

    /// <summary>ລາຍລະອຽດລະບົບ</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>ຊື່ຜູ້ບໍລິຫານ API</summary>
    public string ContactName { get; set; } = string.Empty;

    /// <summary>ອີເມວຜູ້ເບິ່ງແຍງລະບົບ API</summary>
    [EmailAddress]
    public string ContactEmail { get; set; } = string.Empty;
}
