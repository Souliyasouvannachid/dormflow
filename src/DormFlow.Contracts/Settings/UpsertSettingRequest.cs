namespace DormFlow.Contracts.Settings;

/// <summary>ຮ້ອງຂໍໃຫ້ສ້າງ ຫຼືແກ້ໄຂການຕັ້ງຄ່າໃດໜຶ່ງ (Key ມາຈາກ Route)</summary>
public class UpsertSettingRequest
{
    /// <summary>ອົງກອນເປົ້າຝາຍຕ້ອງການພຽງແຕ່ຖ້າຜູ້ໂທແມ່ນ SuperAdmin ຜູ້ໃຊ້ອື່ນໆຖືກບັງຄັບໃຫ້ບັງຄັບໃຫ້ອົງກອນຂອງພວກເຂົາສະເຝີ</summary>
    public Guid? OrganizationId { get; set; }

    /// <summary>ການຕັ້ງຄ່າ</summary>
    public string Value { get; set; } = string.Empty;
}
