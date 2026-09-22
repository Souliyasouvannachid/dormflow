using System.Globalization;
using FluentValidation.Results;

namespace DormFlow.Application.Common.Exceptions;

/// <summary>
/// ຂໍ້ມູນທີ່ສົ່ງມາຍັງບໍ່ໄດ້ຮັບການຢືນຢັນ API ຈະປ່ຽນເປັນ HTTP 400 ມີລາຍລະອຽດຕໍ່ຊ່ອງຂໍ້ມູນ
/// </summary>
/// <remarks>
/// ຊື່ມັນ ValidationFailedException ໃນຄໍາສັ່ງທີ່ຈະບໍ່ collide ກັບ FluentValidation.ValidationException
/// ເຊິ່ງຈະຮຽກຮ້ອງໃຫ້ທ່ານເຂົ້າໄປໃນ namespace ເຕັມທີ່ທຸກຄັ້ງທີ່ກ່າວເຖິງ
/// </remarks>
public class ValidationFailedException : Exception
{
    private static readonly IReadOnlyDictionary<string, string[]> NoErrors =
        new Dictionary<string, string[]>(StringComparer.Ordinal);

    /// <summary>ສ້າງ Exception ດ້ວຍຂໍ້ຄວາມເລີ່ມຕົ້ນ ແລະບໍ່ມີລາຍລະອຽດໃນຊ່ອງຂໍ້ມູນ</summary>
    public ValidationFailedException()
        : base("ຂໍ້ມູນບໍ່ຖືກຕ້ອງ")
    {
        Errors = NoErrors;
    }

    /// <summary>ສ້າງ Exception ກັບຂໍ້ຄວາມທີ່ກໍາຜົດເອງ</summary>
    /// <param name="message">ຂໍ້ຄວາມອະທິບາຍສາເຫດ</param>
    public ValidationFailedException(string message)
        : base(message)
    {
        Errors = NoErrors;
    }

    /// <summary>ສ້າງ Exception ກັບຂໍ້ຄວາມແລະ Exception ຕົ້ນກໍາເນີດ</summary>
    /// <param name="message">ຂໍ້ຄວາມອະທິບາຍສາເຫດ</param>
    /// <param name="innerException">Exception ຕົ້ນກໍາເນີດ</param>
    public ValidationFailedException(string message, Exception innerException)
        : base(message, innerException)
    {
        Errors = NoErrors;
    }

    /// <summary>ສ້າງ Exception ຈາກຜົນການກວດກາຂອງ ສ FluentValidation</summary>
    /// <param name="failures">ກວດພົບລາຍຊື່ຂໍ້ຜິດພາດ</param>
    public ValidationFailedException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        ArgumentNullException.ThrowIfNull(failures);

        Errors = failures
            .GroupBy(failure => ToCamelCase(failure.PropertyName), StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group.Select(failure => failure.ErrorMessage).Distinct(StringComparer.Ordinal).ToArray(),
                StringComparer.Ordinal);
    }

    /// <summary>
    /// ລາຍລະອຽດຄວາມຜິດພາດພາກສະຜາມໂດຍພາກສະຜາມ
    /// key ແມ່ນຊື່ພາກສະຜາມເຊັ່ນ camelCase ເພື່ອໃຫ້ກົງກັບຮູບແບບ JSON ທີ່ Client ສົ່ງແລ້ວ
    /// </summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    private static string ToCamelCase(string propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
        {
            return propertyName;
        }

        // ສະຜັບສະຜູນຊື່ຄູ່, ເຊັ່ນ: Address.PostalCode -> address.postalCode
        string[] segments = propertyName.Split('.');
        for (int i = 0; i < segments.Length; i++)
        {
            string segment = segments[i];
            if (segment.Length > 0 && char.IsUpper(segment[0]))
            {
                segments[i] = string.Create(CultureInfo.InvariantCulture, $"{char.ToLowerInvariant(segment[0])}{segment[1..]}");
            }
        }

        return string.Join('.', segments);
    }
}
