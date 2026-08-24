using DormFlow.Infrastructure.Authentication;
using FluentAssertions;
using Xunit;

namespace DormFlow.UnitTests.Infrastructure;

/// <summary>
/// ตรวจการสร้างและแฮชค่าของ Refresh Token
/// </summary>
/// <remarks>
/// จุดสำคัญที่ต้องคุมคือค่าที่สร้างต้องสุ่มจริงและแฮชต้องคำนวณซ้ำได้ตรงกันทุกครั้ง
/// เพราะการต่ออายุโทเคนอาศัยการค้นหาด้วยค่าแฮชในฐานข้อมูล
/// </remarks>
public class RefreshTokenGeneratorTests
{
    [Fact]
    public void CreateToken_ต้องไม่คืนค่าว่าง()
    {
        string token = RefreshTokenGenerator.CreateToken();

        token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void CreateToken_เรียกสองครั้ง_ต้องได้ค่าไม่ซ้ำกัน()
    {
        string first = RefreshTokenGenerator.CreateToken();
        string second = RefreshTokenGenerator.CreateToken();

        first.Should().NotBe(second);
    }

    [Fact]
    public void CreateToken_ต้องไม่มีอักขระที่ต้องหลบหลีกใน_URL()
    {
        string token = RefreshTokenGenerator.CreateToken();

        token.Should().NotContain("+").And.NotContain("/").And.NotContain("=");
    }

    [Fact]
    public void ComputeHash_ค่าเดิม_ต้องได้แฮชเดิมทุกครั้ง()
    {
        string token = RefreshTokenGenerator.CreateToken();

        string firstHash = RefreshTokenGenerator.ComputeHash(token);
        string secondHash = RefreshTokenGenerator.ComputeHash(token);

        firstHash.Should().Be(secondHash);
    }

    [Fact]
    public void ComputeHash_ค่าต่างกัน_ต้องได้แฮชต่างกัน()
    {
        string firstToken = RefreshTokenGenerator.CreateToken();
        string secondToken = RefreshTokenGenerator.CreateToken();

        RefreshTokenGenerator.ComputeHash(firstToken)
            .Should().NotBe(RefreshTokenGenerator.ComputeHash(secondToken));
    }

    [Fact]
    public void ComputeHash_ต้องคืนค่าเป็นเลขฐานสิบหกตัวพิมพ์เล็กความยาว_64_ตัวอักษร()
    {
        string token = RefreshTokenGenerator.CreateToken();

        string hash = RefreshTokenGenerator.ComputeHash(token);

        hash.Should().HaveLength(64);
        hash.Should().MatchRegex("^[0-9a-f]{64}$");
    }

    [Fact]
    public void ComputeHash_ต้องไม่รับค่าว่าง()
    {
        Action act = () => RefreshTokenGenerator.ComputeHash(string.Empty);

        act.Should().Throw<ArgumentException>();
    }
}
