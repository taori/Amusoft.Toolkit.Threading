# Amusoft.Toolkit.Threading.Debouncer

## FromStackAsync
### Signature
```csharp
Task<T> FromStackAsync<T>(Func<CancellationToken, Task<T>> expression, LoaderIdentity identity)
```
### Purpose

If you have a scenario where a task is repeatiately called in succession and you want to have all those threads be merged to all result in the task that was executed last for that call identity, this method can be used to accomplish this.

If a method with the same identity is called, while the first one is still running, the first method execution will have it's cancellation token cancelled to reduce unnecessary calls.