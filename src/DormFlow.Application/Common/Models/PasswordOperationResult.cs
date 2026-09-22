namespace DormFlow.Application.Common.Models;

/// <summary>ຜົນໄດ້ຮັບຂອງການດໍາເນີນງານລະຫັດຜ່ານ</summary>
/// <param name="Succeeded">ປະສົບຜົນສໍາເລັດຫຼືບໍ່?</param>
/// <param name="Errors">ຂໍ້ຄວາມອະທິບາຍເຫດຜົນສໍາລັບຄວາມລົ້ມເຫຼວ</param>
public sealed record PasswordOperationResult(bool Succeeded, IReadOnlyList<string> Errors)
{
    /// <summary>ຜົນໄດ້ຮັບທີ່ປະສົບຜົນສໍາເລັດ</summary>
    public static PasswordOperationResult Success { get; } = new(true, []);

    /// <summary>ຜະລິດຜົນໄດ້ຮັບທີ່ບໍ່ສໍາເລັດ</summary>
    /// <param name="errors">ຂໍ້ຄວາມອະທິບາຍສາເຫດ</param>
    public static PasswordOperationResult Failed(params string[] errors) => new(false, errors);
}
