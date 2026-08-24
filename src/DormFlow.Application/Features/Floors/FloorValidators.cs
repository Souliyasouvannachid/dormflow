using DormFlow.Contracts.Floors;
using FluentValidation;

namespace DormFlow.Application.Features.Floors;

/// <summary>ตรวจสอบคำขอสร้างชั้นใหม่</summary>
public sealed class CreateFloorRequestValidator : AbstractValidator<CreateFloorRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public CreateFloorRequestValidator()
    {
        RuleFor(request => request.BuildingId)
            .NotEmpty().WithMessage("กรุณาระบุอาคารที่ชั้นนี้สังกัด");

        RuleFor(request => request.FloorNumber)
            .InclusiveBetween(-5, 200).WithMessage("ลำดับชั้นไม่ถูกต้อง");

        RuleFor(request => request.Name)
            .MaximumLength(100).WithMessage("ชื่อชั้นต้องยาวไม่เกิน 100 ตัวอักษร");
    }
}

/// <summary>ตรวจสอบคำขอแก้ไขข้อมูลชั้น</summary>
public sealed class UpdateFloorRequestValidator : AbstractValidator<UpdateFloorRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public UpdateFloorRequestValidator()
    {
        RuleFor(request => request.FloorNumber)
            .InclusiveBetween(-5, 200).WithMessage("ลำดับชั้นไม่ถูกต้อง");

        RuleFor(request => request.Name)
            .MaximumLength(100).WithMessage("ชื่อชั้นต้องยาวไม่เกิน 100 ตัวอักษร");
    }
}
