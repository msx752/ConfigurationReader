using ConfigurationLib.Dashboard.Data;
using ConfigurationLib.Dashboard.Data.Repos;
using ConfigurationLib.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
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
            var mongoDbConnectionString = Configuration.GetConnectionString("MongoDbConnectionString");

            ILogger logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            services.AddSingleton(logger);

            services.AddScoped<IMongoDbContext>(x => new MongoDbContext(mongoDbConnectionString));
            services.AddScoped<IApplicationConfigurationRepository, ApplicationConfigurationRepository>();

            services.AddSingleton<IConfigurationReader>(x => new ConfigurationReader(Configuration["AppName"], mongoDbConnectionString, Configuration.GetValue<int>("RefreshTimerIntervalInMs")));
            services.AddControllers();
            services.AddControllersWithViews();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }
    }
}