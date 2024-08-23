using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Amusoft.Toolkit.Threading;

/// <summary>
/// Base Task for debouncing purposes
/// </summary>
public abstract class DebounceTask
{
}

/// <summary>
/// Task used for debouncing of method execution
/// </summary>
/// <typeparam name="T"></typeparam>
public class DebounceTask<T> : DebounceTask, INotifyCompletion
{
	private Task<T> _loader;
	private CancellationTokenSource _cts = new();

	internal DebounceTask(Func<CancellationToken, Task<T>> loader)
	{
		_loader = loader(_cts.Token);
	}

	internal void CancelUpdate(Func<CancellationToken, Task<T>> loader)
	{
		_cts.Cancel(false);
		_cts = new();
		_loader = loader(_cts.Token);
	}
	
	/// <summary>
	/// Completion callback
	/// </summary>
	/// <param name="continuation"></param>
	public void OnCompleted(Action continuation)
	{
		_cts.Dispose();
		DebounceLoader.Unregister(this);
		continuation();
	}
	
	/// <summary>
	/// Awaiter of the task
	/// </summary>
	/// <returns></returns>
	public DebounceTask<T> GetAwaiter() { return this; }

	/// <summary>
	/// Whether the Task has completed
	/// </summary>
	public bool IsCompleted => _loader.IsCompleted;

	/// <summary>
	/// Result of the task
	/// </summary>
	/// <returns></returns>
	public T GetResult()
	{
		return _loader.Result;
	}
}