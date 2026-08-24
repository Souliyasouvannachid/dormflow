using DormFlow.Contracts.RoomTypes;
using FluentValidation;

namespace DormFlow.Application.Features.RoomTypes;

/// <summary>ตรวจสอบคำขอสร้างประเภทห้องใหม่</summary>
public sealed class CreateRoomTypeRequestValidator : AbstractValidator<CreateRoomTypeRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public CreateRoomTypeRequestValidator()
    {
        RuleFor(request => request.DormitoryId)
            .NotEmpty().WithMessage("กรุณาระบุหอพักที่ประเภทห้องนี้สังกัด");

        RuleFor(request => request.Code)
            .NotEmpty().WithMessage("กรุณาระบุรหัสประเภทห้อง")
            .MaximumLength(50).WithMessage("รหัสประเภทห้องต้องยาวไม่เกิน 50 ตัวอักษร");

        RuleFor(request => request.Name)
            .NotEmpty().WithMessage("กรุณาระบุชื่อประเภทห้อง")
            .MaximumLength(200).WithMessage("ชื่อประเภทห้องต้องยาวไม่เกิน 200 ตัวอักษร");

        RuleFor(request => request.MonthlyRent)
            .GreaterThanOrEqualTo(0).WithMessage("ค่าเช่ารายเดือนต้องไม่ติดลบ");

        RuleFor(request => request.DailyRent)
            .GreaterThanOrEqualTo(0).WithMessage("ค่าเช่ารายวันต้องไม่ติดลบ")
            .When(request => request.DailyRent.HasValue);

        RuleFor(request => request.DepositAmount)
            .GreaterThanOrEqualTo(0).WithMessage("ค่ามัดจำต้องไม่ติดลบ");

        RuleFor(request => request.MaximumOccupants)
            .GreaterThan(0).WithMessage("จำนวนผู้เข้าพักสูงสุดต้องมากกว่า 0");
    }
}

/// <summary>ตรวจสอบคำขอแก้ไขข้อมูลประเภทห้อง</summary>
public sealed class UpdateRoomTypeRequestValidator : AbstractValidator<UpdateRoomTypeRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public UpdateRoomTypeRequestValidator()
    {
        RuleFor(request => request.Code)
            .NotEmpty().WithMessage("กรุณาระบุรหัสประเภทห้อง")
            .MaximumLength(50).WithMessage("รหัสประเภทห้องต้องยาวไม่เกิน 50 ตัวอักษร");

        RuleFor(request => request.Name)
            .NotEmpty().WithMessage("กรุณาระบุชื่อประเภทห้อง")
            .MaximumLength(200).WithMessage("ชื่อประเภทห้องต้องยาวไม่เกิน 200 ตัวอักษร");

        RuleFor(request => request.MonthlyRent)
            .GreaterThanOrEqualTo(0).WithMessage("ค่าเช่ารายเดือนต้องไม่ติดลบ");

        RuleFor(request => request.DailyRent)
            .GreaterThanOrEqualTo(0).WithMessage("ค่าเช่ารายวันต้องไม่ติดลบ")
            .When(request => request.DailyRent.HasValue);

        RuleFor(request => request.DepositAmount)
            .GreaterThanOrEqualTo(0).WithMessage("ค่ามัดจำต้องไม่ติดลบ");

        RuleFor(request => request.MaximumOccupants)
            .GreaterThan(0).WithMessage("จำนวนผู้เข้าพักสูงสุดต้องมากกว่า 0");
    }
}
