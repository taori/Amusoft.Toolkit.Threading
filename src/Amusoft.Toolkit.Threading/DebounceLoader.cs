using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
#if NETSTANDARD2_0
using Amusoft.Toolkit.Threading.Compat;
#endif

namespace Amusoft.Toolkit.Threading;

/// <summary>
/// Core API to create a Task which can debounce execution with the latest result of a call
/// </summary>
public static class DebounceLoader
{
	private static readonly ConcurrentDictionary<LoaderIdentity, DebounceTask> Tasks = new();

	private static readonly ConditionalWeakTable<DebounceTask, LoaderIdentity> IdOfTask = new();

	internal static bool IsLoaderIdle => Tasks.Count == 0; 
	
	/// <summary>
	/// Generates an awaitable Task based on the given delegate
	/// </summary>
	/// <param name="loader"></param>
	/// <param name="identity"></param>
	/// <typeparam name="TResult"></typeparam>
	/// <returns></returns>
	public static DebounceTask<TResult> From<TResult>(Func<CancellationToken, Task<TResult>> loader, LoaderIdentity identity)
	{
		var task = Tasks.GetOrAdd(identity,
			loaderIdentity =>
			{
				if (Tasks.TryGetValue(loaderIdentity, out var debounceTask) && debounceTask is DebounceTask<TResult> castedTask)
				{
					castedTask.CancelUpdate(loader);
					return Tasks[loaderIdentity];
				}

				var t = new DebounceTask<TResult>(loader);
				IdOfTask.Add(t, identity);
				return t;
			});
		return (DebounceTask<TResult>)task;
	}

	internal static void Unregister<T>(DebounceTask<T> debounceTask)
	{
		if (IdOfTask.TryGetValue(debounceTask, out var identity))
			Tasks.Remove(identity, out _);
	}
}