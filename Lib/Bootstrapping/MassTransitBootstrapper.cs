using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lib.Configuration;
using Lib.Consumers;
using Lib.Consumers.Attributes;
using Lib.Observers;
using MassTransit;
using MassTransit.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus.Primitives;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Lib.Bootstrapping
{
    public class MassTransitBootstrapper : IMassTransitBootstrapper
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IOptions<ServiceBusSection> _options;

        public static Type[] ConsumerTypes { get; } = GetConsumersTypes().ToArray();

        public MassTransitBootstrapper(IServiceProvider serviceProvider, IOptions<ServiceBusSection> options)
        {
            _serviceProvider = serviceProvider;
            _options = options;
        }

        public IBusControl CreateBusControl()
        {
            var serviceBusSettings = _options.Value;
            var azureNamespace = serviceBusSettings.Namespace;
            var sharedAccessKey     = serviceBusSettings.SharedAccessKey;
            var sharedAccessKeyName = serviceBusSettings.SharedAccessKeyName;
            var serviceName = $"{serviceBusSettings.ServiceName}.{Environment.MachineName}";

            var loggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>();

            var busControl = Bus.Factory.CreateUsingAzureServiceBus(factoryConfigurator =>
            {
                var serviceUrl = $"sb://{azureNamespace}.servicebus.windows.net/{serviceName}";

                factoryConfigurator.Host(new Uri(serviceUrl), hostConfigurator =>
                {
                    hostConfigurator.SharedAccessSignature(providerConfigurator =>
                    {
                        providerConfigurator.KeyName = sharedAccessKeyName;
                        providerConfigurator.SharedAccessKey = sharedAccessKey;
                        providerConfigurator.TokenTimeToLive = TimeSpan.FromDays(1);
                        providerConfigurator.TokenScope = TokenScope.Namespace;
                    });
                });

                factoryConfigurator.SetLoggerFactory(loggerFactory);
                factoryConfigurator.UseServiceBusMessageScheduler();

                ConfigureBusEndpoints(factoryConfigurator);

                factoryConfigurator.BusObserver(new BusObserver(loggerFactory));
            });

            busControl.ConnectReceiveObserver(new BusReceiveObserver(loggerFactory));

            return busControl;
        }

        private void ConfigureBusEndpoints(IServiceBusBusFactoryConfigurator factoryConfigurator)
        {
            foreach (var consumerType in ConsumerTypes)
            {
                SetNames(consumerType, out var queueName, out var topicName, out var subscriptionName);
                factoryConfigurator.ReceiveEndpoint(queueName, endpointConfigurator =>
                {
                    endpointConfigurator.Subscribe(topicName, subscriptionName);
                    endpointConfigurator.ConfigureConsumer(_serviceProvider, consumerType);
                });
            }
        }

        private static IEnumerable<Type> GetConsumersTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(t => typeof(IConsumer).IsAssignableFrom(t))
                .Where(t => t.IsClass && !t.IsAbstract)
                .Where(IsConsumerType)
                .ToList();
        }

        private static bool IsConsumerType(Type type)
        {
            bool InheritFromAbstractConsumerOne(IEnumerable<Type> types)
            {
                return types.Any(x => x.IsGenericType && typeof(AbstractConsumer<>).IsAssignableFrom(x.GetGenericTypeDefinition()));
            }

            bool InheritFromAbstractConsumerTwo(Type parentType)
            {
                return parentType?.IsGenericType == true && typeof(AbstractConsumer<>).IsAssignableFrom(parentType.GetGenericTypeDefinition());
            }

            var baseType = type.BaseType;
            var interfaceTypes = type.GetInterfaces();
            
            return InheritFromAbstractConsumerOne(interfaceTypes) || InheritFromAbstractConsumerTwo(baseType);
        }

        private static void SetNames(MemberInfo consumerType, out string queueName, out string topicName, out string subscriptionName)
        {
            var consumerAttribute = (ConsumerAttribute) Attribute.GetCustomAttribute(consumerType, typeof(ConsumerAttribute), true);
            queueName = consumerAttribute?.QueueName ?? $"{consumerType.Name}_Queue";
            topicName = consumerAttribute?.TopicName ?? $"{consumerType.Name}_Topic";
            subscriptionName = consumerAttribute?.SubscriptionName ?? $"{consumerType.Name}_Subscription";
        }
    }
}
