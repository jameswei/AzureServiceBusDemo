using System;

namespace Lib.Consumers.Attributes
{
    public sealed class SomeMessageConsumerAttribute : ConsumerAttribute
    {
        public SomeMessageConsumerAttribute() : base($"{nameof(SomeMessageConsumer)}_Queue.{Environment.MachineName}", $"{nameof(SomeMessageConsumer)}_Topic.{Environment.MachineName}", $"{nameof(SomeMessageConsumer)}_Subscription.{Environment.MachineName}")
        {
        }
    }
}
