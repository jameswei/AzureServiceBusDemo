using System;

namespace Lib.Consumers.Attributes
{
    public abstract class ConsumerAttribute : Attribute
    {
        public string QueueName { get; }
        public string TopicName { get; }
        public string SubscriptionName { get; }

        protected ConsumerAttribute(string queueName, string topicName, string subscriptionName)
        {
            this.QueueName = queueName;
            this.TopicName = topicName;
            this.SubscriptionName = subscriptionName;
        }
    }
}
