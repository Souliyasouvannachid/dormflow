using System.Text.Json;
using DormFlow.Contracts.Common;
using FluentAssertions;
using Xunit;

namespace DormFlow.UnitTests.Contracts;

/// <summary>
/// ກວດເບິ່ງວ່າຮູບແບບ JSON ຂອງ API Response ຕາມສັນຍາທີ່ໄດ້ປະກາດກັບຝັ່ງ Client
/// ເນື່ອງຈາກວ່າການປ່ຽນຊື່ພາກສະຜາມຫຼືຄໍາສັ່ງພາກສະຜາມແມ່ນ breaking change
/// </summary>
public class ApiResponseTests
{
    private static readonly JsonSerializerOptions SerializerOptions = DormFlowJsonSerializerOptions.Default;

    [Fact]
    public void Ok_ຄວນຈະຖືກກໍານົດ_Success_ຄວາມຈິງແລະບໍ່ມີຂໍ້ມູນຂໍ້ຜິດພາດ()
    {
        ApiResponse response = ApiResponse.Ok();

        response.Success.Should().BeTrue();
        response.Message.Should().Be(ApiMessages.Success);
        response.Errors.Should().BeNull();
        response.TraceId.Should().BeNull();
    }

    [Fact]
    public void Ok_ກັບຂໍ້ມູນ_ຂໍ້ມູນຕົ້ນສະບັບຄວນຈະຖືກສົ່ງຄືນ()
    {
        var payload = new { Id = 7 };

        ApiResponse<object> response = ApiResponse.Ok<object>(payload, "ດຶງຂໍ້ມູນສຳເລັດ");

        response.Success.Should().BeTrue();
        response.Message.Should().Be("ດຶງຂໍ້ມູນສຳເລັດ");
        response.Data.Should().BeSameAs(payload);
    }

    [Fact]
    public void Fail_ຄວນຈະຖືກກໍານົດ_Success_ບໍ່ຖືກຕ້ອງແລະຮັກສາ_TraceId()
    {
        ApiResponse response = ApiResponse.Fail(ApiMessages.NotFound, "trace-001");

        response.Success.Should().BeFalse();
        response.Message.Should().Be(ApiMessages.NotFound);
        response.TraceId.Should().Be("trace-001");
    }

    [Fact]
    public void ValidationFailed_ລາຍລະອຽດຄົບຖ້ວນສົມບູນຂອງແຕ່ລະພາກສະຜາມຄວນໄດ້ຮັບການເກັບຮັກສາໄວ້()
    {
        Dictionary<string, string[]> errors = new(StringComparer.Ordinal)
        {
            ["name"] = ["ກະລຸນາລະບຸຊື່"],
        };

        ApiResponse response = ApiResponse.ValidationFailed(ApiMessages.ValidationFailed, errors, "trace-002");

        response.Success.Should().BeFalse();
        response.Errors.Should().ContainKey("name");
        response.Errors!["name"].Should().ContainSingle().Which.Should().Be("ກະລຸນາລະບຸຊື່");
    }

    [Fact]
    public void Serialize_ກໍລະນີທີ່ປະສົບຜົນສໍາເລັດ_ຈະຕ້ອງບໍ່ມີພາກສະຜາມ_errors_ແລະ_traceId()
    {
        string json = JsonSerializer.Serialize(ApiResponse.Ok(), SerializerOptions);

        json.Should().Contain("\"success\":true");
        json.Should().Contain("\"message\":");
        json.Should().NotContain("\"errors\"");
        json.Should().NotContain("\"traceId\"");
    }

    [Fact]
    public void Serialize_ໃນກໍລະນີທີ່ມີຂໍ້ມູນ_ຊ່ອງຂໍ້ມູນຕ້ອງຖືກຈັດຮຽງເປັນ_success_message_data()
    {
        ApiResponse<int> response = ApiResponse.Ok(42);

        string json = JsonSerializer.Serialize(response, SerializerOptions);

        json.Should().Be("{\"success\":true,\"message\":\"" + ApiMessages.Success + "\",\"data\":42}");
    }

    [Fact]
    public void Serialize_ກໍລະນີ_Validation_ບໍ່ຜ່ານ_ຕ້ອງມີ_errors_ແລະ_traceId()
    {
        Dictionary<string, string[]> errors = new(StringComparer.Ordinal)
        {
            ["name"] = ["ກະລຸນາລະບຸຊື່"],
        };

        string json = JsonSerializer.Serialize(
            ApiResponse.ValidationFailed(ApiMessages.ValidationFailed, errors, "trace-003"),
            SerializerOptions);

        json.Should().Contain("\"success\":false");
        json.Should().Contain("\"errors\":{\"name\":[\"ກະລຸນາລະບຸຊື່\"]}");
        json.Should().Contain("\"traceId\":\"trace-003\"");
    }
}
