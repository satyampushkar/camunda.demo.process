using Camunda.Api.Client;
using Camunda.Api.Client.ProcessDefinition;
using Microsoft.AspNetCore.Mvc;

namespace camunda.helper.Controllers
{
    public class CamundaProcessController : ControllerBase
    {
        private readonly ILogger<CamundaProcessController> _logger;
        private CamundaClient _client;
        public CamundaProcessController(ILogger<CamundaProcessController> logger)
        {
            _logger = logger;
            _client = CamundaClient.Create("http://127.0.0.1:6060/engine-rest");
        }

        [HttpGet]
        [Route("startProcess")]
        public async Task<IActionResult> StartProcess()
        {
            _logger.LogInformation("Starting the sample Camunda process...");
            try
            {
                Random random = new Random();
                int numberOfCups = random.Next(1, 10);

                //Creating process parameters
                var processParams = new StartProcessInstance()
                    .SetVariable("numberOfCups", random.Next(1, 10));
                //Startinng the process
                var proceStartResult = await _client.ProcessDefinitions.ByKey("Process_Prepare_Tea")
                    .StartProcessInstance(processParams);

                _logger.LogInformation($"Camunda process: {proceStartResult.Id} to prepare tea started. Preaparing {numberOfCups} cup(s) of tea.........");

                return Ok(proceStartResult.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"error occured!! error messge: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
