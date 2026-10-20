using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Exceptions;
using DormFlow.Application.Features.Authentication;
using DormFlow.Contracts.Authentication;
using DormFlow.Infrastructure.Services;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace DormFlow.UnitTests.Application.Authentication;

public class DisabledEmailTests
{
    [Theory]
    [InlineData("existing@example.com")]
    [InlineData("unknown@example.com")]
    public async Task ForgotPassword_WhenEmailDisabled_RejectsBeforeLookingUpAccount(string email)
    {
        IUserAccountService accounts = Substitute.For<IUserAccountService>();
        AuthenticationService service = new(
            accounts,
            Substitute.For<IAccessTokenGenerator>(),
            Substitute.For<IRefreshTokenService>(),
            Substitute.For<IAuditLogWriter>(),
            new DisabledEmailSender(),
            Substitute.For<ICurrentUser>(),
            Substitute.For<IDateTimeProvider>(),
            new InlineValidator<LoginRequest>(),
            new InlineValidator<RefreshTokenRequest>(),
            new InlineValidator<LogoutRequest>(),
            new InlineValidator<ForgotPasswordRequest>(),
            new InlineValidator<ResetPasswordRequest>(),
            new InlineValidator<ChangePasswordRequest>(),
            Substitute.For<ILogger<AuthenticationService>>());

        Func<Task> action = () => service.ForgotPasswordAsync(new ForgotPasswordRequest { Email = email });

        await action.Should().ThrowAsync<ServiceUnavailableException>();
        await accounts.DidNotReceiveWithAnyArgs()
            .GeneratePasswordResetTokenAsync(default!, default);
    }
}
