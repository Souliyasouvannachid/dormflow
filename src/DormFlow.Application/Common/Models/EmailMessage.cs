namespace DormFlow.Application.Common.Models;

/// <summary>ข้อความอีเมลหนึ่งฉบับ</summary>
/// <param name="To">อีเมลผู้รับ</param>
/// <param name="Subject">หัวข้อ</param>
/// <param name="Body">เนื้อหาแบบข้อความธรรมดา</param>
public sealed record EmailMessage(string To, string Subject, string Body);
