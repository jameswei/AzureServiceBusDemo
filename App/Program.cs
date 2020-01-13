using System;
using System.Threading.Tasks;
using Lib;
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
    public  static class Program
    {
        public  static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, configBuilder) =>
                {
                    var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                    configBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    configBuilder.AddJsonFile($"appsettings.{environmentName}.json", optional: true);
                    configBuilder.AddEnvironmentVariables();
                    configBuilder.AddCommandLine(args);
                })
                .ConfigureLogging((hostingContext, loggingBuilder) =>
                {
                    loggingBuilder.AddNLog();
                    loggingBuilder.AddConsole();
                    loggingBuilder.AddApplicationInsights(hostingContext.GetInstrumentationKey());
                    loggingBuilder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    loggingBuilder.Services.AddSingleton(serviceProvider =>
                    {
                        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                        return loggerFactory.CreateLogger("AzureServiceBusDemo");
                    });
                })
                .ConfigureServices((hostingContext, services) =>
                {
                    services.Configure<AppInsightsSection>(hostingContext.Configuration.GetSection("ApplicationInsights"));
                    services.Configure<ServiceBusSection>(hostingContext.Configuration.GetSection("ServiceBus"));
                    services.AddTransient<IMassTransitBootstrapper, MassTransitBootstrapper>();
                    services.AddTransient<SomeMessageConsumer>();
                    services.AddTransient<SomeMessage>();
                    services.AddSingleton(serviceProvider =>
                    {
                        var massTransitHelper = serviceProvider.GetRequiredService<IMassTransitBootstrapper>();
                        var busControl = massTransitHelper.CreateBusControl();
                        return busControl;
                    });
                    services.AddHostedService<BusHostedService>();
                    services.AddHostedService<FakeSenderHostedService>();
                    services.AddMassTransit(configurator =>
                    {
                        configurator.AddConsumers(MassTransitBootstrapper.ConsumerTypes);
                    });
                })
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
