using DormFlow.Contracts.Maintenance;
using DormFlow.Domain.Enums;
using FluentValidation;

namespace DormFlow.Application.Features.Maintenance;

/// <summary>ตรวจสอบคำขอแจ้งซ่อมบำรุง</summary>
public sealed class CreateMaintenanceRequestRequestValidator : AbstractValidator<CreateMaintenanceRequestRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public CreateMaintenanceRequestRequestValidator()
    {
        RuleFor(request => request.RoomId)
            .NotEmpty().WithMessage("กรุณาระบุห้องที่แจ้งซ่อม");

        RuleFor(request => request.Title)
            .NotEmpty().WithMessage("กรุณาระบุหัวข้อของงานซ่อม")
            .MaximumLength(200).WithMessage("หัวข้อต้องยาวไม่เกิน 200 ตัวอักษร");

        RuleFor(request => request.Description)
            .MaximumLength(2000).WithMessage("รายละเอียดต้องยาวไม่เกิน 2000 ตัวอักษร");

        RuleFor(request => request.Priority)
            .Must(value => Enum.TryParse<MaintenancePriority>(value, ignoreCase: true, out _))
            .WithMessage("ระดับความสำคัญไม่ถูกต้อง ต้องเป็น Low, Medium, High หรือ Urgent");
    }
}

/// <summary>ตรวจสอบคำขอมอบหมายช่างซ่อมบำรุง</summary>
public sealed class AssignMaintenanceRequestRequestValidator : AbstractValidator<AssignMaintenanceRequestRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public AssignMaintenanceRequestRequestValidator()
    {
        RuleFor(request => request.TechnicianId)
            .NotEmpty().WithMessage("กรุณาระบุช่างซ่อมบำรุงที่ต้องการมอบหมาย");
    }
}
