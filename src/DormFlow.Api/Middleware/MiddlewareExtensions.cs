namespace DormFlow.Api.Middleware;

/// <summary>ສ່ວນຂະຫຍາຍສໍາລັບການເຊື່ອມຕໍ່ Middleware ຂອງ DormFlow ເຂົ້າໄປໃນທໍ່ນັ້ນ</summary>
public static class MiddlewareExtensions
{
    /// <summary>
    /// ເພີ່ມຕົວຈັບຄວາມຜິດພາດສູນກາງຂອງລະບົບແລ້ວ
    /// ມັນຕ້ອງຖືກເອີ້ນວ່າທໍາອິດໃນທໍ່ສໍາລັບການຄຸ້ມຄອງ Middleware ສ່ວນທີ່ເຫຼືອທັງຝົດ
    /// </summary>
    /// <param name="app">ທໍ່ຄໍາຮ້ອງສະຝັກ</param>
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        return app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
    }
}
