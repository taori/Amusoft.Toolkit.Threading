using Shouldly;

namespace Amusoft.Toolkit.Threading.UnitTests.Tests;

public class LoaderIdentityTests
{
	private class FakeIdentity : LoaderIdentity
	{
		public override bool Equals(LoaderIdentity? other)
		{
			return false;
		}
	}

	[Fact]
	public void GuidIdentityEquals()
	{
		var identity = new GuidLoaderIdentity(Guid.Empty);
		identity.Equals(new FakeIdentity()).ShouldBeFalse();
	}
}