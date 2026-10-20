using System.ComponentModel.DataAnnotations;

namespace DormFlow.Infrastructure.Options;

/// <summary>ການຕັ້ງຄ່າການສົ່ງອີເມວອ່ານຈາກ configuration section ຊື່ Email</summary>
public sealed class EmailOptions
{
    /// <summary>ຊື່ section ໃນ appsettings</summary>
    public const string SectionName = "Email";

    /// <summary>Email provider: Smtp, Disabled, or PickupDirectory for development only.</summary>
    public string Provider { get; set; } = "PickupDirectory";

    /// <summary>ສົ່ງອີເມວ</summary>
    [Required]
    [EmailAddress]
    public string FromAddress { get; set; } = "no-reply@dormflow.local";

    /// <summary>ຊື່ຜູ້ສົ່ງສະແດງຢູ່ໃນອີເມວ</summary>
    public string FromName { get; set; } = "DormFlow";

    /// <summary>
    /// ໂຟນເດີທີ່ໃຊ້ເພື່ອເກັບຮັກສາອີເມວຂາອອກແທນທີ່ຈະສົ່ງພວກມັນຕົວຈິງ
    /// ເສັ້ນທາງທີ່ກ່ຽວຂ້ອງແມ່ນເອົາມາຈາກໂຟນເດີທີ່ແອັບພລິເຄຊັນກໍາລັງແລ່ນຢູ່
    /// </summary>
    [Required]
    public string PickupDirectory { get; set; } = "logs/emails";

    /// <summary>ຊື່ host SMTP ເມື່ອ Provider ເປັນ Smtp</summary>
    public string? SmtpHost { get; set; }

    /// <summary>port SMTP</summary>
    [Range(1, 65535)]
    public int SmtpPort { get; set; } = 587;

    /// <summary>ເປີດ TLS ສຳລັບ SMTP</summary>
    public bool SmtpUseSsl { get; set; } = true;

    /// <summary>ຊື່ຜູ້ໃຊ້ SMTP</summary>
    public string? SmtpUsername { get; set; }

    /// <summary>ລະຫັດຜ່ານ SMTP</summary>
    public string? SmtpPassword { get; set; }
}
