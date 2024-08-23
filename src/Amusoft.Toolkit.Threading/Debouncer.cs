using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Amusoft.Toolkit.Threading.Compat;

namespace Amusoft.Toolkit.Threading;

/// <summary>
/// 
/// </summary>
public class Debouncer
{
	private static readonly ConcurrentDictionary<LoaderIdentity, DebounceStack> Stacks = new();

	private static readonly ConditionalWeakTable<DebounceStack, LoaderIdentity> IdOfStack = new();
	
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public static Task<T> FromStackAsync<T>(Func<CancellationToken, Task<T>> expression, LoaderIdentity identity)
	{
		var current = Stacks.GetOrAdd(identity,
			loaderIdentity =>
			{
				var newStack = new DebounceStack<T>();
				newStack.Task.ContinueWith(_ =>
					{
						Stacks.TryRemove(loaderIdentity, out var prevStack);
						IdOfStack.Remove(prevStack);
					}
				);
				IdOfStack.Add(newStack, loaderIdentity);
				return newStack;
			});

		if (current is not DebounceStack<T> stack)
			throw new Exception($"Task is expected to be of type {typeof(DebounceTask<T>).FullName} but isn't. Check the identity you are using for accidentally similarities");

		stack.CancelAndUpdate(expression);
		
		return stack.Task;
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