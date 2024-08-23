using System.Reflection;
using NLog;

namespace Amusoft.Toolkit.Threading.Tests.Shared.Toolkit;

public class EmbeddedResourceReader
{
	private static readonly Logger Log = LogManager.GetLogger(nameof(EmbeddedResourceReader));

	private readonly Assembly _assembly;

	public EmbeddedResourceReader(Assembly assembly)
	{
		_assembly = assembly;
	}

	public string GetContent(string accessPath)
	{
		try
		{
			using var reader = GetStream(accessPath);
			if (reader is null)
				throw new Exception($"{accessPath} not found");
			
			using var streamReader = new StreamReader(reader);
			return streamReader.ReadToEnd();
		}
		catch (Exception e)
		{
			Log.Error("available names: {@Values}", _assembly.GetManifestResourceNames());
			throw new Exception($"Failed to get content for accessPath {accessPath}", e);
		}
	}

	private Stream? GetStream(string accessPath)
	{
		var fullPath = _assembly.GetName().Name + "." + accessPath;
		return _assembly.GetManifestResourceStream(fullPath);
	}
}