using DormFlow.Domain.Common;
using DormFlow.Domain.Events;
using FluentAssertions;
using Xunit;

namespace DormFlow.UnitTests.Domain;

/// <summary>ກວດສອບພຶດຕິກໍາພື້ນຖານຂອງ BaseEntity ແລະ TenantEntity</summary>
public class BaseEntityTests
{
    [Fact]
    public void Entity_ສ້າງໃໝ່_ຕ້ອງມີ_Id_ແມ່ນ_UUID_v7()
    {
        TestEntity entity = new();

        entity.Id.Should().NotBe(Guid.Empty);
        entity.Id.Version.Should().Be(7);
    }

    [Fact]
    public void Entity_ສອງ_ຕ້ອງມີ_Id_ເປັນເອກະລັກ()
    {
        TestEntity first = new();
        TestEntity second = new();

        first.Id.Should().NotBe(second.Id);
    }

    [Fact]
    public void Entity_ສ້າງໃໝ່_ບໍ່ຄວນຖືກລຶບ()
    {
        TestEntity entity = new();

        entity.IsDeleted.Should().BeFalse();
        entity.DeletedAt.Should().BeNull();
        entity.DeletedBy.Should().BeNull();
    }

    [Fact]
    public void RaiseDomainEvent_ເຫດການຕ້ອງຖືກເກັບໄວ້ໃນແຖວ()
    {
        TestEntity entity = new();
        TestDomainEvent domainEvent = new();

        entity.RaiseDomainEvent(domainEvent);

        entity.DomainEvents.Should().ContainSingle().Which.Should().Be(domainEvent);
    }

    [Fact]
    public void RaiseDomainEvent_ທ່ານຕ້ອງບໍ່ຍອມຮັບຄ່າທໍານຽມ_null()
    {
        TestEntity entity = new();

        Action act = () => entity.RaiseDomainEvent(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ClearDomainEvents_ແຖວຕ້ອງຖືກລຶບລ້າງໃຫ້ໝົດ()
    {
        TestEntity entity = new();
        entity.RaiseDomainEvent(new TestDomainEvent());

        entity.ClearDomainEvents();

        entity.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void TenantEntity_ຕ້ອງສາມາດຮັກສາຂອບເຂດຂອງອົງກອນແລະຫໍພັກ()
    {
        Guid organizationId = Guid.CreateVersion7();
        Guid dormitoryId = Guid.CreateVersion7();

        TestTenantEntity entity = new()
        {
            OrganizationId = organizationId,
            DormitoryId = dormitoryId,
        };

        entity.OrganizationId.Should().Be(organizationId);
        entity.DormitoryId.Should().Be(dormitoryId);
        entity.Should().BeAssignableTo<BaseEntity>();
        entity.Should().BeAssignableTo<ITenantScoped>();
    }

    private sealed class TestEntity : BaseEntity;

    private sealed class TestTenantEntity : TenantEntity;

    private sealed record TestDomainEvent : DomainEvent;
}
