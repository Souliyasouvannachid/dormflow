using DormFlow.Contracts.Parking;
using DormFlow.Domain.Enums;
using FluentValidation;

namespace DormFlow.Application.Features.Parking;

/// <summary>ตรวจสอบคำขอจดทะเบียนที่จอดรถ</summary>
public sealed class RegisterParkingRequestValidator : AbstractValidator<RegisterParkingRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public RegisterParkingRequestValidator()
    {
        RuleFor(request => request.RoomId)
            .NotEmpty().WithMessage("กรุณาระบุห้องของผู้เช่าเจ้าของรถ");

        RuleFor(request => request.TenantId)
            .NotEmpty().WithMessage("กรุณาระบุผู้เช่าเจ้าของรถ");

        RuleFor(request => request.LicensePlate)
            .NotEmpty().WithMessage("กรุณาระบุทะเบียนรถ")
            .MaximumLength(20).WithMessage("ทะเบียนรถต้องยาวไม่เกิน 20 ตัวอักษร");

        RuleFor(request => request.VehicleType)
            .Must(value => Enum.TryParse<VehicleType>(value, ignoreCase: true, out _))
            .WithMessage("ประเภทยานพาหนะไม่ถูกต้อง ต้องเป็น Car, Motorcycle หรือ Other");

        RuleFor(request => request.Brand)
            .MaximumLength(100).WithMessage("ยี่ห้อ/รุ่นรถต้องยาวไม่เกิน 100 ตัวอักษร");

        RuleFor(request => request.Color)
            .MaximumLength(50).WithMessage("สีรถต้องยาวไม่เกิน 50 ตัวอักษร");

        RuleFor(request => request.ParkingSpotLabel)
            .MaximumLength(50).WithMessage("ป้ายช่องจอดรถต้องยาวไม่เกิน 50 ตัวอักษร");

        RuleFor(request => request.Notes)
            .MaximumLength(1000).WithMessage("หมายเหตุต้องยาวไม่เกิน 1000 ตัวอักษร");
    }
}
