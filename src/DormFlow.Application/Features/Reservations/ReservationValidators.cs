using DormFlow.Contracts.Reservations;
using FluentValidation;

namespace DormFlow.Application.Features.Reservations;

/// <summary>ตรวจสอบคำขอจองห้องพัก</summary>
public sealed class CreateReservationRequestValidator : AbstractValidator<CreateReservationRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public CreateReservationRequestValidator()
    {
        RuleFor(request => request.RoomId)
            .NotEmpty().WithMessage("กรุณาระบุห้องที่ต้องการจอง");

        RuleFor(request => request.TenantId)
            .NotEmpty().WithMessage("กรุณาระบุผู้เช่าที่จอง");

        RuleFor(request => request.MoveInDate)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("วันที่คาดว่าจะเข้าพักต้องไม่ใช่วันที่ผ่านมาแล้ว");

        RuleFor(request => request.HoldDays)
            .InclusiveBetween(1, 30).WithMessage("จำนวนวันที่กันห้องต้องอยู่ระหว่าง 1 ถึง 30 วัน");

        RuleFor(request => request.ReservationFee)
            .GreaterThanOrEqualTo(0).WithMessage("ค่าจองห้องต้องไม่ติดลบ");

        RuleFor(request => request.Notes)
            .MaximumLength(1000).WithMessage("หมายเหตุต้องยาวไม่เกิน 1000 ตัวอักษร");
    }
}
