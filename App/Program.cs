using System;
using System.Threading.Tasks;
using Lib.Bootstrapping;
using Lib.Configuration;
using Lib.Consumers;
using Lib.HostedServices;
using Lib.Messages;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;

namespace App
{
    public static class Program
    {

        private static void LoggingConfig(HostBuilderContext ctx, ILoggingBuilder builder)
        {
            builder.AddNLog();
            builder.AddConsole();
            builder.AddApplicationInsights(ctx.Configuration.GetSection("ApplicationInsight").Get<AppInsightsSection>().InstrumentationKey);
            builder.AddConfiguration(ctx.Configuration.GetSection("Logging"));
            builder.Services.AddSingleton(srvProvider => srvProvider.GetRequiredService<ILoggerFactory>().CreateLogger("AzureServiceBusDemo"));
        }

        private static void AppConfig(HostBuilderContext ctx, IConfigurationBuilder builder, string[] args)
        {
            builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            builder.AddJsonFile($"appsettings.{env}.json", optional: true);
            builder.AddEnvironmentVariables();
            builder.AddCommandLine(args);
        }

        private static void ServiceConfig(HostBuilderContext ctx, IServiceCollection services)
        {
            services.Configure<AppInsightsSection>(ctx.Configuration.GetSection("ApplicationInsight"));
            services.Configure<ServiceBusSection>(ctx.Configuration.GetSection("ServiceBus"));
            services.AddTransient<IMassTransitBootstrapper, MassTransitBootstrapper>();
            services.AddTransient<SomeMessageConsumer>();
            services.AddTransient<SomeMessage>();
            services.AddSingleton(serviceProvider => serviceProvider.GetRequiredService<IMassTransitBootstrapper>().CreateBusControl());
            services.AddHostedService<BusHostedService>();
            services.AddHostedService<FakeSenderHostedService>();
            services.AddMassTransit(configurator =>
            {
                configurator.AddConsumers(MassTransitBootstrapper.ConsumerTypes);
            });
        }

        public static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureAppConfiguration((ctx, builder) => AppConfig(ctx, builder, args))
                .ConfigureLogging(LoggingConfig)
                .ConfigureServices(ServiceConfig)
                .UseConsoleLifetime();
            await host.RunConsoleAsync();

            LogManager.Shutdown();
            Console.WriteLine("Press any key to exit program !");
            Console.ReadKey();
        }

        private static string GetInstrumentationKey(this HostBuilderContext context)
        {
            var applicationInsights = context.Configuration.GetSection("ApplicationInsights").Get<AppInsightsSection>();
            return applicationInsights.InstrumentationKey;
        }
    }
}
