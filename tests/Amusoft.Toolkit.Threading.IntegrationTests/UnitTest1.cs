using System.Data.Common;
using Amusoft.XUnit.NLog.Extensions;
using Amusoft.Toolkit.Threading.Tests.Shared;
using Amusoft.Toolkit.Threading.Tests.Shared.Toolkit;
using Amusoft.Toolkit.Threading.UnitTests;
using Xunit;
using Xunit.Abstractions;
using NLog.Fluent;

namespace Amusoft.Toolkit.Threading.IntegrationTests;

public class UnitTest1 : TestBase
{
    [Fact]
    public void Test1()
    {
        Log.Error("test");
    }

    public UnitTest1(ITestOutputHelper outputHelper, AssemblyInitializer data) : base(outputHelper, data)
    {
    }
}