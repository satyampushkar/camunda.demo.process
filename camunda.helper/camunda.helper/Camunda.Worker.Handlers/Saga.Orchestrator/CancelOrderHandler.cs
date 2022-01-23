using Camunda.Worker;

namespace camunda.helper.Camunda.Worker.Handlers.Saga.Orchestrator
{
    [HandlerTopics("Cancel_Order")]
    [HandlerVariables(new string[] { "orderId" })]
    public class CancelOrderHandler : IExternalTaskHandler
    {
        private readonly ILogger<CancelOrderHandler> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        public CancelOrderHandler(ILogger<CancelOrderHandler> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IExecutionResult> HandleAsync(ExternalTask externalTask, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Cancel_Order handler is called from Camunda...");
            try
            {
                _logger.LogInformation($"Cancelling Order..........");
                ////Mimicking operation
                if (externalTask.Variables["orderId"] != null)
                {
                    var httpClient = _httpClientFactory.CreateClient();

                    var response = await httpClient.DeleteAsync("http://localhost:3001/order/"
                                    + externalTask.Variables["orderId"].AsString());

                    response.EnsureSuccessStatusCode();
                }                

                //return success
                return new CompleteResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"error occured!! error messge: {ex.Message}");
                //return failure
                return new BpmnErrorResult("Cancel_Order_Failed", "Error occured while invoking Cancel_Order..");
            }
        }
    }
}
