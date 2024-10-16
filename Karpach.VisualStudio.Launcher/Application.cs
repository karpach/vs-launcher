using System.Text;
using EnvDTE;
using System.Text.RegularExpressions;

namespace Karpach.VisualStudio.Launcher;

public class Application
{
	private readonly IVisualStudioLocator _visualStudioLocator;
	private readonly IMessageBox _messageBox;
	private readonly IVisualStudioCommander _visualStudioCommander;

	public Application(IVisualStudioLocator visualStudioLocator, IMessageBox messageBox, IVisualStudioCommander visualStudioCommander)
	{
		_visualStudioLocator = visualStudioLocator;
		_messageBox = messageBox;
		_visualStudioCommander = visualStudioCommander;
	}

	public async Task Run(string[] args)
	{
		if (args.Length == 0)
		{
			_messageBox.ShowError("Please provide a URL.");
			return;
		}
		string url = args[0];
		(string filePath, int lineNumber) = ExtractFilePathAndLineNumber(url);
		if (string.IsNullOrEmpty(filePath))
		{
			return;
		}
		_DTE[] instances = _visualStudioLocator.GetIDEInstances(true);
		StringBuilder solutionNames = new StringBuilder();
		bool found = false;
		foreach (_DTE instance in instances)
		{
			string solutionDirectory = Path.GetDirectoryName(instance.Solution.FileName);
			solutionNames.AppendLine(instance.Solution.FileName);
			if (filePath.StartsWith(solutionDirectory, StringComparison.OrdinalIgnoreCase))
			{
				_visualStudioCommander.OpenFileInVisualStudio(instance, filePath, lineNumber);
				found = true;
			}
		}
		if (!found)
		{
			_messageBox.ShowError($"The following solutions don't match requested url:{Environment.NewLine}{solutionNames}");
		}
	}

	internal (string filePath, int lineNumber) ExtractFilePathAndLineNumber(string url)
	{
		var match = Regex.Match(url, @"\w+://file/(?<filePath>.*):(?<lineNumber>\d+)");
		if (match.Success)
		{
			string filePath = Path.GetFullPath(match.Groups["filePath"].Value);
			int lineNumber = int.Parse(match.Groups["lineNumber"].Value);
			return (filePath, lineNumber);
		}
		_messageBox.ShowError($"Invalid URL format {url}");
		return (string.Empty, 0);
	}
}