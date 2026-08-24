using DormFlow.Contracts.MeterReadings;
using DormFlow.Domain.Enums;
using FluentValidation;

namespace DormFlow.Application.Features.MeterReadings;

/// <summary>ตรวจสอบคำขอบันทึกค่ามิเตอร์</summary>
public sealed class CreateMeterReadingRequestValidator : AbstractValidator<CreateMeterReadingRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public CreateMeterReadingRequestValidator()
    {
        RuleFor(request => request.RoomId)
            .NotEmpty().WithMessage("กรุณาระบุห้องที่จดค่ามิเตอร์");

        RuleFor(request => request.MeterType)
            .Must(value => Enum.TryParse<MeterType>(value, ignoreCase: true, out _))
            .WithMessage("ประเภทมิเตอร์ไม่ถูกต้อง ต้องเป็น Water หรือ Electricity");

        RuleFor(request => request.ReadingDate)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("วันที่จดมิเตอร์ต้องไม่ใช่วันที่ในอนาคต");

        RuleFor(request => request.CurrentReading)
            .GreaterThanOrEqualTo(0).WithMessage("ค่ามิเตอร์ต้องไม่ติดลบ");

        RuleFor(request => request.Notes)
            .MaximumLength(1000).WithMessage("หมายเหตุต้องยาวไม่เกิน 1000 ตัวอักษร");
    }
}
