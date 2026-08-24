using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Contracts.Authentication;
using DormFlow.Contracts.Common;
using DormFlow.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace DormFlow.IntegrationTests.Endpoints;

/// <summary>
/// ทดสอบเส้นทางการยืนยันตัวตนทั้งหมดผ่าน HTTP จริงบน PostgreSQL ใน Testcontainers
/// </summary>
[Collection(ApiTestFixtures.Name)]
public class AuthEndpointTests(DormFlowApiFactory factory)
{
    private readonly DormFlowApiFactory _factory = factory;

    [Fact]
    public async Task Login_ด้วยข้อมูลที่ถูกต้อง_ต้องได้รับ_Token_และข้อมูลผู้ใช้()
    {
        using HttpClient client = _factory.CreateClient();
        string email = await TestUserFactory.CreateAsync(_factory, DormFlowRoles.Staff);

        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/auth/login", UriKind.Relative),
            new LoginRequest { Email = email, Password = TestUserFactory.DefaultPassword },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        ApiResponse<AuthTokenResponse>? body = await response.Content
            .ReadFromJsonAsync<ApiResponse<AuthTokenResponse>>(TestContext.Current.CancellationToken);

        body.Should().NotBeNull();
        body!.Success.Should().BeTrue();
        body.Data!.AccessToken.Should().NotBeNullOrWhiteSpace();
        body.Data.RefreshToken.Should().NotBeNullOrWhiteSpace();
        body.Data.User.Email.Should().Be(email);
        body.Data.User.Roles.Should().Contain(DormFlowRoles.Staff);
    }

    [Fact]
    public async Task Login_ด้วยรหัสผ่านผิด_ต้องได้_401()
    {
        using HttpClient client = _factory.CreateClient();
        string email = await TestUserFactory.CreateAsync(_factory);

        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/auth/login", UriKind.Relative),
            new LoginRequest { Email = email, Password = "WrongPassword1" },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_ด้วยอีเมลที่ไม่มีอยู่จริง_ต้องได้_401_ข้อความเดียวกับรหัสผ่านผิด()
    {
        using HttpClient client = _factory.CreateClient();

        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/auth/login", UriKind.Relative),
            new LoginRequest { Email = "no-such-user@dormflow.local", Password = "AnyPassword1" },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        ApiResponse? body = await response.Content.ReadFromJsonAsync<ApiResponse>(TestContext.Current.CancellationToken);
        body!.Message.Should().Be("อีเมลหรือรหัสผ่านไม่ถูกต้อง");
    }

    [Fact]
    public async Task Login_ข้อมูลไม่ผ่าน_Validation_ต้องได้_400()
    {
        using HttpClient client = _factory.CreateClient();

        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/auth/login", UriKind.Relative),
            new LoginRequest { Email = "not-an-email", Password = string.Empty },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Me_เมื่อไม่ได้แนบ_Token_ต้องได้_401()
    {
        using HttpClient client = _factory.CreateClient();

        using HttpResponseMessage response = await client.GetAsync(
            new Uri("/api/v1/auth/me", UriKind.Relative),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Me_เมื่อแนบ_Token_ที่ถูกต้อง_ต้องได้ข้อมูลผู้ใช้ปัจจุบัน()
    {
        using HttpClient client = _factory.CreateClient();
        string email = await TestUserFactory.CreateAsync(_factory);
        AuthTokenResponse tokens = await LoginAsync(client, email);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

        using HttpResponseMessage response = await client.GetAsync(
            new Uri("/api/v1/auth/me", UriKind.Relative),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        ApiResponse<CurrentUserResponse>? body = await response.Content
            .ReadFromJsonAsync<ApiResponse<CurrentUserResponse>>(TestContext.Current.CancellationToken);

        body!.Data!.Email.Should().Be(email);
    }

    [Fact]
    public async Task RefreshToken_ด้วยโทเคนที่ถูกต้อง_ต้องได้รับชุดโทเคนใหม่()
    {
        using HttpClient client = _factory.CreateClient();
        string email = await TestUserFactory.CreateAsync(_factory);
        AuthTokenResponse tokens = await LoginAsync(client, email);

        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/auth/refresh-token", UriKind.Relative),
            new RefreshTokenRequest { RefreshToken = tokens.RefreshToken },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        ApiResponse<AuthTokenResponse>? body = await response.Content
            .ReadFromJsonAsync<ApiResponse<AuthTokenResponse>>(TestContext.Current.CancellationToken);

        body!.Data!.RefreshToken.Should().NotBe(tokens.RefreshToken);
        body.Data.AccessToken.Should().NotBe(tokens.AccessToken);
    }

    [Fact]
    public async Task RefreshToken_ใบเดิมหลังถูกหมุนไปแล้ว_ต้องใช้ซ้ำไม่ได้()
    {
        using HttpClient client = _factory.CreateClient();
        string email = await TestUserFactory.CreateAsync(_factory);
        AuthTokenResponse original = await LoginAsync(client, email);

        // หมุนโทเคนหนึ่งครั้งเพื่อให้ใบเดิมกลายเป็นใบที่ถูกเพิกถอนแล้ว
        await client.PostAsJsonAsync(
            new Uri("/api/v1/auth/refresh-token", UriKind.Relative),
            new RefreshTokenRequest { RefreshToken = original.RefreshToken },
            TestContext.Current.CancellationToken);

        using HttpResponseMessage reuseResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/auth/refresh-token", UriKind.Relative),
            new RefreshTokenRequest { RefreshToken = original.RefreshToken },
            TestContext.Current.CancellationToken);

        reuseResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Logout_ต้องเพิกถอนโทเคนจนใช้ต่ออายุไม่ได้อีก()
    {
        using HttpClient client = _factory.CreateClient();
        string email = await TestUserFactory.CreateAsync(_factory);
        AuthTokenResponse tokens = await LoginAsync(client, email);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

        using HttpResponseMessage logoutResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/auth/logout", UriKind.Relative),
            new LogoutRequest { RefreshToken = tokens.RefreshToken },
            TestContext.Current.CancellationToken);

        logoutResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using HttpResponseMessage refreshResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/auth/refresh-token", UriKind.Relative),
            new RefreshTokenRequest { RefreshToken = tokens.RefreshToken },
            TestContext.Current.CancellationToken);

        refreshResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ChangePassword_สำเร็จแล้ว_Refresh_Token_เดิมต้องใช้ไม่ได้อีกต่อไป()
    {
        using HttpClient client = _factory.CreateClient();
        string email = await TestUserFactory.CreateAsync(_factory);
        AuthTokenResponse tokens = await LoginAsync(client, email);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

        using HttpResponseMessage changeResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/auth/change-password", UriKind.Relative),
            new ChangePasswordRequest
            {
                CurrentPassword = TestUserFactory.DefaultPassword,
                NewPassword = "Another-Passw0rd2",
                ConfirmPassword = "Another-Passw0rd2",
            },
            TestContext.Current.CancellationToken);

        changeResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using HttpResponseMessage refreshResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/auth/refresh-token", UriKind.Relative),
            new RefreshTokenRequest { RefreshToken = tokens.RefreshToken },
            TestContext.Current.CancellationToken);

        refreshResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ForgotPassword_สำหรับอีเมลที่ไม่มีอยู่จริง_ต้องตอบสำเร็จเสมอ()
    {
        using HttpClient client = _factory.CreateClient();

        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/auth/forgot-password", UriKind.Relative),
            new ForgotPasswordRequest { Email = "no-such-user@dormflow.local" },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private static async Task<AuthTokenResponse> LoginAsync(HttpClient client, string email)
    {
        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/auth/login", UriKind.Relative),
            new LoginRequest { Email = email, Password = TestUserFactory.DefaultPassword },
            TestContext.Current.CancellationToken);

        ApiResponse<AuthTokenResponse>? body = await response.Content
            .ReadFromJsonAsync<ApiResponse<AuthTokenResponse>>(TestContext.Current.CancellationToken);

        return body!.Data!;
    }
}
