using ConfigurationLib.Interfaces;
using ConfigurationLib.LogService.Consumers;
using ConfigurationLib.LogService.Logics;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Shared.Kernel.Data;
using Shared.Kernel.Repositories;
using Shared.Kernel.Repositories.Interfaces;
using System;

namespace ConfigurationLib.LogService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddControllers();
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

                x.AddConsumer<ExceptionLogConsumer>();
            });

            services.AddSingleton(logger);
            services.AddSingleton<IConfigurationReader>(x => new ConfigurationReader(Configuration["AppName"], Configuration.GetConnectionString("MongoDbConnectionString"), Configuration.GetValue<int>("RefreshTimerIntervalInMs")));

            services.AddTransient<IMongoDbContext>(x => new MongoDbContext(Configuration.GetConnectionString("MongoDbConnectionString")));
            services.AddTransient<IExceptionLogRepository, ExceptionLogRepository>();
            services.AddTransient<IExceptionLogLogic, ExceptionLogLogic>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
