using camunda.helper.Models;
using Camunda.Worker;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace camunda.helper.Camunda.Worker.Handlers.Saga.Orchestrator
{
    [HandlerTopics("Deliver_Order")]
    [HandlerVariables(new string[] { "exceptionInProcess", "orderId" })]
    public class DeliverOrderHandler : IExternalTaskHandler
    {
        private readonly ILogger<DeliverOrderHandler> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        public DeliverOrderHandler(ILogger<DeliverOrderHandler> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IExecutionResult> HandleAsync(ExternalTask externalTask, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deliver_Order handler is called from Camunda...");
            try
            {
                //_logger.LogInformation($"Adding Tea leaves for {externalTask.Variables["numberOfCups"].AsInteger()} number of cups..........");
                ////Mimicking operation
                var httpClient = _httpClientFactory.CreateClient();
                var orderId = externalTask.Variables["orderId"].AsString();
                var data = new DeliveryModel { OrderId = orderId };
                var content = new StringContent(JsonConvert.SerializeObject(data));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await httpClient.PostAsync("http://localhost:3004/delivery", content);

                response.EnsureSuccessStatusCode();

                if (externalTask.Variables["exceptionInProcess"].AsInteger() == 4)
                {
                    throw new NotImplementedException();
                }
                
                //return success
                return new CompleteResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"error occured!! error messge: {ex.Message}");
                //return failure
                return new BpmnErrorResult("Deliver_Order_Failed", "Error occured while invoking Deliver_Order..");
            }
        }
    }
}
