using DormFlow.Contracts.Notifications;
using FluentValidation;

namespace DormFlow.Application.Features.Notifications;

/// <summary>ตรวจสอบคำขอส่งการแจ้งเตือน</summary>
public sealed class SendNotificationRequestValidator : AbstractValidator<SendNotificationRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public SendNotificationRequestValidator()
    {
        RuleFor(request => request.RecipientUserId)
            .NotEmpty().WithMessage("กรุณาระบุผู้รับการแจ้งเตือน");

        RuleFor(request => request.Title)
            .NotEmpty().WithMessage("กรุณาระบุหัวข้อการแจ้งเตือน")
            .MaximumLength(200).WithMessage("หัวข้อการแจ้งเตือนต้องยาวไม่เกิน 200 ตัวอักษร");

        RuleFor(request => request.Message)
            .NotEmpty().WithMessage("กรุณาระบุเนื้อหาการแจ้งเตือน")
            .MaximumLength(2000).WithMessage("เนื้อหาการแจ้งเตือนต้องยาวไม่เกิน 2000 ตัวอักษร");
    }
}
