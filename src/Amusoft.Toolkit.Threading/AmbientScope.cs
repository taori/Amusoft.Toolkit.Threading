using System;
using System.Collections.Generic;
using System.Threading;

#if NETSTANDARD2_0
using Amusoft.Toolkit.Threading.Compat;
#endif

namespace Amusoft.Toolkit.Threading;

/// <summary>
/// Ambient scope utility class
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class AmbientScope<T> : IDisposable
	where T: AmbientScope<T>
{
	private static readonly AsyncLocal<Stack<T>> GlobalScopes = new()
	{
		Value = new Stack<T>()
	};

	// ReSharper disable once InconsistentNaming
	private static readonly AsyncLocal<T?> GlobalCurrent = new ();
	
	/// <summary>
	/// Current ambient instance
	/// </summary>
	public static T? Current => GlobalCurrent.Value;

	private readonly AsyncLocal<T?> _localParentScope = new();
	
	/// <summary>
	/// Parent scope of current scope
	/// </summary>
	public T? ParentScope => _localParentScope.Value;

	/// <summary>
	/// Parent scopes of current scope
	/// </summary>
	public IEnumerable<T> GetParentScopes(Predicate<T>? continueCondition = default)
	{
		var current = this;
		while (current != null)
		{
			if (current.ParentScope == null || current._localParentScope.Value == null)
				break;

			if (continueCondition?.Invoke(current._localParentScope.Value) ?? true)
			{
				yield return current._localParentScope.Value;
				current = current._localParentScope.Value;
			}
		}
	}

	/// <summary>
	/// Constructor
	/// </summary>
	protected AmbientScope()
	{
		if (GlobalCurrent.Value != null)
		{
			_localParentScope.Value = GlobalCurrent.Value;
		}

		if (GlobalScopes.Value == null)
			GlobalScopes.Value = new Stack<T>();

		if (this is T c)
			GlobalScopes.Value.Push(c);
		ChangeCurrentScope(this as T);
	}

	private void ChangeCurrentScope(T? scope)
	{
		if (GlobalCurrent.Value == null)
		{
			if (scope is not null)
			{
				GlobalCurrent.Value = scope;
			}
		}
		else
		{
			GlobalCurrent.Value = scope;
		}
	}

	/// <summary>
	/// Dispose method
	/// </summary>
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	private bool _disposed;
	
	/// <summary>
	/// Dispose
	/// </summary>
	/// <param name="disposing">true if called by dispose, false if it is called by finalizer</param>
	protected virtual void Dispose(bool disposing)
	{
		if(_disposed)
			return;

		if (disposing)
		{
			if (GlobalScopes.Value!.TryPop(out var scope))
			{
				ChangeCurrentScope(scope.ParentScope);
			}
		}

		_disposed = true;
	}
}