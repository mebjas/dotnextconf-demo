namespace dotnextconf_demo
{
    using System;
    using System.Threading.Tasks;
    using dotnextconf_demo.Azure;

    public class SimplePublisher: IPublisher
    {
        private IPublisher publisher;

        public SimplePublisher(IPublisher publisher)
        {
            this.publisher = publisher;
        }

        #region public method
        /// <summary>
        /// Method to publish message
        /// </summary>
        /// <param name="message">message string</param>
        /// <returns>Task Instance</returns>
        public async Task PublishAsync(string message)
        {
            try
            {
                await this.publisher.PublishAsync(message).ConfigureAwait(false);
                Logger.Log(string.Format("Message Sent = {0}", message), LoggerState.Success);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }
        #endregion
    }
}
