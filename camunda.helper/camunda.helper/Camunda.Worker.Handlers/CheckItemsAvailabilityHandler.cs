using Camunda.Worker;

namespace camunda.helper.Camunda.Worker.Handlers
{
    [HandlerTopics("Check_Items_Availability")]
    [HandlerVariables(new string[] { "numberOfCups" })]
    public class CheckItemsAvailabilityHandler : IExternalTaskHandler
    {
        private readonly ILogger<CheckItemsAvailabilityHandler> _logger;
        public CheckItemsAvailabilityHandler(ILogger<CheckItemsAvailabilityHandler> logger)
        {
            _logger = logger;
        }
        public async Task<IExecutionResult> HandleAsync(ExternalTask externalTask, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Check_Items_Availability handler is called from Camunda...");
            try
            {
                var numberOfCups = externalTask.Variables["numberOfCups"].AsInteger();
                _logger.LogInformation($"Checking availability of items for {numberOfCups} number of cups of tea..........");
                //Mimicking operation
                Task.Delay(5000).Wait();

                //
                bool availability = (numberOfCups > 7) ? false : true;

                //return success
                return new CompleteResult
                {
                    Variables = new Dictionary<string, Variable>
                    {
                        ["itemsAvailable"] = new Variable(availability, VariableType.Boolean)
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"error occured!! error messge: {ex.Message}");
                //return failure
                return new BpmnErrorResult("Check_Items_Availability", "Error occured while invoking Check_Items_Availability..");
            }
        }
    }
}
