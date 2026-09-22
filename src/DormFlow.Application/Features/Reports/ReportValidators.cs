using DormFlow.Contracts.Reports;
using FluentValidation;

namespace DormFlow.Application.Features.Reports;

/// <summary>ທົບທວນຄືນການຮ້ອງຂໍຂອງທ່ານສໍາລັບບົດລາຍງານລາຍໄດ້ສະຫຼຸບ</summary>
public sealed class RevenueReportRequestValidator : AbstractValidator<RevenueReportRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public RevenueReportRequestValidator()
    {
        RuleFor(request => request.PeriodTo)
            .GreaterThanOrEqualTo(request => request.PeriodFrom)
            .WithMessage("ເດືອນສິ້ນສຸດຕ້ອງບໍ່ຢູ່ກ່ອນເດືອນເລີ່ມຕົ້ນ");
    }
}
