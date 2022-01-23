using camunda.helper.Models;
using Camunda.Worker;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace camunda.helper.Camunda.Worker.Handlers.Saga.Orchestrator
{
    [HandlerTopics("Reverse_Inventory")]
    [HandlerVariables(new string[] { "OrderPostModel", "orderId" })]
    public class ReverseInventoryHandler : IExternalTaskHandler
    {
        private readonly ILogger<ReverseInventoryHandler> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        public ReverseInventoryHandler(ILogger<ReverseInventoryHandler> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IExecutionResult> HandleAsync(ExternalTask externalTask, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Reverse_Inventory handler is called from Camunda...");
            try
            {
                _logger.LogInformation($"Reversing Inventory..........");
                ////Mimicking operation
                var httpClient = _httpClientFactory.CreateClient();
                var orderData = externalTask.Variables["OrderPostModel"].AsString();
                var orders = JsonConvert.DeserializeObject<List<OrderData>>(orderData);

                var inventoryDeatail = new InventoryModel
                {
                    orderId = externalTask.Variables["orderId"].AsString(),
                    products = orders,
                    action = "AddProduct"
                };

                var content = new StringContent(JsonConvert.SerializeObject(inventoryDeatail));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await httpClient.PutAsync("http://localhost:3003/inventory/1", content);

                response.EnsureSuccessStatusCode();

                //return success
                return new CompleteResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"error occured!! error messge: {ex.Message}");
                //return failure
                return new BpmnErrorResult("Reverse_Inventory_Failed", "Error occured while invoking Reverse_Inventory..");
            }
        }
    }
}
