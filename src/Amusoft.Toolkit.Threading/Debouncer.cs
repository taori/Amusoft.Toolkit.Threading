using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Amusoft.Toolkit.Threading.Compat;
using Amusoft.Toolkit.Threading.Exceptions;

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
/// <summary>
/// 
/// </summary>
public class Debouncer
{
	private static readonly ConcurrentDictionary<LoaderIdentity, DebounceStack> Stacks = new(new IdentityComparer());

	private static readonly ConditionalWeakTable<DebounceStack, LoaderIdentity> IdOfStack = new();
	
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public static async Task<T> FromStackAsync<T>(Func<CancellationToken, Task<T>> expression, LoaderIdentity identity)
	{
		var current = Stacks.GetOrAdd(identity,
			loaderIdentity =>
			{
				var newStack = new DebounceStack<T>();
				newStack.Task.ContinueWith(_ =>
					{
						if (Stacks.TryRemove(loaderIdentity, out var prevStack))
							IdOfStack.Remove(prevStack);
					}
				);
				IdOfStack.Add(newStack, loaderIdentity);
				return newStack;
			});

		if (current is not DebounceStack<T> stack)
			throw new TypeMismatchException(current.GetType(), typeof(DebounceStack<T>), identity);

		stack.CancelAndUpdate(expression);
		
		return await stack.Task;
	}
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
internal class DebounceStack<T> : DebounceStack
{
	private readonly TaskCompletionSource<T> _tcs = new();
	private CancellationTokenSource? _cts = new();
	private Task<T>? _loader;

	/// <summary>
	/// 
	/// </summary>
	internal Task<T> Task => _tcs.Task;

	internal void CancelAndUpdate(Func<CancellationToken, Task<T>> expression)
	{
		_cts?.Cancel(false);
		_cts?.Dispose();
		_cts = new();
		_loader = expression(_cts.Token);
		_loader.ContinueWith(r => _tcs.SetResult(r.Result));
	}
}

/// <summary>
/// 
/// </summary>
public abstract class DebounceStack
{
}