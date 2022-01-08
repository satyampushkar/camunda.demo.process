using Camunda.Worker;

namespace camunda.helper.Camunda.Worker.Handlers
{
    [HandlerTopics("Add_TeaLeaves")]
    [HandlerVariables(new string[] { "numberOfCups" })]
    public class AddTeaLeavesHandler : IExternalTaskHandler
    {
        private readonly ILogger<AddTeaLeavesHandler> _logger;
        public AddTeaLeavesHandler(ILogger<AddTeaLeavesHandler> logger)
        {
            _logger = logger;
        }
        public async Task<IExecutionResult> HandleAsync(ExternalTask externalTask, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Add_TeaLeaves handler is called from Camunda...");
            try
            {
                _logger.LogInformation($"Adding Tea leaves for {externalTask.Variables["numberOfCups"].AsInteger()} number of cups..........");
                //Mimicking operation
                Task.Delay(5000).Wait();

                //return success
                return new CompleteResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"error occured!! error messge: {ex.Message}");
                //return failure
                return new BpmnErrorResult("Add_TeaLeavesFailure", "Error occured while invoking Add_TeaLeaves..");
            }
        }
    }
}
