using Camunda.Worker;

namespace camunda.helper.Camunda.Worker.Handlers
{
    [HandlerTopics("Inform_Inventory")]
    public class InformInventoryHandler : IExternalTaskHandler
    {
        private readonly ILogger<InformInventoryHandler> _logger;
        public InformInventoryHandler(ILogger<InformInventoryHandler> logger)
        {
            _logger = logger;
        }
        public async Task<IExecutionResult> HandleAsync(ExternalTask externalTask, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Inform_Inventory handler is called from Camunda...");
            try
            {
                _logger.LogInformation($"Informing inventory to refill..........");
                //Mimicking operation
                Task.Delay(5000).Wait();

                //return success
                return new CompleteResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"error occured!! error messge: {ex.Message}");
                //return failure
                return new BpmnErrorResult("Inform_Inventory", "Error occured while invoking Inform_Inventory..");
            }
        }
    }
}
