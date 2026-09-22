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
/// ທົດສອບເສັ້ນທາງການຮັບຮອງທັງໝົດຜ່ານ HTTP ແທ້ຈິງແລ້ວ PostgreSQL ໃນ Testcontainers
/// </summary>
[Collection(ApiTestFixtures.Name)]
public class AuthEndpointTests(DormFlowApiFactory factory)
{
    private readonly DormFlowApiFactory _factory = factory;

    [Fact]
    public async Task Login_ດ້ວຍຂໍ້ມູນທີ່ຖືກຕ້ອງ_ຕ້ອງໄດ້ຮັບ_Token_ແລະຂໍ້ມູນຜູ້ໃຊ້()
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
    public async Task Login_ດ້ວຍລະຫັດຜ່ານຜິດ_ຕ້ອງໄດ້ຮັບ_401()
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
    public async Task Login_ດ້ວຍທີ່ຢູ່ອີເມວທີ່ບໍ່ມີຢູ່_ຕ້ອງໄດ້ຮັບ_401_ຂໍ້ຄວາມດຽວກັນກັບລະຫັດຜ່ານຜິດ()
    {
        using HttpClient client = _factory.CreateClient();

        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/auth/login", UriKind.Relative),
            new LoginRequest { Email = "no-such-user@dormflow.local", Password = "AnyPassword1" },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        ApiResponse? body = await response.Content.ReadFromJsonAsync<ApiResponse>(TestContext.Current.CancellationToken);
        body!.Message.Should().Be("ອີເມວຫຼືລະຫັດຜ່ານບໍ່ຖືກຕ້ອງ");
    }

    [Fact]
    public async Task Login_ຂໍ້ມູນບໍ່ໄດ້ຜ່ານ_Validation_ຕ້ອງໄດ້ຮັບ_400()
    {
        using HttpClient client = _factory.CreateClient();

        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/auth/login", UriKind.Relative),
            new LoginRequest { Email = "not-an-email", Password = string.Empty },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Me_ເມື່ອບໍ່ຕິດ_Token_ຕ້ອງໄດ້ຮັບ_401()
    {
        using HttpClient client = _factory.CreateClient();

        using HttpResponseMessage response = await client.GetAsync(
            new Uri("/api/v1/auth/me", UriKind.Relative),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Me_ເມື່ອຕິດ_Token_ຖືກຕ້ອງ_ຕ້ອງໄດ້ຮັບຂໍ້ມູນຜູ້ໃຊ້ປະຈຸບັນ()
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
    public async Task RefreshToken_ກັບtokenທີ່ຖືກຕ້ອງ_ທ່ານຕ້ອງໄດ້ຮັບຊຸດໂທເຄັນໃໝ່()
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
    public async Task RefreshToken_ບັດຕົ້ນສະບັບໄດ້ຖືກຝຸນແລ້ວ_ຕ້ອງບໍ່ໄດ້ນໍາໃຊ້ຄືນ()
    {
        using HttpClient client = _factory.CreateClient();
        string email = await TestUserFactory.CreateAsync(_factory);
        AuthTokenResponse original = await LoginAsync(client, email);

        // ໝຸນໂທເຄັນຄັ້ງດຽວເພື່ອໃຫ້ເຄື່ອງເດີມຖືກຖອນຄືນ
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
    public async Task Logout_tokenຕ້ອງໄດ້ຮັບການຖອນຄືນຈົນກ່ວາມັນບໍ່ສາມາດຖືກນໍາໃຊ້ສໍາລັບການຕໍ່ອາຍຸໄດ້()
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
    public async Task ChangePassword_ມັນຈົບແລ້ວ_Refresh_Token_ອະດີດບໍ່ສາມາດໃຊ້ໄດ້ອີກຕໍ່ໄປ()
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
    public async Task ForgotPassword_ສໍາລັບທີ່ຢູ່ອີເມວທີ່ບໍ່ມີຢູ່_ຕ້ອງຕອບຢ່າງສຳເລັດຜົນສະເໝີ()
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
