using System;

namespace Amusoft.Toolkit.Threading.Exceptions;

/// <summary>
/// 
/// </summary>
public class TypeMismatchException : Exception
{
	/// <summary>
	/// 
	/// </summary>
	public Type Actual { get; }
	
	/// <summary>
	/// 
	/// </summary>
	public Type Expected { get; }
	
	/// <summary>
	/// 
	/// </summary>
	public LoaderIdentity Identity { get; }

	/// <summary>
	/// 
	/// </summary>
	/// <param name="actual"></param>
	/// <param name="expected"></param>
	/// <param name="identity"></param>
	public TypeMismatchException(Type actual, Type expected, LoaderIdentity identity)
	{
		Actual = actual;
		Expected = expected;
		Identity = identity;
	}
}