using EnvDTE;

namespace Karpach.VisualStudio.Launcher;

public class VisualStudioCommander : IVisualStudioCommander
{
	public async Task OpenFileInVisualStudio(_DTE dte, string filePath, int lineNumber)
	{
		dte.MainWindow.Activate();
		dte.ItemOperations.OpenFile(filePath);
		await Task.Delay(100);
		TextSelection selection = (TextSelection)dte.ActiveDocument.Selection;
		selection.GotoLine(lineNumber);
	}
}