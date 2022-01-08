using Camunda.Worker;

namespace camunda.helper.Camunda.Worker.Handlers
{
    [HandlerTopics("Strain_and_Serve")]
    [HandlerVariables(new string[] { "numberOfCups" })]
    public class StrainAndServeHandler : IExternalTaskHandler
    {
        private readonly ILogger<StrainAndServeHandler> _logger;
        public StrainAndServeHandler(ILogger<StrainAndServeHandler> logger)
        {
            _logger = logger;
        }
        public async Task<IExecutionResult> HandleAsync(ExternalTask externalTask, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Strain_and_Serve handler is called from Camunda...");

            try
            {
                _logger.LogInformation($"Serving tea in {externalTask.Variables["numberOfCups"].AsInteger()} number of cups..........");
                //Mimicking operation
                Task.Delay(5000, cancellationToken).Wait(cancellationToken);

                //return success
                return new CompleteResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"error occured!! error messge: {ex.Message}");
                //return failure
                return new BpmnErrorResult("Strain_and_ServeFailure", "Error occured while invoking Strain_and_Serve..");
            }
        }
    }
}
