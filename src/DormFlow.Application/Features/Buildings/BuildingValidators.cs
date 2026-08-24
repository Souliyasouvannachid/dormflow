using DormFlow.Contracts.Buildings;
using FluentValidation;

namespace DormFlow.Application.Features.Buildings;

/// <summary>ตรวจสอบคำขอสร้างอาคารใหม่</summary>
public sealed class CreateBuildingRequestValidator : AbstractValidator<CreateBuildingRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public CreateBuildingRequestValidator()
    {
        RuleFor(request => request.DormitoryId)
            .NotEmpty().WithMessage("กรุณาระบุหอพักที่อาคารนี้สังกัด");

        RuleFor(request => request.Code)
            .NotEmpty().WithMessage("กรุณาระบุรหัสอาคาร")
            .MaximumLength(50).WithMessage("รหัสอาคารต้องยาวไม่เกิน 50 ตัวอักษร");

        RuleFor(request => request.Name)
            .NotEmpty().WithMessage("กรุณาระบุชื่ออาคาร")
            .MaximumLength(200).WithMessage("ชื่ออาคารต้องยาวไม่เกิน 200 ตัวอักษร");

        RuleFor(request => request.TotalFloors)
            .GreaterThan(0).WithMessage("จำนวนชั้นต้องมากกว่า 0");
    }
}

/// <summary>ตรวจสอบคำขอแก้ไขข้อมูลอาคาร</summary>
public sealed class UpdateBuildingRequestValidator : AbstractValidator<UpdateBuildingRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public UpdateBuildingRequestValidator()
    {
        RuleFor(request => request.Code)
            .NotEmpty().WithMessage("กรุณาระบุรหัสอาคาร")
            .MaximumLength(50).WithMessage("รหัสอาคารต้องยาวไม่เกิน 50 ตัวอักษร");

        RuleFor(request => request.Name)
            .NotEmpty().WithMessage("กรุณาระบุชื่ออาคาร")
            .MaximumLength(200).WithMessage("ชื่ออาคารต้องยาวไม่เกิน 200 ตัวอักษร");

        RuleFor(request => request.TotalFloors)
            .GreaterThan(0).WithMessage("จำนวนชั้นต้องมากกว่า 0");
    }
}
