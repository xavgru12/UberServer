using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UberStrok.WebServices.AspNetCore.Core.Discord;

namespace UberStrok.WebServices.AspNetCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost builded = CreateHostBuilder(args).Build();
            _ = builded.Services.GetService<CoreDiscord>().RunAsync();
            _ = builded.RunAsync();
            builded.WaitForShutdown();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(delegate (IWebHostBuilder webBuilder)
            {
                _ = webBuilder.UseStartup<Startup>().UseKestrel();
            }).ConfigureLogging(delegate (HostBuilderContext hostingContext, ILoggingBuilder logging)
            {
                _ = logging.AddLog4Net();
                _ = logging.SetMinimumLevel(LogLevel.Debug);
            });
        }
    }
}
