namespace DormFlow.Contracts.Leases;

/// <summary>ຮ້ອງຂໍສ້າງສັນຍາເຊົ່າໃຝ່</summary>
public class CreateContractRequest
{
    /// <summary>ຫ້ອງເພື່ອເຮັດສັນຍາ</summary>
    public Guid RoomId { get; set; }

    /// <summary>ຜູ້ເຊົ່າຕາມສັນຍາ</summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// ການຈອງໄດ້ນໍາເອົາເຂົ້າໃນສັນຍານີ້ຫວ່າງເປົ່າໃນເວລາທີ່ເຮັດສັນຍາໂດຍບໍ່ມີການຈອງລ່ວງຜ້າ
    /// ເມື່ອລະບຸໄວ້, ຫ້ອງ ແລະຜູ້ເຊົ່າຂອງການຈອງຕ້ອງກົງກັບທີ່ລະບຸໄວ້ໃນຄໍາຮ້ອງຂໍນີ້
    /// </summary>
    public Guid? ReservationId { get; set; }

    /// <summary>ວັນທີເລີ່ມຕົ້ນຂອງສັນຍາ</summary>
    public DateOnly StartDate { get; set; }

    /// <summary>ກຳນົດວັນທີສິ້ນສຸດສັນຍາ</summary>
    public DateOnly EndDate { get; set; }

    /// <summary>ເຊົ່າລາຍເດືອນ ປ່ອຍຫວ່າງໄວ້ເພື່ອໃຊ້ຄ່າເຊົ່າຂອງຫ້ອງເປັນຄ່າເລີ່ມຕົ້ນ</summary>
    public decimal? MonthlyRent { get; set; }

    /// <summary>ເງິນຝາກປະໄວ້ຫວ່າງເປົ່າເພື່ອໃຊ້ເງິນຝາກປະຈຸບັນຂອງຫ້ອງເປັນຄ່າເລີ່ມຕົ້ນ</summary>
    public decimal? DepositAmount { get; set; }

    /// <summary>ບັນທຶກເພີ່ມເຕີມ</summary>
    public string? Notes { get; set; }
}
