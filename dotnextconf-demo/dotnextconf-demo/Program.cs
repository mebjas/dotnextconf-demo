namespace dotnextconf_demo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using dotnextconf_demo.Azure;

    class Program
    {
        //// TODO: load from configuration
        static string connectionString = "";
        static string queueName = "testqueue";
        static string secondaryQueueName = "testsecondaryqueue2";

        static void Main(string[] args)
        {
            IPublisher azureSBQPublisherPrimary = new ServiceBusQueue(connectionString, queueName);
            IPublisher azureSBQPublisherSecondary = new ServiceBusQueue(connectionString, secondaryQueueName);
            IPublisher publisher = null;
            Console.WriteLine("Select 1. for simple publisher, 2. for HA publisher");
            string response = Console.ReadLine();
            if (int.Parse(response) == 1)
            {
                publisher = new SimplePublisher(azureSBQPublisherPrimary);
            }
            else if (int.Parse(response) == 2)
            {
                publisher = new HAPublisher(azureSBQPublisherPrimary, azureSBQPublisherSecondary);
            }
            else
            {
                Logger.Log("Invalid choise", LoggerState.Failure);
            }

            Task.Run(() => SendRequestInLoop(publisher, 1));
            Console.ReadKey();
        }

        static async Task SendRequestInLoop(IPublisher publisher, int reqPerSec = 2)
        {
            int iteration = 0;
            while (true)
            {
                Logger.Log("Iteration=" + iteration);
                List<Task> tasks = new List<Task>(0);
                for (int i = 0; i < reqPerSec; i++)
                {
                    tasks.Add(publisher.PublishAsync(string.Format("{0};{1}", iteration, i)));
                }

                await Task.Delay(1000).ConfigureAwait(false);
                //await Task.WhenAll(tasks).ConfigureAwait(false);
                ++iteration;
            }
        }
    }
}
