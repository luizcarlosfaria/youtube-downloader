using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace DevWeek
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
            services.AddMvc();

            services.AddScoped((sp) =>
            {
                return new RabbitMQ.Client.ConnectionFactory()
                {
                    Uri = new Uri(this.Configuration.GetSection("DevWeek:RabbitMQ:ConnectionString").Get<string>())
                };
            });

            services.AddScoped((sp) =>
            {
                return sp.GetRequiredService<ConnectionFactory>().CreateConnection();
            });

            services.AddTransient((sp) =>
            {
                return sp.GetRequiredService<IConnection>().CreateModel();
            });

            services.AddSingleton((sp) =>
            {
                return this.Configuration;
            });

            services.AddSingleton((sp) =>
            {
                return StackExchange.Redis.ConnectionMultiplexer.Connect(this.Configuration.GetSection("DevWeek:Redis:ConnectionString").Get<string>(), null);
            });

            

            services.AddSingleton((sp) =>
            {
                return new Minio.MinioClient(
                    this.Configuration.GetSection("DevWeek:S3:Endpoint").Get<string>(), 
                    this.Configuration.GetSection("DevWeek:S3:AccessKey").Get<string>(), 
                    this.Configuration.GetSection("DevWeek:S3:SecretKey").Get<string>());
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
