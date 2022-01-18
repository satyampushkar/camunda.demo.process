using Camunda.Worker;

namespace camunda.helper.Camunda.Worker.Handlers.Saga.Orchestrator
{
    [HandlerTopics("Cancel_Delivery")]
    public class CancelDeliveryHandler : IExternalTaskHandler
    {
        private readonly ILogger<CancelDeliveryHandler> _logger;
        public CancelDeliveryHandler(ILogger<CancelDeliveryHandler> logger)
        {
            _logger = logger;
        }
        public async Task<IExecutionResult> HandleAsync(ExternalTask externalTask, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Cancel_Delivery handler is called from Camunda...");
            try
            {
                //_logger.LogInformation($"Adding Tea leaves for {externalTask.Variables["numberOfCups"].AsInteger()} number of cups..........");
                ////Mimicking operation
                //Task.Delay(5000).Wait();

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
