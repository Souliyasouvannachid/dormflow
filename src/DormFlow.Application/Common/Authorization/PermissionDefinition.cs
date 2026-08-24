namespace DormFlow.Application.Common.Authorization;

/// <summary>
/// นิยามของสิทธิ์หนึ่งรายการ ใช้เป็นแหล่งความจริงเดียวสำหรับการ Seed ลงฐานข้อมูล
/// </summary>
/// <param name="Code">รหัสสิทธิ์ที่ใช้ตรวจในโค้ดและใน Claim</param>
/// <param name="Name">ชื่อสำหรับแสดงผล</param>
/// <param name="Module">โมดูลที่สิทธิ์นี้สังกัด</param>
/// <param name="Description">คำอธิบายเพิ่มเติม</param>
public sealed record PermissionDefinition(
    string Code,
    string Name,
    string Module,
    string? Description = null);
