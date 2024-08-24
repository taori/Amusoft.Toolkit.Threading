using Amusoft.Toolkit.Threading.Exceptions;
using Shouldly;
using Xunit.Abstractions;

namespace Amusoft.Toolkit.Threading.UnitTests;

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
                    await Task.Delay(100, cancel);
                    return a;
                },
                new GuidLoaderIdentity(guid)
            ),
            Debouncer.FromStackAsync(async (cancel) =>
                {
                    await Task.Delay(100, cancel);
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
    
    [Fact]
    public async Task SimilarIdentityException()
    {
        var guid = Guid.Parse("A150A23C-3490-4967-8FA4-81573A86699E");

        var identity = new GuidLoaderIdentity(guid);
        var ta = Debouncer.FromStackAsync(async (cancel) =>
            {
                await Task.Delay(1000, cancel);
                return "a";
            },
            identity
        );
        var tb = Debouncer.FromStackAsync(async (cancel) =>
            {
                await Task.Delay(1000, cancel);
                return 1;
            },
            identity
        );

        _ = ta;
        await Task.Delay(50);
        _ = tb;

        var settings = new VerifySettings();
        settings.ScrubMember<Exception>(nameof(Exception.StackTrace));
        await Verifier.ThrowsTask(async () => await Task.WhenAll(ta, tb), settings);
    }
    
    public DebouncerTests(ITestOutputHelper outputHelper, AssemblyInitializer data) : base(outputHelper, data)
    {
    }
}