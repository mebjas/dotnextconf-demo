namespace dotnextconf_demo
{
    using System;
    using System.Threading.Tasks;
    using dotnextconf_demo.Azure;
    using Polly;
    using Polly.CircuitBreaker;
    using Polly.Fallback;
    using Polly.Retry;

    public class HAPublisher: IPublisher
    {
        private IPublisher primaryPublisher;
        private IPublisher secondaryPublisher;

        private AsyncRetryPolicy exponentialRetryPolicy;
        private AsyncCircuitBreakerPolicy circuitBreakerPolicy;
        private AsyncFallbackPolicy fallbackPolicy;

        public HAPublisher(IPublisher primaryPublisher, IPublisher secondaryPublisher)
        {
            this.primaryPublisher = primaryPublisher;
            this.secondaryPublisher = secondaryPublisher;

            this.exponentialRetryPolicy = Policy
                .Handle<TimeoutException>()
                .WaitAndRetryAsync(
                    2,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, context) => {
                        Logger.Log(string.Format("Retry for ex={0}", exception.GetType().Name), LoggerState.Warning);
                    });

            this.circuitBreakerPolicy = Policy.Handle<Exception>().CircuitBreakerAsync(
                2,
                TimeSpan.FromSeconds(20),
                (context, ts) => { Logger.Log("CircuitBreaker Closed to Open", LoggerState.Warning); },
                () => { Logger.Log("CircuitBreaker Reset", LoggerState.Warning); });

            this.fallbackPolicy = Policy
                .Handle<Exception>()
                .FallbackAsync(async (ctx, ct) => {
                    await this.PublishSecondaryAsync(ctx["message"].ToString());
                }, async (ex, ctx) => {
                    Logger.Log(string.Format("Executing fallback for Ex={0}", ex.GetType().Name), LoggerState.Warning);
                    await Task.FromResult(0);
                });
        }

        public async Task PublishAsync(string message)
        {
            try
            {
                Context ctx = new Context();
                ctx.Add("message", message);

                await this.fallbackPolicy.ExecuteAsync(async (context) =>
                {
                    await this.circuitBreakerPolicy.ExecuteAsync(
                        () => this.primaryPublisher.PublishAsync(message)
                    ).ConfigureAwait(false);
                }, ctx).ConfigureAwait(false);
                
                Logger.Log(string.Format("Message Sent = {0}", message), LoggerState.Success);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public async Task PublishSecondaryAsync(string message)
        {
            try
            {
                await this.secondaryPublisher.PublishAsync(message).ConfigureAwait(false);
                Logger.Log(string.Format("Message Sent = {0}, [Secondary Queue]", message), LoggerState.Success);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }
    }
}
