using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Quartz;
using SoapCore;
using System;
using UberStrok.Core;
using UberStrok.WebServices.AspNetCore.Authentication;
using UberStrok.WebServices.AspNetCore.Authentication.Jwt;
using UberStrok.WebServices.AspNetCore.Configurations;
using UberStrok.WebServices.AspNetCore.Database;
using UberStrok.WebServices.AspNetCore.Database.LiteDb;
using UberStrok.WebServices.AspNetCore.Job;

namespace UberStrok.WebServices.AspNetCore
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
            => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            // Register configurations.

            services.Configure<MapsConfiguration>(Configuration.GetSection("Maps"));
            services.Configure<ItemsConfiguration>(Configuration.GetSection("Items"));
            services.Configure<ExcludedItemsNewPlayerConfiguration>(Configuration.GetSection("ExcludedItemsNewPlayer"));
            services.Configure<ServersConfiguration>(Configuration.GetSection("Servers"));
            services.Configure<ApplicationConfiguration>(Configuration.GetSection("Application"));
            services.Configure<AccountConfiguration>(Configuration.GetSection("Account"));
            services.Configure<AuthConfiguration>(Configuration.GetSection("Auth"));

            // Register services.
            services.AddScoped<IDbService, LiteDbService>();
            services.AddSingleton<IAuthService, JwtAuthService>();
            services.AddScoped<ISessionService, SessionService>();

            services.AddSoapCore();

            services.AddScoped(s => new ItemManager(s.GetRequiredService<IOptions<ItemsConfiguration>>().Value));
            services.AddScoped(s => new MapManager(s.GetRequiredService<IOptions<MapsConfiguration>>().Value));

            // Register web services.
            services.AddScoped<ApplicationWebService>();
            services.AddScoped<AuthenticationWebService>();
            services.AddScoped<ShopWebService>();
            services.AddScoped<UserWebService>();
            services.AddScoped<ClanWebService>();
            services.AddScoped<PrivateMessageWebService>();
            services.AddScoped<RelationshipWebService>();

            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();
                q.UseSimpleTypeLoader();
                q.UseInMemoryStore();
                q.UseDefaultThreadPool(tp =>
                {
                    tp.MaxConcurrency = 2;
                });
                q.ScheduleJob<SessionJob>(trigger => trigger
                    .StartNow()
                    .WithDailyTimeIntervalSchedule(x => x.WithInterval(1, IntervalUnit.Hour))
                );
            });
            services.AddQuartzHostedService(options =>
            {
                // when shutting down we want jobs to complete gracefully
                options.WaitForJobsToComplete = true;
            });
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            app.UseSoapEndpoint<AuthenticationWebService>("/AuthenticationWebService", new SoapEncoderOptions(), SoapSerializer.DataContractSerializer);
            app.UseSoapEndpoint<ApplicationWebService>("/ApplicationWebService", new SoapEncoderOptions(), SoapSerializer.DataContractSerializer);
            app.UseSoapEndpoint<ShopWebService>("/ShopWebService", new SoapEncoderOptions(), SoapSerializer.DataContractSerializer);
            app.UseSoapEndpoint<UserWebService>("/UserWebService", new SoapEncoderOptions(), SoapSerializer.DataContractSerializer);
            app.UseSoapEndpoint<ClanWebService>("/ClanWebService", new SoapEncoderOptions(), SoapSerializer.DataContractSerializer);
            app.UseSoapEndpoint<PrivateMessageWebService>("/PrivateMessageWebService", new SoapEncoderOptions(), SoapSerializer.DataContractSerializer);
            app.UseSoapEndpoint<RelationshipWebService>("/RelationshipWebService", new SoapEncoderOptions(), SoapSerializer.DataContractSerializer);
        }
    }
}
