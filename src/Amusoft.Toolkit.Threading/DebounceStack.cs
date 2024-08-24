using System;
using System.Threading;
using System.Threading.Tasks;

namespace Amusoft.Toolkit.Threading;

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

internal abstract class DebounceStack
{
}