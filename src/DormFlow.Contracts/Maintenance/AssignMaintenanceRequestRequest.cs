namespace DormFlow.Contracts.Maintenance;

/// <summary>ຮ້ອງຂໍມອບໜ້າທີ່ຊ່າງສ້ອມແປງວຽກງານສ້ອມແປງ</summary>
public class AssignMaintenanceRequestRequest
{
    /// <summary>ຊ່າງສ້ອມແປງທີ່ເຈົ້າຕ້ອງການມອບໝາຍຕ້ອງມີບົດບາດ Technician ແລະຖືກມອບໝາຍໃຫ້ເບິ່ງແຍງຫໍພັກນີ້</summary>
    public Guid TechnicianId { get; set; }
}
