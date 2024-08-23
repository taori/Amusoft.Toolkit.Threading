using System;

namespace Amusoft.Toolkit.Threading;

/// <summary>
/// The identity of a loader
/// </summary>
public abstract class LoaderIdentity : IEquatable<LoaderIdentity>
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="other"></param>
	/// <returns></returns>
	public abstract bool Equals(LoaderIdentity? other);
}