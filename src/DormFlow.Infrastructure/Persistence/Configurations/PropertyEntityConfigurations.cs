using DormFlow.Domain.Entities;
using DormFlow.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DormFlow.Infrastructure.Persistence.Configurations;

/// <summary>ຕັ້ງຄ່າຕາຕະລາງຫໍພັກ</summary>
public sealed class DormitoryConfiguration : IEntityTypeConfiguration<Dormitory>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Dormitory> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("dormitories");

        builder.Property(dormitory => dormitory.Code).HasMaxLength(50).IsRequired();
        builder.Property(dormitory => dormitory.Name).HasMaxLength(200).IsRequired();
        builder.Property(dormitory => dormitory.Description).HasMaxLength(1000);
        builder.Property(dormitory => dormitory.Phone).HasMaxLength(30);
        builder.Property(dormitory => dormitory.Email).HasMaxLength(256);
        builder.Property(dormitory => dormitory.Address).HasMaxLength(500);
        builder.Property(dormitory => dormitory.Province).HasMaxLength(100);
        builder.Property(dormitory => dormitory.District).HasMaxLength(100);
        builder.Property(dormitory => dormitory.Subdistrict).HasMaxLength(100);
        builder.Property(dormitory => dormitory.PostalCode).HasMaxLength(10);
        builder.Property(dormitory => dormitory.Latitude).HasPrecision(9, 6);
        builder.Property(dormitory => dormitory.Longitude).HasPrecision(9, 6);
        builder.Property(dormitory => dormitory.LateFeeType).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(dormitory => dormitory.Status).HasConversion<string>().HasMaxLength(20).IsRequired();

        // ລະຫັດຫໍພັກຕ້ອງເປັນເອກະລັກພາຍໃນອົງກອນດຽວກັນ. ມີແຕ່ແຖວທີ່ຍັງບໍ່ໄດ້ລຶບ
        builder.HasIndex(dormitory => new { dormitory.OrganizationId, dormitory.Code })
            .IsUnique()
            .HasFilter("is_deleted = false");

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(dormitory => dormitory.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>ຕັ້ງຄ່າຕາຂ່າຍໄຟຟ້າ</summary>
public sealed class BuildingConfiguration : IEntityTypeConfiguration<Building>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Building> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("buildings");

        builder.Property(building => building.DormitoryId).IsRequired();
        builder.Property(building => building.Code).HasMaxLength(50).IsRequired();
        builder.Property(building => building.Name).HasMaxLength(200).IsRequired();
        builder.Property(building => building.Description).HasMaxLength(1000);
        builder.Property(building => building.Status).HasConversion<string>().HasMaxLength(20).IsRequired();

        // ລະຫັດອາຄານຕ້ອງເປັນເອກະລັກພາຍໃນຫໍພັກດຽວກັນ. ມີແຕ່ແຖວທີ່ຍັງບໍ່ໄດ້ລຶບ
        builder.HasIndex(building => new { building.DormitoryId, building.Code })
            .IsUnique()
            .HasFilter("is_deleted = false");

        builder.HasOne<Dormitory>()
            .WithMany()
            .HasForeignKey(building => building.DormitoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>ຕັ້ງຄ່າຕາຂ່າຍໄຟຟ້າຊັ້ນອາຄານ</summary>
public sealed class FloorConfiguration : IEntityTypeConfiguration<Floor>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Floor> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("floors");

        builder.Property(floor => floor.DormitoryId).IsRequired();
        builder.Property(floor => floor.Name).HasMaxLength(100);
        builder.Property(floor => floor.Description).HasMaxLength(1000);
        builder.Property(floor => floor.Status).HasConversion<string>().HasMaxLength(20).IsRequired();

        // ຕົວເລກຊັ້ນຕ້ອງເປັນເອກະລັກພາຍໃນອາຄານດຽວກັນ. ມີແຕ່ແຖວທີ່ຍັງບໍ່ໄດ້ລຶບ
        builder.HasIndex(floor => new { floor.BuildingId, floor.FloorNumber })
            .IsUnique()
            .HasFilter("is_deleted = false");

        builder.HasOne<Building>()
            .WithMany()
            .HasForeignKey(floor => floor.BuildingId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>ຕັ້ງຄ່າຕາຕະລາງປະເພດຫ້ອງ</summary>
public sealed class RoomTypeConfiguration : IEntityTypeConfiguration<RoomType>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<RoomType> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("room_types");

        builder.Property(roomType => roomType.DormitoryId).IsRequired();
        builder.Property(roomType => roomType.Code).HasMaxLength(50).IsRequired();
        builder.Property(roomType => roomType.Name).HasMaxLength(200).IsRequired();
        builder.Property(roomType => roomType.Description).HasMaxLength(1000);
        builder.Property(roomType => roomType.Status).HasConversion<string>().HasMaxLength(20).IsRequired();

        // ລະຫັດປະເພດຫ້ອງຈະຕ້ອງບໍ່ຊໍ້າກັນພາຍໃນຫໍພັກດຽວກັນ. ມີແຕ່ແຖວທີ່ຍັງບໍ່ໄດ້ລຶບ
        builder.HasIndex(roomType => new { roomType.DormitoryId, roomType.Code })
            .IsUnique()
            .HasFilter("is_deleted = false");

        builder.HasOne<Dormitory>()
            .WithMany()
            .HasForeignKey(roomType => roomType.DormitoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>ຕັ້ງຄ່າຕາຕະລາງຫ້ອງ</summary>
public sealed class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("rooms");

        builder.Property(room => room.DormitoryId).IsRequired();
        builder.Property(room => room.RoomNumber).HasMaxLength(20).IsRequired();
        builder.Property(room => room.Description).HasMaxLength(1000);
        builder.Property(room => room.Status).HasConversion<string>().HasMaxLength(20).IsRequired();

        // ໃນອັດຕາສ່ວນ (0-100) ສະນັ້ນຕັ້ງຄວາມລະອຽດດ້ວຍຕົນເອງ, ຢ່າໃຊ້ມັນ numeric(18,2) ປະຕິບັດຕາມ convention ທົ່ວໄປ
        builder.Property(room => room.FloorPlanX).HasPrecision(5, 2);
        builder.Property(room => room.FloorPlanY).HasPrecision(5, 2);
        builder.Property(room => room.FloorPlanWidth).HasPrecision(5, 2);
        builder.Property(room => room.FloorPlanHeight).HasPrecision(5, 2);

        // ຕົວເລກຫ້ອງຈະຕ້ອງບໍ່ຊໍ້າກັນພາຍໃນອາຄານດຽວກັນ. ມີແຕ່ແຖວທີ່ຍັງບໍ່ໄດ້ລຶບ
        builder.HasIndex(room => new { room.BuildingId, room.RoomNumber })
            .IsUnique()
            .HasFilter("is_deleted = false");

        builder.HasIndex(room => room.FloorId);
        builder.HasIndex(room => room.RoomTypeId);
        builder.HasIndex(room => room.Status);

        builder.HasOne<Building>()
            .WithMany()
            .HasForeignKey(room => room.BuildingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Floor>()
            .WithMany()
            .HasForeignKey(room => room.FloorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<RoomType>()
            .WithMany()
            .HasForeignKey(room => room.RoomTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>ຕັ້ງຄ່າກຳນົດເວລາການມອບໝາຍຜູ້ໃຊ້ຫໍພັກ</summary>
public sealed class UserDormitoryConfiguration : IEntityTypeConfiguration<UserDormitory>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<UserDormitory> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("user_dormitories");

        builder.HasKey(assignment => assignment.Id);

        builder.Property(assignment => assignment.AssignedAt).IsRequired();

        // ຜູ້ໃຊ້ໜຶ່ງຄົນບໍ່ສາມາດຖືກມອບໝາຍໃຫ້ຢູ່ໃນຫໍພັກດຽວກັນໄດ້
        builder.HasIndex(assignment => new { assignment.UserId, assignment.DormitoryId }).IsUnique();
        // ໃຫ້ມີຫໍພັກຫຼັກໄດ້ພຽງແຫ່ງດຽວຕໍ່ຜູ້ໃຊ້ ແມ້ຈະມີການມອບໝາຍພ້ອມກັນ.
        builder.HasIndex(assignment => assignment.UserId)
            .IsUnique()
            .HasDatabaseName("ix_user_dormitories_one_primary_per_user")
            .HasFilter("is_primary = true");
        builder.HasIndex(assignment => assignment.DormitoryId);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(assignment => assignment.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Dormitory>()
            .WithMany()
            .HasForeignKey(assignment => assignment.DormitoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
