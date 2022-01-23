using camunda.helper.Models;
using Camunda.Worker;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace camunda.helper.Camunda.Worker.Handlers.Saga.Orchestrator
{
    [HandlerTopics("Update_Inventory")]
    [HandlerVariables(new string[] { "exceptionInProcess", "OrderPostModel", "orderId" })]
    public class UpdateInventoryHandler : IExternalTaskHandler
    {
        private readonly ILogger<UpdateInventoryHandler> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        public UpdateInventoryHandler(ILogger<UpdateInventoryHandler> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IExecutionResult> HandleAsync(ExternalTask externalTask, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Update_Inventory handler is called from Camunda...");
            try
            {
                _logger.LogInformation($"Updating Inventory..........");
                ////Mimicking operation
                var httpClient = _httpClientFactory.CreateClient();
                var orderData = externalTask.Variables["OrderPostModel"].AsString();
                var orders = JsonConvert.DeserializeObject<List<OrderData>>(orderData);

                var inventoryDeatail = new InventoryModel 
                {
                    orderId = externalTask.Variables["orderId"].AsString(),
                    products = orders,
                    action = "RemoveProduct"
                };

                var content = new StringContent(JsonConvert.SerializeObject(inventoryDeatail));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await httpClient.PutAsync("http://localhost:3003/inventory/1", content);

                response.EnsureSuccessStatusCode();

                if (externalTask.Variables["exceptionInProcess"].AsInteger() == 3)
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
                return new BpmnErrorResult("Update_Inventory_Failed", "Error occured while invoking Update_Inventory..");
            }
        }
    }
}
