using DormFlow.Contracts.Rooms;
using FluentValidation;

namespace DormFlow.Application.Features.Rooms;

/// <summary>ตรวจสอบคำขอสร้างห้องพักใหม่</summary>
public sealed class CreateRoomRequestValidator : AbstractValidator<CreateRoomRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public CreateRoomRequestValidator()
    {
        RuleFor(request => request.BuildingId)
            .NotEmpty().WithMessage("กรุณาระบุอาคารที่ห้องนี้สังกัด");

        RuleFor(request => request.FloorId)
            .NotEmpty().WithMessage("กรุณาระบุชั้นที่ห้องนี้สังกัด");

        RuleFor(request => request.RoomTypeId)
            .NotEmpty().WithMessage("กรุณาระบุประเภทของห้อง");

        RuleFor(request => request.RoomNumber)
            .NotEmpty().WithMessage("กรุณาระบุเลขห้อง")
            .MaximumLength(20).WithMessage("เลขห้องต้องยาวไม่เกิน 20 ตัวอักษร");

        RuleFor(request => request.MonthlyRent)
            .GreaterThanOrEqualTo(0).WithMessage("ค่าเช่ารายเดือนต้องไม่ติดลบ");

        RuleFor(request => request.DepositAmount)
            .GreaterThanOrEqualTo(0).WithMessage("ค่ามัดจำต้องไม่ติดลบ");

        RuleFor(request => request.MaximumOccupants)
            .GreaterThan(0).WithMessage("จำนวนผู้เข้าพักสูงสุดต้องมากกว่า 0");
    }
}

/// <summary>ตรวจสอบคำขอแก้ไขข้อมูลห้องพัก</summary>
public sealed class UpdateRoomRequestValidator : AbstractValidator<UpdateRoomRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public UpdateRoomRequestValidator()
    {
        RuleFor(request => request.FloorId)
            .NotEmpty().WithMessage("กรุณาระบุชั้นที่ห้องนี้สังกัด");

        RuleFor(request => request.RoomTypeId)
            .NotEmpty().WithMessage("กรุณาระบุประเภทของห้อง");

        RuleFor(request => request.RoomNumber)
            .NotEmpty().WithMessage("กรุณาระบุเลขห้อง")
            .MaximumLength(20).WithMessage("เลขห้องต้องยาวไม่เกิน 20 ตัวอักษร");

        RuleFor(request => request.MonthlyRent)
            .GreaterThanOrEqualTo(0).WithMessage("ค่าเช่ารายเดือนต้องไม่ติดลบ");

        RuleFor(request => request.DepositAmount)
            .GreaterThanOrEqualTo(0).WithMessage("ค่ามัดจำต้องไม่ติดลบ");

        RuleFor(request => request.MaximumOccupants)
            .GreaterThan(0).WithMessage("จำนวนผู้เข้าพักสูงสุดต้องมากกว่า 0");
    }
}
