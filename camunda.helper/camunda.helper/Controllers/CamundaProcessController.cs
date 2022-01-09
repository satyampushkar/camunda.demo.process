using Camunda.Api.Client;
using Camunda.Api.Client.ProcessDefinition;
using Camunda.Api.Client.UserTask;
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

        [HttpGet("startProcess")]
        public async Task<IActionResult> StartProcess(MyBPMNProcess myBPMNProcess)
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
                var proceStartResult = await _client.ProcessDefinitions.ByKey(myBPMNProcess.ToString())
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

        [HttpGet("tasks")]
        public async Task<IActionResult> GetUserTasks(MyBPMNProcess myBPMNProcess)
        {
            _logger.LogInformation($"Fetching user tasks for {myBPMNProcess}...");
            try
            {
                var userTaskQuery = new TaskQuery { ProcessDefinitionKey = myBPMNProcess.ToString() };
                var userTasks = await _client.UserTasks.Query(userTaskQuery).List();

                _logger.LogInformation($"user tasks fetched for {myBPMNProcess}....");

                return Ok(userTasks.Select(x=>x.Id).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError($"error occured!! error messge: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("completeUserTask/{id}")]
        public async Task<IActionResult> CompleteUserTask(string id) 
        {
            _logger.LogInformation($"Marking the manual task {id} complete...");
            try
            {
                Random random = new Random();

                //Fetch the manual task to be marked as complete
                var task = await _client.UserTasks[id].Get();
                var completeTask = new CompleteTask()
                    // Setting variable numberOfCups between 1-7 so that avaialability clause go through 
                    .SetVariable("numberOfCups", random.Next(1, 7)); 
                //Mark the manual task as complete
                await _client.UserTasks[id].Complete(completeTask);

                _logger.LogInformation($"Manual task {id} completed. Inventory is full now.......");

                return Ok($"Manual task {id} completed. Inventory is full now.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"error occured!! error messge: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }
    }

    public enum MyBPMNProcess
    {
        Process_Prepare_Tea,
        Process_Check_Items_Availability
    }
}
