using ConfigurationLib.Dashboard.Middlewares;
using ConfigurationLib.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Shared.Kernel.Bus;
using Shared.Kernel.Data;
using Shared.Kernel.Repositories;
using Shared.Kernel.Repositories.Interfaces;
using System;

namespace ConfigurationLib.Dashboard
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseGlobalExceptionHandler();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=ApplicationConfiguration}/{action=Index}/{id?}");
            });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddControllers();
            services.AddControllersWithViews();
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(Configuration.GetConnectionString("rabbitMQConnectionString"), h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            services.AddScoped<IMongoDbContext>(x => new MongoDbContext(Configuration.GetConnectionString("MongoDbConnectionString")));
            services.AddScoped<IApplicationConfigurationRepository, ApplicationConfigurationRepository>();
            services.AddScoped<IExceptionLogRepository, ExceptionLogRepository>();

            services.AddSingleton(logger);
            services.AddSingleton<IMessageBus, MessageBus>();
            services.AddSingleton<IConfigurationReader>(x => 
                new ConfigurationReader(Configuration["AppName"], 
                Configuration.GetConnectionString("MongoDbConnectionString"), 
                Configuration.GetValue<int>("RefreshTimerIntervalInMs")));
        }
    }
}