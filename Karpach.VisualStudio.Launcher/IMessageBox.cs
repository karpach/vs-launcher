namespace Karpach.VisualStudio.Launcher;

public interface IMessageBox
{
	void ShowError(string message);
	void ShowInformation(string message);
}