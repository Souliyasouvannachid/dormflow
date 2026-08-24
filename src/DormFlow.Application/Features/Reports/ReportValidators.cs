using DormFlow.Contracts.Reports;
using FluentValidation;

namespace DormFlow.Application.Features.Reports;

/// <summary>ตรวจสอบคำขอรายงานสรุปรายได้</summary>
public sealed class RevenueReportRequestValidator : AbstractValidator<RevenueReportRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public RevenueReportRequestValidator()
    {
        RuleFor(request => request.PeriodTo)
            .GreaterThanOrEqualTo(request => request.PeriodFrom)
            .WithMessage("เดือนสิ้นสุดต้องไม่อยู่ก่อนเดือนเริ่มต้น");
    }
}
