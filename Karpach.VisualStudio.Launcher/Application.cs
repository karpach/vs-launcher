using EnvDTE;
using System.Text.RegularExpressions;

namespace Karpach.VisualStudio.Launcher;

public class Application
{
	private readonly IVisualStudioLocator _visualStudioLocator;

	public Application(IVisualStudioLocator visualStudioLocator)
	{
		_visualStudioLocator = visualStudioLocator;
	}

	public async Task Run(string[] args)
	{
		if (args.Length == 0)
		{
			Console.WriteLine("Please provide a URL");
			return;
		}
		string url = args[0];
		(string filePath, int lineNumber) = ExtractFilePathAndLineNumber(url);
		_DTE[] instances = _visualStudioLocator.GetIDEInstances(true);
		foreach (_DTE instance in instances)
		{
			string solutionDirectory = Path.GetDirectoryName(instance.Solution.FileName);
			if (filePath.StartsWith(solutionDirectory, StringComparison.OrdinalIgnoreCase))
			{
				instance.MainWindow.Activate();
				instance.ItemOperations.OpenFile(filePath);
				await Task.Delay(100);
				TextSelection selection = (TextSelection)instance.ActiveDocument.Selection;
				selection.GotoLine(lineNumber);
			}
		}
	}

	private static (string filePath, int lineNumber) ExtractFilePathAndLineNumber(string url)
	{
		var match = Regex.Match(url, @"\w+://file/(?<filePath>.*):(?<lineNumber>\d+)");
		if (match.Success)
		{
			string filePath = Path.GetFullPath(match.Groups["filePath"].Value);
			int lineNumber = int.Parse(match.Groups["lineNumber"].Value);
			return (filePath, lineNumber);
		}
		throw new ArgumentException("Invalid URL format", nameof(url));
	}
}