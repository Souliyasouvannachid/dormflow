using DormFlow.Contracts.Parcels;
using FluentValidation;

namespace DormFlow.Application.Features.Parcels;

/// <summary>ตรวจสอบคำขอบันทึกพัสดุที่มาถึง</summary>
public sealed class LogParcelRequestValidator : AbstractValidator<LogParcelRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public LogParcelRequestValidator()
    {
        RuleFor(request => request.RoomId)
            .NotEmpty().WithMessage("กรุณาระบุห้องที่พัสดุนี้ส่งถึง");

        RuleFor(request => request.CourierName)
            .MaximumLength(100).WithMessage("ชื่อบริษัทขนส่งต้องยาวไม่เกิน 100 ตัวอักษร");

        RuleFor(request => request.TrackingNumber)
            .MaximumLength(100).WithMessage("เลขพัสดุต้องยาวไม่เกิน 100 ตัวอักษร");

        RuleFor(request => request.Description)
            .MaximumLength(500).WithMessage("คำอธิบายต้องยาวไม่เกิน 500 ตัวอักษร");

        RuleFor(request => request.Notes)
            .MaximumLength(1000).WithMessage("หมายเหตุต้องยาวไม่เกิน 1000 ตัวอักษร");
    }
}

/// <summary>ตรวจสอบคำขอบันทึกการรับพัสดุ</summary>
public sealed class PickupParcelRequestValidator : AbstractValidator<PickupParcelRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public PickupParcelRequestValidator()
    {
        RuleFor(request => request.PickedUpByName)
            .MaximumLength(200).WithMessage("ชื่อผู้รับพัสดุต้องยาวไม่เกิน 200 ตัวอักษร");
    }
}
