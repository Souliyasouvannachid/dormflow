using Microsoft.AspNetCore.Authorization;

namespace DormFlow.Api.Authorization;

/// <summary>ข้อกำหนดว่าผู้ใช้ต้องมีสิทธิ์ตามรหัสที่ระบุ</summary>
/// <param name="PermissionCode">รหัสสิทธิ์ที่ต้องมี เช่น rooms.create</param>
public sealed record PermissionRequirement(string PermissionCode) : IAuthorizationRequirement;
