using DormFlow.Domain.Enums;
using DormFlow.Infrastructure.Identity;
using FluentAssertions;
using Xunit;

namespace DormFlow.UnitTests.Domain;

/// <summary>ກວດເບິ່ງເຫດຜົນຂອງບັນຊີຜູ້ໃຊ້ທີ່ທ່ານສ້າງຂຶ້ນ ASP.NET Core Identity</summary>
public class ApplicationUserTests
{
    private static ApplicationUser CreateUser(UserStatus status, bool isDeleted = false)
        => new()
        {
            FirstName = "ການທົດສອບ",
            LastName = "ລະບົບ",
            Status = status,
            IsDeleted = isDeleted,
        };

    [Fact]
    public void CanSignIn_ໃນເວລາທີ່ສະຖານະ_Active_ແລະບໍ່ໄດ້ລຶບ_ຕ້ອງເປັນຄວາມຈິງ()
    {
        ApplicationUser user = CreateUser(UserStatus.Active);

        user.CanSignIn.Should().BeTrue();
    }

    [Theory]
    [InlineData(UserStatus.Inactive)]
    [InlineData(UserStatus.Suspended)]
    public void CanSignIn_ໃນເວລາທີ່ສະຖານະບໍ່ແມ່ນ_Active_ຈະຕ້ອງບໍ່ຖືກຕ້ອງ(UserStatus status)
    {
        ApplicationUser user = CreateUser(status);

        user.CanSignIn.Should().BeFalse();
    }

    [Fact]
    public void CanSignIn_ເມື່ອມັນຖືກຕ້ອງ_Soft_Delete_ຈະຕ້ອງບໍ່ຖືກຕ້ອງເຖິງແມ່ນວ່າສະຖານະແມ່ນ_Active()
    {
        ApplicationUser user = CreateUser(UserStatus.Active, isDeleted: true);

        user.CanSignIn.Should().BeFalse();
    }

    [Fact]
    public void FullName_ນາມສະກຸນແລະນາມສະກຸນຈະຕ້ອງຖືກລວມເຂົ້າກໍາຈັດຊ່ອງຫວ່າງເພີ່ມເຕີມ()
    {
        ApplicationUser user = CreateUser(UserStatus.Active);

        user.FullName.Should().Be($"{user.FirstName} {user.LastName}");
    }
}
