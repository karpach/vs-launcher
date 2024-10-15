using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Karpach.VisualStudio.Launcher
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			var host = CreateHostBuilder(args).Build();
			var app = host.Services.GetRequiredService<Application>();
			await app.Run(args);
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureServices((_, services) =>
					services.AddSingleton<Application>()
						.AddSingleton<IVisualStudioLocator, VisualStudioLocator>()
						.AddSingleton<IMessageBox, MessageBox>()
						.AddSingleton<IVisualStudioCommander, VisualStudioCommander>());
	}
}
