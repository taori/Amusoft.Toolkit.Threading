using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Amusoft.Toolkit.Threading.Compat;
using Amusoft.Toolkit.Threading.Exceptions;

namespace Amusoft.Toolkit.Threading;

/// <summary>
/// Debouncing API
/// </summary>
public class Debouncer
{
	private static readonly ConcurrentDictionary<LoaderIdentity, DebounceStack> Stacks = new(new IdentityComparer());

	private static readonly ConditionalWeakTable<DebounceStack, LoaderIdentity> IdOfStack = new();
	
	/// <summary>
	/// ensures that the last call with a given identity will provide the result for all awaiting tasks
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