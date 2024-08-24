using System.Collections.Generic;

namespace Amusoft.Toolkit.Threading;

internal class IdentityComparer : IEqualityComparer<LoaderIdentity>
{
	public bool Equals(LoaderIdentity? x, LoaderIdentity? y)
	{
		return x?.Equals(y) ?? false;
	}

	public int GetHashCode(LoaderIdentity obj)
	{
		return obj.GetHashCode();
	}
}