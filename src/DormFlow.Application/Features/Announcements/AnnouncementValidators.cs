using DormFlow.Contracts.Announcements;
using FluentValidation;

namespace DormFlow.Application.Features.Announcements;

/// <summary>ตรวจสอบคำขอสร้างประกาศใหม่</summary>
public sealed class CreateAnnouncementRequestValidator : AbstractValidator<CreateAnnouncementRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public CreateAnnouncementRequestValidator()
    {
        RuleFor(request => request.DormitoryId)
            .NotEmpty().WithMessage("กรุณาระบุหอพักที่ประกาศนี้เกี่ยวข้อง");

        RuleFor(request => request.Title)
            .NotEmpty().WithMessage("กรุณาระบุหัวข้อประกาศ")
            .MaximumLength(200).WithMessage("หัวข้อประกาศต้องยาวไม่เกิน 200 ตัวอักษร");

        RuleFor(request => request.Content)
            .NotEmpty().WithMessage("กรุณาระบุเนื้อหาประกาศ")
            .MaximumLength(5000).WithMessage("เนื้อหาประกาศต้องยาวไม่เกิน 5000 ตัวอักษร");

        RuleFor(request => request.Notes)
            .MaximumLength(1000).WithMessage("หมายเหตุต้องยาวไม่เกิน 1000 ตัวอักษร");
    }
}
