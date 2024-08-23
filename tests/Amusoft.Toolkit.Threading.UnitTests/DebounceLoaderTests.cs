using Shouldly;
using Xunit.Abstractions;

namespace Amusoft.Toolkit.Threading.UnitTests;

public class DebounceLoaderTests : TestBase
{
    [Theory]
    [InlineData("a","b","b")]
    [InlineData("b","a","a")]
    public async Task SecondWins(string a, string b, string expected)
    {
        var guid = Guid.Parse("91C22717-AE7C-493D-A490-A98749846035");

        var tasks = new[]
        {
            DebounceLoader.From(async (cancel) =>
                {
                    await Task.Delay(500, cancel);
                    return a;
                },
                new GuidLoaderIdentity(guid)
            ),
            DebounceLoader.From(async (cancel) =>
                {
                    await Task.Delay(500, cancel);
                    return b;
                },
                new GuidLoaderIdentity(guid)
            ),
        }.ToArray();
            
        _ = tasks[0];
        await Task.Delay(50);
        _ = tasks[1];

        // await Task.WhenAll(tasks);
    }

    public DebounceLoaderTests(ITestOutputHelper outputHelper, AssemblyInitializer data) : base(outputHelper, data)
    {
    }
}

public class DebouncerTests : TestBase
{
    [Theory]
    [InlineData("a","b","b")]
    [InlineData("b","a","a")]
    public async Task SecondWins(string a, string b, string expected)
    {
        var guid = Guid.Parse("91C22717-AE7C-493D-A490-A98749846035");

        var tasks = new[]
        {
            Debouncer.FromStackAsync(async (cancel) =>
                {
                    await Task.Delay(1000, cancel);
                    return a;
                },
                new GuidLoaderIdentity(guid)
            ),
            Debouncer.FromStackAsync(async (cancel) =>
                {
                    await Task.Delay(1000, cancel);
                    return b;
                },
                new GuidLoaderIdentity(guid)
            ),
        }.ToArray();
            
        _ = tasks[0];
        await Task.Delay(50);
        _ = tasks[1];

        await Task.WhenAll(tasks);
        (await tasks[0]).ShouldBe(expected);
        (await tasks[1]).ShouldBe(expected);
    }
    
    public DebouncerTests(ITestOutputHelper outputHelper, AssemblyInitializer data) : base(outputHelper, data)
    {
    }
}