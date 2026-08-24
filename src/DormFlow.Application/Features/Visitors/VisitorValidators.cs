using DormFlow.Contracts.Visitors;
using FluentValidation;

namespace DormFlow.Application.Features.Visitors;

/// <summary>ตรวจสอบคำขอบันทึกการเข้าของผู้มาเยือน</summary>
public sealed class CheckInVisitorRequestValidator : AbstractValidator<CheckInVisitorRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public CheckInVisitorRequestValidator()
    {
        RuleFor(request => request.RoomId)
            .NotEmpty().WithMessage("กรุณาระบุห้องที่ผู้มาเยือนมาเยี่ยม");

        RuleFor(request => request.VisitorName)
            .NotEmpty().WithMessage("กรุณาระบุชื่อผู้มาเยือน")
            .MaximumLength(200).WithMessage("ชื่อผู้มาเยือนต้องยาวไม่เกิน 200 ตัวอักษร");

        RuleFor(request => request.VisitorPhone)
            .MaximumLength(30).WithMessage("เบอร์โทรศัพท์ต้องยาวไม่เกิน 30 ตัวอักษร");

        RuleFor(request => request.IdNumber)
            .MaximumLength(20).WithMessage("เลขบัตรประจำตัวต้องยาวไม่เกิน 20 ตัวอักษร");

        RuleFor(request => request.Purpose)
            .MaximumLength(500).WithMessage("วัตถุประสงค์ต้องยาวไม่เกิน 500 ตัวอักษร");

        RuleFor(request => request.Notes)
            .MaximumLength(1000).WithMessage("หมายเหตุต้องยาวไม่เกิน 1000 ตัวอักษร");
    }
}
