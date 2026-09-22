namespace DormFlow.Contracts.Rooms;

/// <summary>ສະຫຼຸບຂອງຈໍານວນຫ້ອງທີ່ແຍກຕາມສະຖານະ</summary>
public class RoomSummaryResponse
{
    /// <summary>ຈຳນວນຫ້ອງທັງໝົດທີ່ຕອບສະໜອງເງື່ອນໄຂ</summary>
    public int TotalRooms { get; init; }

    /// <summary>ຈຳນວນຫ້ອງທີ່ໃຫ້ເຊົ່າ</summary>
    public int Available { get; init; }

    /// <summary>ຈຳນວນຫ້ອງທີ່ຈອງໄວ້ແລ້ວ</summary>
    public int Reserved { get; init; }

    /// <summary>ຈຳນວນຫ້ອງທີ່ຜູ້ເຊົ່າຄອບຄອງ</summary>
    public int Occupied { get; init; }

    /// <summary>ຈຳນວນຫ້ອງທີ່ກຳລັງຖືກອະນາໄມ</summary>
    public int Cleaning { get; init; }

    /// <summary>ຈຳນວນຫ້ອງທີ່ກຳລັງຢູ່ໃນການບຳລຸງຮັກສາ</summary>
    public int Maintenance { get; init; }

    /// <summary>ຈຳນວນຫ້ອງທີ່ຖືກປິດການນຳໃຊ້</summary>
    public int Inactive { get; init; }
}
