using Xunit;

namespace DormFlow.IntegrationTests.Infrastructure;

/// <summary>
/// ລວມມີການທົດສອບທັງໝົດທີ່ຕ້ອງການ Test Host ດຽວກັນຢູ່ໃນຄໍເລັກຊັນດຽວ
/// ເພື່ອສ້າງຖັງ PostgreSQL ພຽງແຕ່ຄັ້ງດຽວຕໍ່ການແລ່ນ
/// </summary>
[CollectionDefinition(Name)]
public sealed class ApiTestFixtures : ICollectionFixture<DormFlowApiFactory>
{
    /// <summary>ຊື່ຂອງຄໍເລັກຊັນທີ່ໃຊ້ເພື່ອຕິດປ້າຍຫ້ອງສອບເສັງ</summary>
    public const string Name = "DormFlow API";
}
