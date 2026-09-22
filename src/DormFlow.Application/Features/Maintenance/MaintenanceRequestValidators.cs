using DormFlow.Contracts.Maintenance;
using DormFlow.Domain.Enums;
using FluentValidation;

namespace DormFlow.Application.Features.Maintenance;

/// <summary>ກວດສອບການຮ້ອງຂໍການບໍາລຸງຮັກສາ</summary>
public sealed class CreateMaintenanceRequestRequestValidator : AbstractValidator<CreateMaintenanceRequestRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public CreateMaintenanceRequestRequestValidator()
    {
        RuleFor(request => request.RoomId)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຫ້ອງທີ່ແຈ້ງສ້ອມແປງ");

        RuleFor(request => request.Title)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຫົວຂໍ້ຂອງງານສ້ອມແປງ")
            .MaximumLength(200).WithMessage("ຫົວຂໍ້ຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 200 ໂຕອັກສອນ");

        RuleFor(request => request.Description)
            .MaximumLength(2000).WithMessage("ລາຍລະອຽດຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 2000 ໂຕອັກສອນ");

        RuleFor(request => request.Priority)
            .Must(value => Enum.TryParse<MaintenancePriority>(value, ignoreCase: true, out _))
            .WithMessage("ລະດັບຄວາມສຳຄັນບໍ່ຖືກຕ້ອງ ຕ້ອງເປັນ Low, Medium, High ຫຼື Urgent");
    }
}

/// <summary>ທົບທວນຄືນຄຳຮ້ອງຂໍການມອບໝາຍຊ່າງສ້ອມແປງ</summary>
public sealed class AssignMaintenanceRequestRequestValidator : AbstractValidator<AssignMaintenanceRequestRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public AssignMaintenanceRequestRequestValidator()
    {
        RuleFor(request => request.TechnicianId)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຊ່າງສ້ອມແປງທີ່ຕ້ອງການມອບໝາຍ");
    }
}
