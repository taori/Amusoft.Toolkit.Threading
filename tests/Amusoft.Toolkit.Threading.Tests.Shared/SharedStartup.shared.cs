using System.Runtime.CompilerServices;
using Amusoft.Toolkit.Threading.Tests.Shared.Initializers;
using Amusoft.XUnit.NLog.Extensions;
using Xunit.Abstractions;

namespace Amusoft.Toolkit.Threading.UnitTests;

public class SharedStartup
{
	[ModuleInitializer]
	public static void Initialize()
	{
		VerifyInitializer.Initialize();
	}
}

[Collection("AssemblyInitializer")]
public class TestBase : LoggedTestBase
{
	private readonly AssemblyInitializer _data;

	protected TestBase(ITestOutputHelper outputHelper, AssemblyInitializer data) : base(outputHelper)
	{
		_data = data;
		XUnitOutputTarget.OutputHelper = outputHelper;
	}
}

[CollectionDefinition("AssemblyInitializer", DisableParallelization = false)]
public class AssemblyInitializer: IAsyncLifetime, ICollectionFixture<AssemblyInitializer>
{
	public Task InitializeAsync()
	{
		return Task.CompletedTask;
	}

	public Task DisposeAsync()
	{
		return Task.CompletedTask;
	}
}