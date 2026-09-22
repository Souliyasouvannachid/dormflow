namespace DormFlow.Contracts.Common;

/// <summary>
/// ການຮ້ອງຂໍການປ່ຽນແປງສະຖານະການນໍາໃຊ້ຮ່ວມກັບທັງຝົດ Entity ກັບຄໍາສັ່ງ PATCH .../status
/// </summary>
/// <typeparam name="TStatus">ປະເພດຂອງ Enum ສະຖານະຂອງ Entity ນັ້ນ</typeparam>
public class ChangeStatusRequest<TStatus>
    where TStatus : struct, Enum
{
    /// <summary>ສະຖານະໃໝ່ທີ່ທ່ານຕ້ອງການຕັ້ງ</summary>
    public TStatus Status { get; set; }
}
