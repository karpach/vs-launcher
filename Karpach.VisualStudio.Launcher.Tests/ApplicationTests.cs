using EnvDTE;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;

namespace Karpach.VisualStudio.Launcher.Tests
{
	public class ApplicationTests
	{
		private Application _instance;
		private Mock<IVisualStudioLocator> _visualStudioLocator;
		private Mock<IMessageBox> _messageBox;
		private Mock<IVisualStudioCommander> _visualStudioCommander;

		[SetUp]
		public void Setup()
		{
			_visualStudioLocator = new Mock<IVisualStudioLocator>();
			_messageBox = new Mock<IMessageBox>();
			_visualStudioCommander = new Mock<IVisualStudioCommander>();
			_instance = new Application(_visualStudioLocator.Object, _messageBox.Object, _visualStudioCommander.Object);
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

		[Test]
		public void ExtractFilePathAndLineNumber_Invalid_Url_Test()
		{
			// Arrange
			string url = "vscode-insiders://file/c:/SourceCode/vs-launcher/Program.cs";

			// Act
			(string filePath, int lineNumber) = _instance.ExtractFilePathAndLineNumber(url);

			// Assert
			using (new AssertionScope())
			{
				filePath.Should().BeNullOrEmpty();
				lineNumber.Should().Be(0);
			}
			_messageBox.Verify(x => x.ShowError($"Invalid URL format {url}"), Times.Once);
		}

		[Test]
		public async Task Run_No_Arguments()
		{
			// Arrange
			string[] args = [];

			// Act
			await _instance.Run(args);

			// Assert
			_messageBox.Verify(x => x.ShowError("Please provide a URL."), Times.Once);
		}

		[Test]
		public async Task Run_Invalid_Url_Test()
		{
			// Arrange
			string url = "vscode-insiders://file/c:/SourceCode/vs-launcher/Program.cs";
			string[] args = [ url ];

			// Act
			await _instance.Run(args);

			// Assert
			_messageBox.Verify(x => x.ShowError($"Invalid URL format {url}"), Times.Once);
		}

		[Test]
		public async Task Run_Non_Matching_Solutions()
		{
			// Arrange
			string url = "vscode-insiders://file/c:/SourceCode/vs-launcher/Program.cs:12";
			string[] args = [url];
			Mock<_DTE>[] instances = new Mock<_DTE>[]
			{
				new Mock<_DTE>(),
				new Mock<_DTE>(),
			};
			string[] solutionNames = [@"c:\SourceCode\app\app.sln", @"c:\SourceCode\app2\app2.sln"];
			for (int i = 0; i < solutionNames.Length; i++)
			{
				instances[i].Setup(x => x.Solution.FileName).Returns(solutionNames[i]);
			}
			_visualStudioLocator.Setup(x => x.GetIDEInstances(true)).Returns(instances.Select(s => s.Object).ToArray());

			// Act
			await _instance.Run(args);

			// Assert
			string expectedSolutionNames = string.Join(Environment.NewLine, solutionNames);
			_messageBox.Verify(x => x.ShowError($"No Visual Studio instance found with the following solutions:{Environment.NewLine}{expectedSolutionNames}{Environment.NewLine}"), Times.Once);
		}

		[Test]
		public async Task Run_Matching_Solution()
		{
			// Arrange
			int lineNumber = 12;
			string url = $"vscode-insiders://file/c:/SourceCode/vs-launcher/Program.cs:{lineNumber}";
			string[] args = [url];
			Mock<_DTE>[] instances = new Mock<_DTE>[]
			{
				new Mock<_DTE>(),
				new Mock<_DTE>(),
				new Mock<_DTE>(),
			};
			string[] solutionNames = [@"c:\SourceCode\app\app.sln", @"c:\SourceCode\vs-launcher\app.sln", @"c:\SourceCode\app2\app2.sln"];
			for (int i = 0; i < solutionNames.Length; i++)
			{
				instances[i].Setup(x => x.Solution.FileName).Returns(solutionNames[i]);
			}
			_visualStudioLocator.Setup(x => x.GetIDEInstances(true)).Returns(instances.Select(s => s.Object).ToArray());

			// Act
			await _instance.Run(args);

			// Assert
			_visualStudioCommander.Verify(x => x.OpenFileInVisualStudio(@"c:\SourceCode\vs-launcher\Program.cs", lineNumber), Times.Once);
		}
	}
}