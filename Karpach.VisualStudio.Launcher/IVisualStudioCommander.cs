namespace Karpach.VisualStudio.Launcher;

public interface IVisualStudioCommander
{
	Task OpenFileInVisualStudio(string filePath, int lineNumber);
}