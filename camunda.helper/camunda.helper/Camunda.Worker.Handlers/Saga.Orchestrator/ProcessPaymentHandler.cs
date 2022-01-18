using Camunda.Worker;

namespace camunda.helper.Camunda.Worker.Handlers.Saga.Orchestrator
{
    [HandlerTopics("Process_Payment")]
    [HandlerVariables(new string[] { "exceptionInProcess" })]
    public class ProcessPaymentHandler : IExternalTaskHandler
    {
        private readonly ILogger<ProcessPaymentHandler> _logger;
        public ProcessPaymentHandler(ILogger<ProcessPaymentHandler> logger)
        {
            _logger = logger;
        }
        public async Task<IExecutionResult> HandleAsync(ExternalTask externalTask, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Process_Payment handler is called from Camunda...");
            try
            {
                //_logger.LogInformation($"Adding Tea leaves for {externalTask.Variables["numberOfCups"].AsInteger()} number of cups..........");
                ////Mimicking operation
                //Task.Delay(5000).Wait();
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
