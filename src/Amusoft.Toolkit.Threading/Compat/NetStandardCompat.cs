using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Amusoft.Toolkit.Threading.Compat;

#if NETSTANDARD2_0

internal static class NetStandardCompat
{
	internal static void Remove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> source, TKey key, out TValue removed)
	{
		source.TryRemove(key, out removed);
	}
	
	internal static bool TryPop<TValue>(this Stack<TValue> source, out TValue popped)
		where TValue : notnull
	{
		popped = source.Pop();
		return true;
	}
}
#endif