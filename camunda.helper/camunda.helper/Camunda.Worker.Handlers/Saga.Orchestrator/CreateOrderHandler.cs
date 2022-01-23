using camunda.helper.Models;
using Camunda.Worker;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace camunda.helper.Camunda.Worker.Handlers.Saga.Orchestrator
{

    [HandlerTopics("Create_Order")]
    [HandlerVariables(new string[] { "exceptionInProcess", "OrderPostModel" })]
    public class CreateOrderHandler : IExternalTaskHandler
    {
        private readonly ILogger<CreateOrderHandler> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        public CreateOrderHandler(ILogger<CreateOrderHandler> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IExecutionResult> HandleAsync(ExternalTask externalTask, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Create_Order handler is called from Camunda...");
            try
            {
                _logger.LogInformation($"Creating Order..........");
                ////Mimicking operation

                var httpClient = _httpClientFactory.CreateClient();
                var orderData = externalTask.Variables["OrderPostModel"].AsString();
                var content = new StringContent(orderData);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await httpClient.PostAsync("http://localhost:3001/order", content);

                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                var orderId = JsonConvert.DeserializeObject<string>(result);

                if (externalTask.Variables["exceptionInProcess"].AsInteger() == 1)
                {
                    throw new NotImplementedException();
                }
                //return success
                return new CompleteResult 
                {
                    Variables = new Dictionary<string, Variable>
                    {
                        ["orderId"] = new Variable(orderId, VariableType.String)
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"error occured!! error messge: {ex.Message}");
                //return failure
                return new BpmnErrorResult("Create_Order_Failed", "Error occured while invoking Create_Order..");
            }
        }
    }
}
