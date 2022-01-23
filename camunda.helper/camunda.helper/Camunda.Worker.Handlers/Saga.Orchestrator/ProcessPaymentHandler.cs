using camunda.helper.Models;
using Camunda.Worker;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace camunda.helper.Camunda.Worker.Handlers.Saga.Orchestrator
{
    [HandlerTopics("Process_Payment")]
    [HandlerVariables(new string[] { "exceptionInProcess", "OrderPostModel", "orderId" })]
    public class ProcessPaymentHandler : IExternalTaskHandler
    {
        private readonly ILogger<ProcessPaymentHandler> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        public ProcessPaymentHandler(ILogger<ProcessPaymentHandler> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IExecutionResult> HandleAsync(ExternalTask externalTask, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Process_Payment handler is called from Camunda...");
            try
            {
                _logger.LogInformation($"Processing Payment..........");
                ////Mimicking operation
                var httpClient = _httpClientFactory.CreateClient();
                var orderData = externalTask.Variables["OrderPostModel"].AsString();
                var orders = JsonConvert.DeserializeObject<List<OrderData>>(orderData);
                double amt = 0;
                foreach (var order in orders)
                {
                    amt += (double)(order.unitPrice * order.units);
                }
                var payment = new PaymentProcessModel
                {
                    orderId = externalTask.Variables["orderId"].AsString(),
                    paymentAmount = amt
                };

                var content = new StringContent(JsonConvert.SerializeObject(payment));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await httpClient.PostAsync("http://localhost:3002/payment", content);

                response.EnsureSuccessStatusCode();
                
                if (externalTask.Variables["exceptionInProcess"].AsInteger() == 2)
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
                return new BpmnErrorResult("Process_Payment_Failed", "Error occured while invoking Process_Payment..");
            }
        }
    }
}
