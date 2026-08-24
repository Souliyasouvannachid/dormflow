using System.Text.Json;
using DormFlow.Contracts.Common;
using FluentAssertions;
using Xunit;

namespace DormFlow.UnitTests.Contracts;

/// <summary>
/// ตรวจว่ารูปแบบ JSON ของ API Response ตรงตามสัญญาที่ประกาศไว้กับฝั่ง Client
/// เพราะการเปลี่ยนชื่อฟิลด์หรือลำดับฟิลด์คือ breaking change
/// </summary>
public class ApiResponseTests
{
    private static readonly JsonSerializerOptions SerializerOptions = DormFlowJsonSerializerOptions.Default;

    [Fact]
    public void Ok_ควรตั้งค่า_Success_เป็นจริงและไม่มีข้อมูลข้อผิดพลาด()
    {
        ApiResponse response = ApiResponse.Ok();

        response.Success.Should().BeTrue();
        response.Message.Should().Be(ApiMessages.Success);
        response.Errors.Should().BeNull();
        response.TraceId.Should().BeNull();
    }

    [Fact]
    public void Ok_พร้อมข้อมูล_ควรคืนข้อมูลเดิมกลับมา()
    {
        var payload = new { Id = 7 };

        ApiResponse<object> response = ApiResponse.Ok<object>(payload, "ดึงข้อมูลสำเร็จ");

        response.Success.Should().BeTrue();
        response.Message.Should().Be("ดึงข้อมูลสำเร็จ");
        response.Data.Should().BeSameAs(payload);
    }

    [Fact]
    public void Fail_ควรตั้งค่า_Success_เป็นเท็จและเก็บ_TraceId()
    {
        ApiResponse response = ApiResponse.Fail(ApiMessages.NotFound, "trace-001");

        response.Success.Should().BeFalse();
        response.Message.Should().Be(ApiMessages.NotFound);
        response.TraceId.Should().Be("trace-001");
    }

    [Fact]
    public void ValidationFailed_ควรเก็บรายละเอียดรายฟิลด์ไว้ครบ()
    {
        Dictionary<string, string[]> errors = new(StringComparer.Ordinal)
        {
            ["name"] = ["กรุณาระบุชื่อ"],
        };

        ApiResponse response = ApiResponse.ValidationFailed(ApiMessages.ValidationFailed, errors, "trace-002");

        response.Success.Should().BeFalse();
        response.Errors.Should().ContainKey("name");
        response.Errors!["name"].Should().ContainSingle().Which.Should().Be("กรุณาระบุชื่อ");
    }

    [Fact]
    public void Serialize_กรณีสำเร็จ_ต้องไม่มีฟิลด์_errors_และ_traceId()
    {
        string json = JsonSerializer.Serialize(ApiResponse.Ok(), SerializerOptions);

        json.Should().Contain("\"success\":true");
        json.Should().Contain("\"message\":");
        json.Should().NotContain("\"errors\"");
        json.Should().NotContain("\"traceId\"");
    }

    [Fact]
    public void Serialize_กรณีมีข้อมูล_ต้องเรียงฟิลด์เป็น_success_message_data()
    {
        ApiResponse<int> response = ApiResponse.Ok(42);

        string json = JsonSerializer.Serialize(response, SerializerOptions);

        json.Should().Be("{\"success\":true,\"message\":\"" + ApiMessages.Success + "\",\"data\":42}");
    }

    [Fact]
    public void Serialize_กรณี_Validation_ไม่ผ่าน_ต้องมี_errors_และ_traceId()
    {
        Dictionary<string, string[]> errors = new(StringComparer.Ordinal)
        {
            ["name"] = ["กรุณาระบุชื่อ"],
        };

        string json = JsonSerializer.Serialize(
            ApiResponse.ValidationFailed(ApiMessages.ValidationFailed, errors, "trace-003"),
            SerializerOptions);

        json.Should().Contain("\"success\":false");
        json.Should().Contain("\"errors\":{\"name\":[\"กรุณาระบุชื่อ\"]}");
        json.Should().Contain("\"traceId\":\"trace-003\"");
    }
}
