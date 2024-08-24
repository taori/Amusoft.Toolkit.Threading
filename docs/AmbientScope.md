# Amusoft.Toolkit.Threading.AmbientScope

## AmbientScope
### Signature
```csharp
class YourScope : AmbientScope<YourScope> {}
```
### Purpose

This is an abstract implementation of the AmbientScope pattern, which allows you to simply derive your own type for it for whichever purpose you want to use it. It functions similarly to the [TransactionScope](https://learn.microsoft.com/en-us/dotnet/api/system.transactions.transactionscope?view=net-8.0).

One scenario where this can be used is for passing data to subsequently called methods, without forwarding them to methods or constructors.

### Samples
See [Tests](https://github.com/taori/Amusoft.Toolkit.Threading/blob/main/tests/Amusoft.Toolkit.Threading.UnitTests/Tests/AmbientScopeTests.cs) for comprehensive examples