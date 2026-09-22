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

/// <summary>ສ້າງຜູ້ໃຊ້ທົດສອບຜ່ານ UserManager ໂດຍກົງແທນທີ່ຈະອີງໃສ່ບັນຊີຜູ້ເບິ່ງແຍງລະບົບນັ້ນ Seed ຈາກ environment variable</summary>
public static class TestUserFactory
{
    /// <summary>ລະຫັດຜ່ານມາດຕະຖານທີ່ໃຊ້ສໍາລັບຜູ້ໃຊ້ການທົດສອບທັງຝົດທີ່ສ້າງຂຶ້ນຈາກຫ້ອງຮຽນນີ້</summary>
    public const string DefaultPassword = "Test-Passw0rd1";

    /// <summary>ສ້າງຜູ້ໃຊ້ທົດສອບທີ່ມີບົດບາດທີ່ກໍານົດໄວ້. ຈາກນັ້ນສົ່ງຄືນອີເມວທີ່ໃຊ້ເພື່ອເຂົ້າສູ່ລະບົບ</summary>
    /// <param name="factory">Test Host ໃຊ້ເພື່ອສ້າງ Service Scope</param>
    /// <param name="roleName">ບົດບາດທີ່ທ່ານຕ້ອງການທີ່ຈະມອບຝາຍໃຫ້ຜູ້ໃຊ້ແມ່ນ null ເມື່ອເຈົ້າບໍ່ຕ້ອງການບົດບາດໃດໆ</param>
    /// <param name="organizationId">ອົງກອນທີ່ຜູ້ໃຊ້ຈະຂຶ້ນກັບແມ່ນ null ສໍາລັບຜູ້ໃຊ້ທີ່ບໍ່ໄດ້ເຊື່ອມຕໍ່ກັບອົງກອນໃດຜຶ່ງ, ເຊັ່ນ: SuperAdmin</param>
    /// <param name="status">ສະຖານະບັນຊີ</param>
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
            FirstName = "ຜູ້ໃຊ້",
            LastName = "ການທົດສອບ",
            OrganizationId = organizationId,
            Status = status,
        };

        IdentityResult createResult = await userManager.CreateAsync(user, DefaultPassword);
        if (!createResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"ລົ້ມເຫຼວໃນການສ້າງຜູ້ໃຊ້ທົດສອບ: {string.Join(", ", createResult.Errors.Select(error => error.Description))}");
        }

        if (roleName is not null)
        {
            IdentityResult roleResult = await userManager.AddToRoleAsync(user, roleName);
            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"ເພີ່ມບົດບາດໃຫ້ກັບຜູ້ໃຊ້ທີ່ບໍ່ສາມາດທົດສອບໄດ້: {string.Join(", ", roleResult.Errors.Select(error => error.Description))}");
            }
        }

        return email;
    }

    /// <summary>ເຂົ້າສູ່ລະບົບດ້ວຍອີເມວທີ່ລະບຸ. ສົ່ງຄືນຊຸດໂທເຄັນທັງໝົດທີ່ໄດ້ຮັບ</summary>
    /// <param name="client">HttpClient ຂອງ Test Host</param>
    /// <param name="email">ອີເມວທີ່ໃຊ້ເພື່ອເຂົ້າສູ່ລະບົບຈະຕ້ອງສ້າງລ່ວງຜ້າ <see cref="CreateAsync"/></param>
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
    /// ສ້າງຜູ້ໃຊ້ທົດສອບ, ເຂົ້າສູ່ລະບົບ, ແລະກັບຄືນ HttpClient ຕິດ Access Token ສາມາດໃຊ້ໄດ້
    /// </summary>
    /// <param name="factory">Test Host ໃຊ້ເພື່ອສ້າງ Service Scope ແລະ HttpClient</param>
    /// <param name="roleName">ບົດບາດທີ່ທ່ານຕ້ອງການທີ່ຈະມອບຝາຍໃຫ້ຜູ້ໃຊ້ແມ່ນ null ເມື່ອເຈົ້າບໍ່ຕ້ອງການບົດບາດໃດໆ</param>
    /// <param name="organizationId">ອົງກອນທີ່ຜູ້ໃຊ້ຈະຂຶ້ນກັບແມ່ນ null ສໍາລັບຜູ້ໃຊ້ທີ່ບໍ່ໄດ້ເຊື່ອມຕໍ່ກັບອົງກອນໃດຜຶ່ງ, ເຊັ່ນ: SuperAdmin</param>
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
