using System;

namespace Amusoft.Toolkit.Threading;

/// <summary>
/// Guid identity
/// </summary>
public class GuidLoaderIdentity : LoaderIdentity
{
	/// <summary>
	/// Identity Key
	/// </summary>
	public Guid Id { get; }
	
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="id"></param>
	public GuidLoaderIdentity(Guid id)
	{
		Id = id;
	}

	/// <summary>
	/// Equality comparison
	/// </summary>
	/// <param name="other"></param>
	/// <returns></returns>
	public override bool Equals(LoaderIdentity? other)
	{
		if (other is GuidLoaderIdentity guid)
			return guid.Id == Id;
		return false;
	}
}