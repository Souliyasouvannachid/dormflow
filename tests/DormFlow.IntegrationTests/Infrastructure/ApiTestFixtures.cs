using Xunit;

namespace DormFlow.IntegrationTests.Infrastructure;

/// <summary>
/// รวมทุกเทสต์ที่ต้องใช้ Test Host เดียวกันไว้ในคอลเลกชันเดียว
/// เพื่อให้สร้างคอนเทนเนอร์ PostgreSQL เพียงครั้งเดียวต่อการรันหนึ่งรอบ
/// </summary>
[CollectionDefinition(Name)]
public sealed class ApiTestFixtures : ICollectionFixture<DormFlowApiFactory>
{
    /// <summary>ชื่อคอลเลกชันที่ใช้กำกับคลาสเทสต์</summary>
    public const string Name = "DormFlow API";
}
