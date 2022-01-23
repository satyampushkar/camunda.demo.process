using Camunda.Worker;

namespace camunda.helper.Camunda.Worker.Handlers.Saga.Orchestrator
{
    [HandlerTopics("Cancel_Delivery")]
    [HandlerVariables(new string[] { "orderId" })]
    public class CancelDeliveryHandler : IExternalTaskHandler
    {
        private readonly ILogger<CancelDeliveryHandler> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        public CancelDeliveryHandler(ILogger<CancelDeliveryHandler> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IExecutionResult> HandleAsync(ExternalTask externalTask, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Cancel_Delivery handler is called from Camunda...");
            try
            {
                //_logger.LogInformation($"Adding Tea leaves for {externalTask.Variables["numberOfCups"].AsInteger()} number of cups..........");
                ////Mimicking operation
                var httpClient = _httpClientFactory.CreateClient();
                var orderId = externalTask.Variables["orderId"].AsString();

                var response = await httpClient.DeleteAsync("http://localhost:3004/delivery/" + orderId);

                response.EnsureSuccessStatusCode();

                //return success
                return new CompleteResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"error occured!! error messge: {ex.Message}");
                //return failure
                return new BpmnErrorResult("Cancel_Delivery_Failed", "Error occured while invoking Cancel_Delivery..");
            }
        }
    }
}
