namespace Karpach.VisualStudio.Launcher;

public class MessageBox : IMessageBox
{
	public void ShowError(string message)
	{
		System.Windows.Forms.MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
	}

	public void ShowInformation(string message)
	{
		System.Windows.Forms.MessageBox.Show(message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
	}
}
