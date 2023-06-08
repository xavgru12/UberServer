using log4net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using SoapCore;
using System.IO;
using System.Net;
using UberStrok.WebServices.AspNetCore.Core.Db.Tables;
using UberStrok.WebServices.AspNetCore.Core.Discord;
using UberStrok.WebServices.AspNetCore.Core.Manager;
using UberStrok.WebServices.AspNetCore.Helper;
using UberStrok.WebServices.AspNetCore.Middleware;
using UberStrok.WebServices.AspNetCore.WebService;

namespace UberStrok.WebServices.AspNetCore
{
    public class Startup
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Startup));
        public IWebHostEnvironment HostingEnvironment { get; private set; }
        public IConfiguration Configuration { get; private set; }

        public static WebServiceConfiguration WebServiceConfiguration
        {
            get;
            private set;
        }
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            HostingEnvironment = env;
            Configuration = configuration;
            Log.Info("Initializing web services...");
            WebServiceConfiguration config = Utils.DeserializeJsonAt<WebServiceConfiguration>("assets/configs/main.json");
            if (config == null)
            {
                config = WebServiceConfiguration.Default;
                Utils.SerializeJsonAt("assets/configs/main.json", config);
            }
            WebServiceConfiguration = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Register services.
            services.AddSoapCore();

            // Register web services.
            services.AddSingleton<ApplicationWebService>();
            services.AddSingleton<AuthenticationWebService>();
            services.AddSingleton<ShopWebService>();
            services.AddSingleton<UserWebService>();
            services.AddSingleton<ClanWebService>();
            services.AddSingleton<PrivateMessageWebService>();
            services.AddSingleton<RelationshipWebService>();
            services.AddSingleton<ModerationWebService>();

            //Register Manager
            services.AddSingleton<ClanManager>();
            services.AddSingleton<UserManager>();
            services.AddSingleton<ServerManager>();
            services.AddSingleton<SecurityManager>();
            services.AddSingleton<StreamManager>();
            services.AddSingleton<ResourceManager>();
            services.AddSingleton<GameSessionManager>();
            services.AddSingleton<UberBeatManager>();

            //Register Middleware
            services.AddSingleton<JoinRoom>();

            //Register Tables
            services.AddSingleton<ClanTable>();
            services.AddSingleton<UserTable>();
            services.AddSingleton<UberBeatTable>();
            services.AddSingleton<SecurityTable>();
            services.AddSingleton<StreamTable>();

            //Register Discord
            services.AddSingleton<CoreDiscord>();

            services.Configure(delegate (KestrelServerOptions options)
            {
                options.AllowSynchronousIO = true;
                options.Listen(IPAddress.Parse("0.0.0.0"), 5000);
            });
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "assets/images")),
                RequestPath = new PathString("/images")
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
                RequestPath = new PathString("")
            });
            app.UseSoapEndpoint<AuthenticationWebService>("/2.0/AuthenticationWebService", new SoapEncoderOptions(), SoapSerializer.DataContractSerializer);
            app.UseSoapEndpoint<ApplicationWebService>("/2.0/ApplicationWebService", new SoapEncoderOptions(), SoapSerializer.DataContractSerializer);
            app.UseSoapEndpoint<ShopWebService>("/2.0/ShopWebService", new SoapEncoderOptions(), SoapSerializer.DataContractSerializer);
            app.UseSoapEndpoint<UserWebService>("/2.0/UserWebService", new SoapEncoderOptions(), SoapSerializer.DataContractSerializer);
            app.UseSoapEndpoint<ClanWebService>("/2.0/ClanWebService", new SoapEncoderOptions(), SoapSerializer.DataContractSerializer);
            app.UseSoapEndpoint<PrivateMessageWebService>("/2.0/PrivateMessageWebService", new SoapEncoderOptions(), SoapSerializer.DataContractSerializer);
            app.UseSoapEndpoint<RelationshipWebService>("/2.0/RelationshipWebService", new SoapEncoderOptions(), SoapSerializer.DataContractSerializer);
            app.UseMiddleware<JoinRoom>();
        }
    }
}
