using DormFlow.Domain.Enums;
using DormFlow.Infrastructure.Identity;
using FluentAssertions;
using Xunit;

namespace DormFlow.UnitTests.Domain;

/// <summary>ตรวจตรรกะของบัญชีผู้ใช้ที่ต่อยอดจาก ASP.NET Core Identity</summary>
public class ApplicationUserTests
{
    private static ApplicationUser CreateUser(UserStatus status, bool isDeleted = false)
        => new()
        {
            FirstName = "ทดสอบ",
            LastName = "ระบบ",
            Status = status,
            IsDeleted = isDeleted,
        };

    [Fact]
    public void CanSignIn_เมื่อสถานะ_Active_และไม่ถูกลบ_ต้องเป็นจริง()
    {
        ApplicationUser user = CreateUser(UserStatus.Active);

        user.CanSignIn.Should().BeTrue();
    }

    [Theory]
    [InlineData(UserStatus.Inactive)]
    [InlineData(UserStatus.Suspended)]
    public void CanSignIn_เมื่อสถานะไม่ใช่_Active_ต้องเป็นเท็จ(UserStatus status)
    {
        ApplicationUser user = CreateUser(status);

        user.CanSignIn.Should().BeFalse();
    }

    [Fact]
    public void CanSignIn_เมื่อถูก_Soft_Delete_ต้องเป็นเท็จแม้สถานะเป็น_Active()
    {
        ApplicationUser user = CreateUser(UserStatus.Active, isDeleted: true);

        user.CanSignIn.Should().BeFalse();
    }

    [Fact]
    public void FullName_ต้องรวมชื่อและนามสกุลโดยตัดช่องว่างส่วนเกิน()
    {
        ApplicationUser user = CreateUser(UserStatus.Active);

        user.FullName.Should().Be("ทดสอบ ระบบ");
    }
}
