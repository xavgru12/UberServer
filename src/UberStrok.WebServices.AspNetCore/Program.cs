using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace UberStrok.WebServices.AspNetCore
{
    public class Program
    {
        private static Startup startup;
        public static void Main(string[] args)
        {
            var app = CreateWebHostBuilder(args).Build();
            startup.Configure(app, app.Environment);
            app.Run();
        }

        public static WebApplicationBuilder CreateWebHostBuilder(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            startup = new Startup(builder.Configuration);
            builder.Configuration.AddJsonFile("configs/game/items.json", optional: false, reloadOnChange: true);
            builder.Configuration.AddJsonFile("configs/game/maps.json", optional: false, reloadOnChange: true);
            builder.Configuration.AddJsonFile("configs/game/application.json", optional: false, reloadOnChange: true);
            builder.Configuration.AddJsonFile("configs/game/servers.json", optional: false, reloadOnChange: true);
            builder.Configuration.AddJsonFile("configs/account.json", optional: false, reloadOnChange: true);
            builder.Configuration.AddJsonFile("configs/game/excludedItemsNewPlayer.json", optional: false, reloadOnChange: true);
            startup.ConfigureServices(builder.Services);
            return builder;
        }
    }
}
