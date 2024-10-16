using EnvDTE;

namespace Karpach.VisualStudio.Launcher;

public interface IVisualStudioCommander
{
	Task OpenFileInVisualStudio(_DTE dte, string filePath, int lineNumber);
}