using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TesteECS.Services;
using TesteECS.Settings;

namespace TesteECS
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
            services.AddControllers();
            services.AddHostedService<Worker>();
            // services.Configure<AwsSnsSettings>("AWS::SNS", Configuration);
            services.AddOptions<AwsSnsSettings>("AWS::SNS");

            var assembly = this.GetType().Assembly;
            foreach(var i in assembly.GetTypes().Where(_ => _.IsInterface && _.GetCustomAttributes<InjectableAttribute>().Any()))
            {
                var imp = assembly.GetTypes().FirstOrDefault(_ => _.IsClass && i.IsAssignableFrom(_));
                if (imp != null) services.AddScoped(i, imp);
            }
            
            services.AddScoped<IAmazonSimpleNotificationService>(sp => new AmazonSimpleNotificationServiceClient(
                new AmazonSimpleNotificationServiceConfig
                {
                    ServiceURL = "http://localhost:4566"
                }));
            services.AddScoped<IAmazonSQS>(sp => new AmazonSQSClient(
                new AmazonSQSConfig
                {
                    ServiceURL = "http://localhost:4566"
                }));
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI();

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
