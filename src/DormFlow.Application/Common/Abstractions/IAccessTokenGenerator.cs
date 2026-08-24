using DormFlow.Application.Common.Models;

namespace DormFlow.Application.Common.Abstractions;

/// <summary>ออก Access Token แบบ JWT ให้ผู้ใช้</summary>
public interface IAccessTokenGenerator
{
    /// <summary>สร้าง Access Token จากข้อมูลผู้ใช้</summary>
    /// <param name="profile">ข้อมูลผู้ใช้ที่จะใส่ลงใน Claim</param>
    AccessToken Generate(UserSecurityProfile profile);
}
