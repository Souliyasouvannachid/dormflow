using DormFlow.Domain.Common;
using DormFlow.Domain.Events;
using FluentAssertions;
using Xunit;

namespace DormFlow.UnitTests.Domain;

/// <summary>ตรวจพฤติกรรมพื้นฐานของ BaseEntity และ TenantEntity</summary>
public class BaseEntityTests
{
    [Fact]
    public void Entity_ที่สร้างใหม่_ต้องมี_Id_เป็น_UUID_v7()
    {
        TestEntity entity = new();

        entity.Id.Should().NotBe(Guid.Empty);
        entity.Id.Version.Should().Be(7);
    }

    [Fact]
    public void Entity_สองตัว_ต้องมี_Id_ไม่ซ้ำกัน()
    {
        TestEntity first = new();
        TestEntity second = new();

        first.Id.Should().NotBe(second.Id);
    }

    [Fact]
    public void Entity_ที่สร้างใหม่_ต้องยังไม่ถูกลบ()
    {
        TestEntity entity = new();

        entity.IsDeleted.Should().BeFalse();
        entity.DeletedAt.Should().BeNull();
        entity.DeletedBy.Should().BeNull();
    }

    [Fact]
    public void RaiseDomainEvent_ต้องเก็บเหตุการณ์ไว้ในคิว()
    {
        TestEntity entity = new();
        TestDomainEvent domainEvent = new();

        entity.RaiseDomainEvent(domainEvent);

        entity.DomainEvents.Should().ContainSingle().Which.Should().Be(domainEvent);
    }

    [Fact]
    public void RaiseDomainEvent_ต้องไม่รับค่า_null()
    {
        TestEntity entity = new();

        Action act = () => entity.RaiseDomainEvent(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ClearDomainEvents_ต้องล้างคิวจนหมด()
    {
        TestEntity entity = new();
        entity.RaiseDomainEvent(new TestDomainEvent());

        entity.ClearDomainEvents();

        entity.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void TenantEntity_ต้องเก็บขอบเขตองค์กรและหอพักได้()
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
