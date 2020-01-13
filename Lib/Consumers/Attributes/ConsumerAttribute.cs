using System;

namespace Lib.Consumers.Attributes
{
    public class ConsumerAttribute : Attribute
    {
        public string QueueName { get; set; }
        public string TopicName { get; set; }
        public string SubscriptionName { get; set; }
    }
}
