using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EnvDTE;

namespace Karpach.VisualStudio.Launcher
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine("Please provide a URL");
				return;
			}
			string url = args[0];
			(string filePath, int lineNumber) = ExtractFilePathAndLineNumber(url);
			_DTE[] instances = VisualStudioLocator.GetIDEInstances(true);
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

		/// <summary>
		/// Extracts file path and line number from the URL.
		/// </summary>
		/// <param name="url">Example "vscode-insiders://file/c:/SourceCode/ocr-shared-service/Source/ocr-shared-service-test-client/Program.cs:98"</param>
		/// <returns>filePath and line number</returns>
		/// <exception cref="ArgumentException"></exception>
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
}
