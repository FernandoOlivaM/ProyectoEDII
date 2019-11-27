using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProyectoEDII.LogInServices;
using ProyectoEDII.Models;

namespace ProyectoEDII
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
            //LogInDatabase
            services.Configure<LogInDatabase>(Configuration.GetSection(nameof(LogInDatabase)));
            services.AddSingleton<ILogInDatabase>(sp => sp.GetRequiredService<IOptions<LogInDatabase>>().Value);
            services.AddSingleton<LogInService>();
            //MessagesDatabase
            services.Configure<MessagesDatabase>(Configuration.GetSection(nameof(MessagesDatabase)));
            services.AddSingleton<IMessagesDatabase>(sp => sp.GetRequiredService<IOptions<MessagesDatabase>>().Value);
            services.AddSingleton<MessagesService>();
            services.AddControllers();
            //Files
            services.Configure<FilescompressionDatabase>(Configuration.GetSection(nameof(FilescompressionDatabase)));
            services.AddSingleton<IFilesCompressionDatabase>(sp => sp.GetRequiredService<IOptions<FilescompressionDatabase>>().Value);
            services.AddSingleton<FilesCompressionService>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
