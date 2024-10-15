using EnvDTE;

namespace Karpach.VisualStudio.Launcher;

public class VisualStudioCommander : IVisualStudioCommander
{
	private _DTE _instance;

	public VisualStudioCommander(_DTE instance)
	{
		_instance = instance;
	}

	public async Task OpenFileInVisualStudio(string filePath, int lineNumber)
	{
		_instance.MainWindow.Activate();
		_instance.ItemOperations.OpenFile(filePath);
		await Task.Delay(100);
		TextSelection selection = (TextSelection)_instance.ActiveDocument.Selection;
		selection.GotoLine(lineNumber);
	}
}