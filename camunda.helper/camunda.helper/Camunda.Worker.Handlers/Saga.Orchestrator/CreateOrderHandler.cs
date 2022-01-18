using Camunda.Worker;

namespace camunda.helper.Camunda.Worker.Handlers.Saga.Orchestrator
{

    [HandlerTopics("Create_Order")]
    [HandlerVariables(new string[] { "exceptionInProcess" })]
    public class CreateOrderHandler : IExternalTaskHandler
    {
        private readonly ILogger<CreateOrderHandler> _logger;
        public CreateOrderHandler(ILogger<CreateOrderHandler> logger)
        {
            _logger = logger;
        }
        public async Task<IExecutionResult> HandleAsync(ExternalTask externalTask, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Create_Order handler is called from Camunda...");
            try
            {
                //_logger.LogInformation($"Adding Tea leaves for {externalTask.Variables["numberOfCups"].AsInteger()} number of cups..........");
                ////Mimicking operation
                //Task.Delay(5000).Wait();
                if (externalTask.Variables["exceptionInProcess"].AsInteger() == 1)
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
                return new BpmnErrorResult("Create_Order_Failed", "Error occured while invoking Create_Order..");
            }
        }
    }
}
