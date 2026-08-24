using System.Net.Http.Headers;
using System.Net.Http.Json;
using DormFlow.Contracts.Authentication;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Enums;
using DormFlow.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DormFlow.IntegrationTests.Infrastructure;

/// <summary>สร้างผู้ใช้ทดสอบผ่าน UserManager โดยตรง แทนการพึ่งบัญชีผู้ดูแลระบบที่ Seed จาก environment variable</summary>
public static class TestUserFactory
{
    /// <summary>รหัสผ่านมาตรฐานที่ใช้กับผู้ใช้ทดสอบทุกคนที่สร้างจากคลาสนี้</summary>
    public const string DefaultPassword = "Test-Passw0rd1";

    /// <summary>สร้างผู้ใช้ทดสอบพร้อมบทบาทที่ระบุ แล้วคืนอีเมลที่ใช้เข้าสู่ระบบ</summary>
    /// <param name="factory">Test Host ที่ใช้สร้าง Service Scope</param>
    /// <param name="roleName">บทบาทที่ต้องการมอบให้ผู้ใช้ เป็น null เมื่อไม่ต้องการบทบาทใด</param>
    /// <param name="organizationId">องค์กรที่ผู้ใช้จะสังกัด เป็น null สำหรับผู้ใช้ที่ไม่ผูกกับองค์กรใด เช่น SuperAdmin</param>
    /// <param name="status">สถานะของบัญชี</param>
    public static async Task<string> CreateAsync(
        DormFlowApiFactory factory,
        string? roleName = null,
        Guid? organizationId = null,
        UserStatus status = UserStatus.Active)
    {
        ArgumentNullException.ThrowIfNull(factory);

        using IServiceScope scope = factory.Services.CreateScope();
        UserManager<ApplicationUser> userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        string email = $"test-{Guid.CreateVersion7():N}@dormflow.local";

        ApplicationUser user = new()
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FirstName = "ผู้ใช้",
            LastName = "ทดสอบ",
            OrganizationId = organizationId,
            Status = status,
        };

        IdentityResult createResult = await userManager.CreateAsync(user, DefaultPassword);
        if (!createResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"สร้างผู้ใช้ทดสอบไม่สำเร็จ: {string.Join(", ", createResult.Errors.Select(error => error.Description))}");
        }

        if (roleName is not null)
        {
            IdentityResult roleResult = await userManager.AddToRoleAsync(user, roleName);
            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"เพิ่มบทบาทให้ผู้ใช้ทดสอบไม่สำเร็จ: {string.Join(", ", roleResult.Errors.Select(error => error.Description))}");
            }
        }

        return email;
    }

    /// <summary>เข้าสู่ระบบด้วยอีเมลที่ระบุ คืนโทเคนทั้งชุดที่ได้รับ</summary>
    /// <param name="client">HttpClient ของ Test Host</param>
    /// <param name="email">อีเมลที่ใช้เข้าสู่ระบบ ต้องสร้างไว้ล่วงหน้าด้วย <see cref="CreateAsync"/></param>
    public static async Task<AuthTokenResponse> LoginAsync(HttpClient client, string email)
    {
        ArgumentNullException.ThrowIfNull(client);

        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/auth/login", UriKind.Relative),
            new LoginRequest { Email = email, Password = DefaultPassword },
            TestContext.Current.CancellationToken);

        ApiResponse<AuthTokenResponse>? body = await response.Content
            .ReadFromJsonAsync<ApiResponse<AuthTokenResponse>>(TestContext.Current.CancellationToken);

        return body!.Data!;
    }

    /// <summary>
    /// สร้างผู้ใช้ทดสอบ เข้าสู่ระบบ แล้วคืน HttpClient ที่แนบ Access Token ไว้ให้พร้อมใช้งาน
    /// </summary>
    /// <param name="factory">Test Host ที่ใช้สร้าง Service Scope และ HttpClient</param>
    /// <param name="roleName">บทบาทที่ต้องการมอบให้ผู้ใช้ เป็น null เมื่อไม่ต้องการบทบาทใด</param>
    /// <param name="organizationId">องค์กรที่ผู้ใช้จะสังกัด เป็น null สำหรับผู้ใช้ที่ไม่ผูกกับองค์กรใด เช่น SuperAdmin</param>
    public static async Task<HttpClient> CreateAuthenticatedClientAsync(
        DormFlowApiFactory factory,
        string? roleName = null,
        Guid? organizationId = null)
    {
        ArgumentNullException.ThrowIfNull(factory);

        HttpClient client = factory.CreateClient();
        string email = await CreateAsync(factory, roleName, organizationId);
        AuthTokenResponse tokens = await LoginAsync(client, email);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

        return client;
    }
}
