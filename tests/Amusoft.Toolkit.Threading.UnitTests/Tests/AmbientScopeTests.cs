using Shouldly;
using Xunit.Abstractions;

namespace Amusoft.Toolkit.Threading.UnitTests.Tests;

public class AmbientScopeTests : TestBase
{
	[Fact]
	public void NoScopeAvailable()
	{
		TestScope.Current?.Name.ShouldBeNull();
	}
	
	[Theory]
	[InlineData("a")]
	[InlineData("b")]
	public void DoubleDisposeNoThrow(string name)
	{
		using (var scope = new TestScope(name))
		{
			scope.Name.ShouldBe(name);
			TestScope.Current?.Name.ShouldBe(name);
			// ReSharper disable DisposeOnUsingVariable
			scope.Dispose();
			// ReSharper restore DisposeOnUsingVariable
		}
	}
	
	[Theory]
	[InlineData("a")]
	[InlineData("b")]
	public void NameIsSet(string name)
	{
		var scope = new TestScope(name);
		scope.Name.ShouldBe(name);
		TestScope.Current?.Name.ShouldBe(name);
	}
	
	[Theory]
	[InlineData("a")]
	[InlineData("b")]
	public void CurrentNullAfterRelease(string name)
	{
		using (var scope = new TestScope(name))
		{
			scope.Name.ShouldBe(name);
			TestScope.Current?.Name.ShouldBe(name);
		}
		
		TestScope.Current.ShouldBeNull();
	}
	
	[Theory]
	[InlineData("a","b")]
	[InlineData("c","d")]
	public void ScopeNesting(string a, string b)
	{
		using (var outerScope = new TestScope(a))
		{
			outerScope.Name.ShouldBe(a);
			TestScope.Current?.Name.ShouldBe(a);
		
			using (var innerScope = new TestScope(b))
			{
				innerScope.Name.ShouldBe(b);
				TestScope.Current?.Name.ShouldBe(b);
			}	
			
			TestScope.Current?.Name.ShouldBe(a);
		}
	}
	
	[Theory]
	[InlineData("a","b","c")]
	[InlineData("d","e","f")]
	public async Task ParentScopeOrder(string a, string b, string c)
	{
		using (new TestScope(a))
		using (new TestScope(b))
		using (new TestScope(c))
		{
			var names = TestScope.Current?.GetParentScopes()
				.Select(d => d.Name)
				.ToArray();
			await Verifier
				.Verify(names)
				.UseParameters(a, b, c);
		}
	}
	
	private class TestScope : AmbientScope<TestScope>
	{
		public TestScope(string name)
		{
			Name = name;
		}

		public string Name { get; set; }
	}
	
	public AmbientScopeTests(ITestOutputHelper outputHelper, AssemblyInitializer data) : base(outputHelper, data)
	{
	}
}