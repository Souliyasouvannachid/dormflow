using Asp.Versioning;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Contracts.Authentication;
using DormFlow.Contracts.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DormFlow.Api.Controllers.V1;

/// <summary>Endpoint ສໍາລັບການຢັ້ງຢືນຕົວຕົນ, ເຂົ້າສູ່ລະບົບ, ການຕໍ່ອາຍຸໂທເຄັນ, ແລະການຄຸ້ມຄອງລະຫັດຜ່ານ</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
[Produces("application/json")]
public sealed class AuthController(IAuthenticationService authenticationService) : ControllerBase
{
    private readonly IAuthenticationService _authenticationService = authenticationService;

    /// <summary>ເຂົ້າສູ່ລະບົບດ້ວຍອີເມວ ແລະລະຫັດຜ່ານ</summary>
    /// <param name="request">ອີເມວ ແລະລະຫັດຜ່ານ</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    /// <response code="200">ເຂົ້າສູ່ລະບົບສຳເລັດແລ້ວ. ໄດ້ຮັບແລ້ວ Access Token ແລະ Refresh Token</response>
    /// <response code="400">ຂໍ້ມູນທີ່ສົ່ງມາບໍ່ຖືກຕ້ອງ</response>
    /// <response code="401">ອີເມວ ຫຼືລະຫັດຜ່ານບໍ່ຖືກຕ້ອງ</response>
    /// <response code="403">ບັນຊີ ຫຼືອົງກອນຂອງທ່ານຖືກລັອກ, ປິດການນຳໃຊ້ ຫຼືຖືກລະງັບໄວ້</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthTokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<AuthTokenResponse>>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        AuthTokenResponse result = await _authenticationService.LoginAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok(result, "ເຂົ້າສູ່ລະບົບສຳເລັດ"));
    }

    /// <summary>ຮ້ອງຂໍ Access Token ໃບໃຝ່ເຊັ່ນດຽວກັນ Refresh Token</summary>
    /// <param name="request">Refresh Token ຖື</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    /// <remarks>Refresh Token ບັດຕົ້ນສະບັບຈະຖືກຖອນຄືນທັນທີແລະບັດໃຝ່ຈະຖືກສົ່ງຄືນຢູ່ໃນສະຖານທີ່ຂອງຕົນຕາມກົນໄກ Token Rotation</remarks>
    /// <response code="200">ສຳເລັດການຕໍ່ອາຍຸ, ໄດ້ຮັບ Access Token ແລະ Refresh Token ຊຸດໃໝ່</response>
    /// <response code="400">ຂໍ້ມູນທີ່ສົ່ງມາບໍ່ຖືກຕ້ອງ</response>
    /// <response code="401">Refresh Token ບໍ່ຖືກຕ້ອງ, ໝົດອາຍຸ, ຫຼືຖືກຖອນອອກ</response>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthTokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<AuthTokenResponse>>> RefreshToken(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        AuthTokenResponse result = await _authenticationService.RefreshTokenAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok(result, "ຕໍ່ອາຍຸ Token ສຳເລັດ"));
    }

    /// <summary>ອອກຈາກລະບົບແລະຖອນຄືນ Refresh Token ສົ່ງ</summary>
    /// <param name="request">Refresh Token ທີ່ທ່ານຕ້ອງການຖອນຄືນ</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    /// <response code="200">ອອກຈາກລະບົບສຳເລັດແລ້ວ</response>
    /// <response code="400">ຂໍ້ມູນທີ່ສົ່ງມາບໍ່ຖືກຕ້ອງ</response>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> Logout(
        [FromBody] LogoutRequest request,
        CancellationToken cancellationToken)
    {
        await _authenticationService.LogoutAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok("ອອກຈາກລະບົບສຳເລັດ"));
    }

    /// <summary>ຂໍລິ້ງເພື່ອຣີເຊັດລະຫັດຜ່ານຂອງທ່ານຜ່ານທາງອີເມວ</summary>
    /// <param name="request">ອີເມວບັນຊີ</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    /// <remarks>ຕອບກັບຂໍ້ຄວາມດຽວກັນສະເໝີວ່າພົບບັນຊີທີ່ໃຊ້ທີ່ຢູ່ອີເມວນີ້ຫຼືບໍ່</remarks>
    /// <response code="200">ຍອມຮັບການຮ້ອງຂໍ. ຖ້າທ່ານມີບັນຊີກັບທີ່ຢູ່ອີເມວນີ້, ທ່ານຈະໄດ້ຮັບອີເມວເພື່ອປ່ຽນລະຫັດຜ່ານຂອງທ່ານ</response>
    /// <response code="400">ຂໍ້ມູນທີ່ສົ່ງມາບໍ່ຖືກຕ້ອງ</response>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> ForgotPassword(
        [FromBody] ForgotPasswordRequest request,
        CancellationToken cancellationToken)
    {
        await _authenticationService.ForgotPasswordAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok("ຫາກມີບັນຊີທີ່ໃຊ້ອີເມວນີ້ຢູ່ໃນລະບົບ ທ່ານຈະໄດ້ຮັບອີເມວສຳລັບຕັ້ງລະຫັດຜ່ານໃໝ່"));
    }

    /// <summary>ຕັ້ງລະຫັດຜ່ານໃໝ່ດ້ວຍໂທເຄັນທີ່ໄດ້ຮັບຜ່ານທາງອີເມວ</summary>
    /// <param name="request">ອີເມວໃໝ່, ໂທເຄັນ ແລະລະຫັດຜ່ານ</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    /// <response code="200">ຕັ້ງລະຫັດຜ່ານໃໝ່ສຳເລັດແລ້ວ. ເຊດຊັນຕົ້ນສະບັບທັງໝົດຖືກຖອນຄືນ</response>
    /// <response code="400">ຂໍ້ມູນບໍ່ຖືກຕ້ອງ ຫຼືໂທເຄັນໝົດອາຍຸແລ້ວ/ໄດ້ຖືກນໍາໃຊ້</response>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> ResetPassword(
        [FromBody] ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        await _authenticationService.ResetPasswordAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok("ຕັ້ງລະຫັດຜ່ານໃໝ່ສຳເລັດ ກະລຸນາເຂົ້າສູ່ລະບົບດ້ວຍລະຫັດຜ່ານໃໝ່"));
    }

    /// <summary>ປ່ຽນລະຫັດຜ່ານສຳລັບບັນຊີທີ່ເຈົ້າເຂົ້າສູ່ລະບົບ</summary>
    /// <param name="request">ລະຫັດຜ່ານປັດຈຸບັນ ແລະລະຫັດຜ່ານໃໝ່</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    /// <remarks>ເຊດຊັນຜູ້ໃຊ້ທີ່ຜ່ານມາທັງໝົດຈະຖືກຖອນຄືນຫຼັງຈາກການປ່ຽນແປງລະຫັດຜ່ານສຳເລັດແລ້ວ</remarks>
    /// <response code="200">ປ່ຽນລະຫັດຜ່ານສຳເລັດແລ້ວ</response>
    /// <response code="400">ຂໍ້ມູນບໍ່ຖືກຕ້ອງ ຫຼືລະຫັດຜ່ານປັດຈຸບັນບໍ່ຖືກຕ້ອງ</response>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        await _authenticationService.ChangePasswordAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok("ປ່ຽນລະຫັດຜ່ານສຳເລັດ"));
    }

    /// <summary>ອ່ານຂໍ້ມູນຂອງຜູ້ໃຊ້ທີ່ເຂົ້າສູ່ລະບົບໃນປັດຈຸບັນ</summary>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    /// <response code="200">ຂໍ້ມູນຜູ້ໃຊ້ປະຈຸບັນ</response>
    /// <response code="401">ຍັງບໍ່ໄດ້ເຂົ້າສູ່ລະບົບເທື່ອ? Token ບໍ່ຖືກຕ້ອງ</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<CurrentUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<CurrentUserResponse>>> GetCurrentUser(
        CancellationToken cancellationToken)
    {
        CurrentUserResponse result = await _authenticationService.GetCurrentUserAsync(cancellationToken);
        return Ok(ApiResponse.Ok(result, "ດຶງຂໍ້ມູນສຳເລັດ"));
    }
}
