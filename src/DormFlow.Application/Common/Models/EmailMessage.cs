namespace DormFlow.Application.Common.Models;

/// <summary>ຜຶ່ງໃນອີເມວ</summary>
/// <param name="To">ອີເມວຜູ້ຮັບ</param>
/// <param name="Subject">ຫົວຂໍ້</param>
/// <param name="Body">ເນື້ອໃນຂໍ້ຄວາມທຳມະດາ</param>
public sealed record EmailMessage(string To, string Subject, string Body);
