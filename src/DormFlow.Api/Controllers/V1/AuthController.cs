using Asp.Versioning;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Contracts.Authentication;
using DormFlow.Contracts.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DormFlow.Api.Controllers.V1;

/// <summary>Endpoint สำหรับการยืนยันตัวตน เข้าสู่ระบบ ต่ออายุโทเคน และจัดการรหัสผ่าน</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
[Produces("application/json")]
public sealed class AuthController(IAuthenticationService authenticationService) : ControllerBase
{
    private readonly IAuthenticationService _authenticationService = authenticationService;

    /// <summary>เข้าสู่ระบบด้วยอีเมลและรหัสผ่าน</summary>
    /// <param name="request">อีเมลและรหัสผ่าน</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    /// <response code="200">เข้าสู่ระบบสำเร็จ ได้รับ Access Token และ Refresh Token</response>
    /// <response code="400">ข้อมูลที่ส่งมาไม่ถูกต้อง</response>
    /// <response code="401">อีเมลหรือรหัสผ่านไม่ถูกต้อง</response>
    /// <response code="403">บัญชีหรือองค์กรถูกล็อก ปิดใช้งาน หรือถูกระงับ</response>
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
        return Ok(ApiResponse.Ok(result, "เข้าสู่ระบบสำเร็จ"));
    }

    /// <summary>ขอ Access Token ใบใหม่ด้วย Refresh Token</summary>
    /// <param name="request">Refresh Token ที่ถืออยู่</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    /// <remarks>Refresh Token ใบเดิมจะถูกเพิกถอนทันทีและได้รับใบใหม่กลับมาแทนตามกลไก Token Rotation</remarks>
    /// <response code="200">ต่ออายุสำเร็จ ได้รับ Access Token และ Refresh Token ชุดใหม่</response>
    /// <response code="400">ข้อมูลที่ส่งมาไม่ถูกต้อง</response>
    /// <response code="401">Refresh Token ไม่ถูกต้อง หมดอายุ หรือถูกเพิกถอนไปแล้ว</response>
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
        return Ok(ApiResponse.Ok(result, "ต่ออายุ Token สำเร็จ"));
    }

    /// <summary>ออกจากระบบและเพิกถอน Refresh Token ที่ส่งมา</summary>
    /// <param name="request">Refresh Token ที่ต้องการเพิกถอน</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    /// <response code="200">ออกจากระบบสำเร็จ</response>
    /// <response code="400">ข้อมูลที่ส่งมาไม่ถูกต้อง</response>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> Logout(
        [FromBody] LogoutRequest request,
        CancellationToken cancellationToken)
    {
        await _authenticationService.LogoutAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok("ออกจากระบบสำเร็จ"));
    }

    /// <summary>ขอลิงก์ตั้งรหัสผ่านใหม่ทางอีเมล</summary>
    /// <param name="request">อีเมลของบัญชี</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    /// <remarks>ตอบข้อความเดียวกันเสมอไม่ว่าจะพบบัญชีที่ใช้อีเมลนี้อยู่จริงหรือไม่</remarks>
    /// <response code="200">รับคำขอแล้ว หากมีบัญชีที่ใช้อีเมลนี้จะได้รับอีเมลตั้งรหัสผ่านใหม่</response>
    /// <response code="400">ข้อมูลที่ส่งมาไม่ถูกต้อง</response>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> ForgotPassword(
        [FromBody] ForgotPasswordRequest request,
        CancellationToken cancellationToken)
    {
        await _authenticationService.ForgotPasswordAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok("หากมีบัญชีที่ใช้อีเมลนี้อยู่ในระบบ คุณจะได้รับอีเมลสำหรับตั้งรหัสผ่านใหม่"));
    }

    /// <summary>ตั้งรหัสผ่านใหม่ด้วยโทเคนที่ได้รับทางอีเมล</summary>
    /// <param name="request">อีเมล โทเคน และรหัสผ่านใหม่</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    /// <response code="200">ตั้งรหัสผ่านใหม่สำเร็จ เซสชันเดิมทั้งหมดถูกเพิกถอน</response>
    /// <response code="400">ข้อมูลไม่ถูกต้อง หรือโทเคนหมดอายุ/ถูกใช้ไปแล้ว</response>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> ResetPassword(
        [FromBody] ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        await _authenticationService.ResetPasswordAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok("ตั้งรหัสผ่านใหม่สำเร็จ กรุณาเข้าสู่ระบบด้วยรหัสผ่านใหม่"));
    }

    /// <summary>เปลี่ยนรหัสผ่านของบัญชีที่เข้าสู่ระบบอยู่</summary>
    /// <param name="request">รหัสผ่านปัจจุบันและรหัสผ่านใหม่</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    /// <remarks>เซสชันเดิมทั้งหมดของผู้ใช้จะถูกเพิกถอนหลังเปลี่ยนรหัสผ่านสำเร็จ</remarks>
    /// <response code="200">เปลี่ยนรหัสผ่านสำเร็จ</response>
    /// <response code="400">ข้อมูลไม่ถูกต้อง หรือรหัสผ่านปัจจุบันไม่ถูกต้อง</response>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        await _authenticationService.ChangePasswordAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok("เปลี่ยนรหัสผ่านสำเร็จ"));
    }

    /// <summary>อ่านข้อมูลของผู้ใช้ที่เข้าสู่ระบบอยู่</summary>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    /// <response code="200">ข้อมูลผู้ใช้ปัจจุบัน</response>
    /// <response code="401">ยังไม่ได้เข้าสู่ระบบหรือ Token ไม่ถูกต้อง</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<CurrentUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<CurrentUserResponse>>> GetCurrentUser(
        CancellationToken cancellationToken)
    {
        CurrentUserResponse result = await _authenticationService.GetCurrentUserAsync(cancellationToken);
        return Ok(ApiResponse.Ok(result, "ดึงข้อมูลสำเร็จ"));
    }
}
