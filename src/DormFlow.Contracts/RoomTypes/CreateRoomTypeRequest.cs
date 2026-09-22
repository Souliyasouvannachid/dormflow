namespace DormFlow.Contracts.RoomTypes;

/// <summary>ຮ້ອງຂໍການສ້າງປະເພດຫ້ອງໃຝ່</summary>
public class CreateRoomTypeRequest
{
    /// <summary>ຫໍພັກທີ່ປະເພດຫ້ອງນີ້ຂຶ້ນກັບ</summary>
    public Guid DormitoryId { get; set; }

    /// <summary>ລະຫັດປະເພດຫ້ອງບໍ່ຊໍ້າກັນພາຍໃນຫໍພັກດຽວກັນ</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>ຊື່ປະເພດຫ້ອງ</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>ຄໍາອະທິບາຍເພີ່ມເຕີມ</summary>
    public string? Description { get; set; }

    /// <summary>ຄ່າເຊົ່າລາຍເດືອນ</summary>
    public decimal MonthlyRent { get; set; }

    /// <summary>ເຊົ່າລາຍວັນ ປ່ອຍຫວ່າງໄວ້ເມື່ອບໍ່ຮອງຮັບການເຊົ່າລາຍວັນ</summary>
    public decimal? DailyRent { get; set; }

    /// <summary>ຝາກ</summary>
    public decimal DepositAmount { get; set; }

    /// <summary>ຈຳນວນແຂກສູງສຸດ</summary>
    public int MaximumOccupants { get; set; } = 1;
}
