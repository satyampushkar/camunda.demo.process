using Camunda.Worker;

namespace camunda.helper.Camunda.Worker.Handlers
{
    [HandlerTopics("Boil_Water")]
    [HandlerVariables(new string[] { "numberOfCups"})]
    public class BoilWaterHandler : IExternalTaskHandler
    {
        private readonly ILogger<BoilWaterHandler> _logger;
        public BoilWaterHandler(ILogger<BoilWaterHandler> logger)
        {
            _logger = logger;
        }
        public async Task<IExecutionResult> HandleAsync(ExternalTask externalTask, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Boil_Water handler is called from Camunda...");
            try
            {
                _logger.LogInformation($"Boiling water for {externalTask.Variables["numberOfCups"].AsInteger()} number of cups..........");
                //Mimicking operation
                Task.Delay(5000).Wait();

                //return success
                return new CompleteResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"error occured!! error messge: {ex.Message}");
                //return failure
                return new BpmnErrorResult("Boil_WaterFailure", "Error occured while invoking Boil_Water..");
            }
        }
    }
}
