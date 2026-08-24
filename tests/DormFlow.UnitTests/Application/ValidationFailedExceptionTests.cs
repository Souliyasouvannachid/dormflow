using DormFlow.Application.Common.Exceptions;
using FluentAssertions;
using FluentValidation.Results;
using Xunit;

namespace DormFlow.UnitTests.Application;

/// <summary>
/// ตรวจว่าข้อผิดพลาดจาก FluentValidation ถูกแปลงเป็นคีย์แบบ camelCase
/// ให้ตรงกับชื่อฟิลด์ในเอกสาร JSON ที่ Client ส่งเข้ามา
/// </summary>
public class ValidationFailedExceptionTests
{
    [Fact]
    public void ชื่อฟิลด์_ต้องถูกแปลงเป็น_camelCase()
    {
        ValidationFailedException exception = new([new ValidationFailure("RoomNumber", "กรุณาระบุเลขห้อง")]);

        exception.Errors.Should().ContainKey("roomNumber");
    }

    [Fact]
    public void ชื่อฟิลด์ซ้อน_ต้องถูกแปลงทุกระดับ()
    {
        ValidationFailedException exception = new([new ValidationFailure("Address.PostalCode", "รหัสไปรษณีย์ไม่ถูกต้อง")]);

        exception.Errors.Should().ContainKey("address.postalCode");
    }

    [Fact]
    public void ข้อผิดพลาดหลายรายการของฟิลด์เดียวกัน_ต้องถูกรวมไว้ด้วยกัน()
    {
        ValidationFailedException exception = new(
        [
            new ValidationFailure("Code", "กรุณาระบุรหัส"),
            new ValidationFailure("Code", "รหัสต้องยาวไม่เกิน 20 ตัวอักษร"),
        ]);

        exception.Errors["code"].Should().HaveCount(2);
    }

    [Fact]
    public void ข้อความซ้ำของฟิลด์เดียวกัน_ต้องเหลือรายการเดียว()
    {
        ValidationFailedException exception = new(
        [
            new ValidationFailure("Code", "กรุณาระบุรหัส"),
            new ValidationFailure("Code", "กรุณาระบุรหัส"),
        ]);

        exception.Errors["code"].Should().ContainSingle();
    }

    [Fact]
    public void สร้างโดยไม่ระบุรายการข้อผิดพลาด_ต้องได้พจนานุกรมว่าง()
    {
        ValidationFailedException exception = new();

        exception.Errors.Should().BeEmpty();
        exception.Message.Should().NotBeNullOrWhiteSpace();
    }
}
