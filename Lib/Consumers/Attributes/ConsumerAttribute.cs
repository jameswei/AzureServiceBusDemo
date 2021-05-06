using System;

namespace Lib.Consumers.Attributes
{
    public abstract class ConsumerAttribute : Attribute
    {
        protected string QueueName { get; }
        public string TopicName { get; set; }
        public string SubscriptionName { get; set; }

        protected ConsumerAttribute(string queueName, string topicName, string subscriptionName)
        {
            this.QueueName = queueName;
            this.TopicName = topicName;
            this.SubscriptionName = subscriptionName;
        }
    }
}
