using System;

namespace Lib.Consumers.Attributes
{
    public sealed class SomeMessageConsumerAttribute : ConsumerAttribute
    {
        public SomeMessageConsumerAttribute()
        {
            QueueName = $"{nameof(SomeMessageConsumer)}_Queue.{Environment.MachineName}";
            TopicName = $"{nameof(SomeMessageConsumer)}_Topic.{Environment.MachineName}";
            SubscriptionName = $"{nameof(SomeMessageConsumer)}_Subscription.{Environment.MachineName}";
        }
    }
}
