using System;
using System.Threading.Tasks;
using Lib;
using Lib.Configuration;
using Lib.Consumers;
using Lib.Helpers;
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
                .ConfigureAppConfiguration(config =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                    config.AddCommandLine(args);
                })
                .ConfigureLogging((hostingContext, loggingBuilder) =>
                {
                    loggingBuilder.AddNLog();
                    loggingBuilder.AddConsole();
                    loggingBuilder.AddApplicationInsights();
                    loggingBuilder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    loggingBuilder.Services.AddSingleton(serviceProvider =>
                    {
                        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                        return loggerFactory.CreateLogger("AzureServiceBusDemo");
                    });
                })
                .ConfigureServices((hostingContext, services) =>
                {
                    services.AddLogging();
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
    }
}
