using Camunda.Worker;

namespace camunda.helper.Camunda.Worker.Handlers.Saga.Orchestrator
{
    [HandlerTopics("Reverse_Payment")]
    [HandlerVariables(new string[] { "orderId" })]
    public class ReversePaymentHandler : IExternalTaskHandler
    {
        private readonly ILogger<ReversePaymentHandler> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        public ReversePaymentHandler(ILogger<ReversePaymentHandler> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IExecutionResult> HandleAsync(ExternalTask externalTask, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Reverse_Payment handler is called from Camunda...");
            try
            {
                _logger.LogInformation($"Reversing Payment..........");
                ////Mimicking operation
                var httpClient = _httpClientFactory.CreateClient();

                var response = await httpClient.DeleteAsync("http://localhost:3002/payment/"
                                + externalTask.Variables["orderId"].AsString());

                response.EnsureSuccessStatusCode();

                //return success
                return new CompleteResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"error occured!! error messge: {ex.Message}");
                //return failure
                return new BpmnErrorResult("Reverse_Payment_Failed", "Error occured while invoking Reverse_Payment..");
            }
        }
    }
}
