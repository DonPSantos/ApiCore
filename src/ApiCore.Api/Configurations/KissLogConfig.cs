using KissLog;
using KissLog.AspNetCore;
using KissLog.CloudListeners.Auth;
using KissLog.CloudListeners.RequestLogsListener;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiCore.Api.Configurations
{
    public static class KissLogConfig
    {
        public static IServiceCollection AddKissLogConfig(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ILogger>(context => Logger.Factory.Get());

            services.AddLogging(logging =>
            {
                logging.AddKissLog();
            });

            return services;
        }

        public static IApplicationBuilder UseKissLogConfig(this IApplicationBuilder app, IConfiguration configuration)
        {
            app.UseKissLogMiddleware(options =>
            {
                options.Listeners.Add(new RequestLogsApiListener(new Application(configuration.GetSection("KissLog.OrganizationId").Value,
                                                                                    configuration.GetSection("KissLog.ApplicationId").Value))
                {
                    ApiUrl = configuration.GetSection("KissLog.ApiUrl").Value
                });
            });

            return app;
        }
    }
}