using FluentAssertions;
using FluentAssertions.Execution;
using Moq;

namespace Karpach.VisualStudio.Launcher.Tests
{
	public class Tests
	{
		private Application _instance;
		private Mock<IVisualStudioLocator> _visualStudioLocator;
		private Mock<IMessageBox> _messageBox;

		[SetUp]
		public void Setup()
		{
			_visualStudioLocator = new Mock<IVisualStudioLocator>();
			_messageBox = new Mock<IMessageBox>();
			_instance = new Application(_visualStudioLocator.Object, _messageBox.Object);
		}

		[Test]
		public void ExtractFilePathAndLineNumber_Gold_Flow_Test()
		{
			// Arrange
			string url = "vscode-insiders://file/c:/SourceCode/vs-launcher/Program.cs:12";

			// Act
			(string filePath, int lineNumber) = _instance.ExtractFilePathAndLineNumber(url);

			// Assert
			using (new AssertionScope())
			{
				filePath.Should().Be(@"c:\SourceCode\vs-launcher\Program.cs");
				lineNumber.Should().Be(12);
			}
		}
	}
}