using DormFlow.Infrastructure.Authentication;
using FluentAssertions;
using Xunit;

namespace DormFlow.UnitTests.Infrastructure;

/// <summary>
/// ຢືນຢັນການສ້າງແລະຄ່າ hash ຂອງ Refresh Token
/// </summary>
/// <remarks>
/// ຈຸດສໍາຄັນໃນການຄວບຄຸມແມ່ນວ່າຄ່າທີ່ສ້າງຂຶ້ນຕ້ອງເປັນແບບສຸ່ມຢ່າງແທ້ຈິງແລະ hashes ຕ້ອງໄດ້ຮັບການຄິດໄລ່ຄືນໃຝ່ເພື່ອໃຫ້ສອດຄ່ອງທຸກໆຄັ້ງ
/// ນີ້ແມ່ນຍ້ອນວ່າການຕໍ່ອາຍຸ token ແມ່ນອີງໃສ່ການຊອກຫາຄ່າ hash ໃນຖານຂໍ້ມູນ
/// </remarks>
public class RefreshTokenGeneratorTests
{
    [Fact]
    public void CreateToken_ບໍ່ຕ້ອງກັບຄືນnull()
    {
        string token = RefreshTokenGenerator.CreateToken();

        token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void CreateToken_ໂທສອງຄັ້ງ_ຕ້ອງມີຄຸນຄ່າທີ່ເປັນເອກະລັກ()
    {
        string first = RefreshTokenGenerator.CreateToken();
        string second = RefreshTokenGenerator.CreateToken();

        first.Should().NotBe(second);
    }

    [Fact]
    public void CreateToken_ຈະຕ້ອງບໍ່ມີຕົວລະຄອນທີ່ຕ້ອງໄດ້ຮັບການmaneuveredໃນ_URL()
    {
        string token = RefreshTokenGenerator.CreateToken();

        token.Should().NotContain("+").And.NotContain("/").And.NotContain("=");
    }

    [Fact]
    public void ComputeHash_ມູນຄ່າຕົ້ນສະບັບ_ທ່ານຕ້ອງໄດ້ຮັບhashດຽວກັນທຸກຄັ້ງ()
    {
        string token = RefreshTokenGenerator.CreateToken();

        string firstHash = RefreshTokenGenerator.ComputeHash(token);
        string secondHash = RefreshTokenGenerator.ComputeHash(token);

        firstHash.Should().Be(secondHash);
    }

    [Fact]
    public void ComputeHash_ຄຸນຄ່າທີ່ແຕກຕ່າງກັນ_ທ່ານຕ້ອງໄດ້ຮັບhashesທີ່ແຕກຕ່າງກັນ()
    {
        string firstToken = RefreshTokenGenerator.CreateToken();
        string secondToken = RefreshTokenGenerator.CreateToken();

        RefreshTokenGenerator.ComputeHash(firstToken)
            .Should().NotBe(RefreshTokenGenerator.ComputeHash(secondToken));
    }

    [Fact]
    public void ComputeHash_ຕ້ອງສົ່ງຄ່າເປັນຕົວເລກເລກຖານສິບຫົກຕົວພິມນ້ອຍ_64_ຈົດໝາຍ()
    {
        string token = RefreshTokenGenerator.CreateToken();

        string hash = RefreshTokenGenerator.ComputeHash(token);

        hash.Should().HaveLength(64);
        hash.Should().MatchRegex("^[0-9a-f]{64}$");
    }

    [Fact]
    public void ComputeHash_ບໍ່ຕ້ອງຍອມຮັບຄ່າທໍານຽມເປົ່າ()
    {
        Action act = () => RefreshTokenGenerator.ComputeHash(string.Empty);

        act.Should().Throw<ArgumentException>();
    }
}
