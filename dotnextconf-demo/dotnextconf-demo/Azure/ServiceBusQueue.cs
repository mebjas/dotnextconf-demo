namespace dotnextconf_demo.Azure
{
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Azure.ServiceBus;

    public class ServiceBusQueue: IPublisher
    {
        private IQueueClient queueClient;

        public ServiceBusQueue(string connectionString, string queueName)
        {
            this.queueClient = new QueueClient(connectionString, queueName);
        }

        public async Task PublishAsync(string message)
        {
            var messageObject = new Message(Encoding.UTF8.GetBytes(message));
            await this.queueClient.SendAsync(messageObject).ConfigureAwait(false);
        }
    }
}
