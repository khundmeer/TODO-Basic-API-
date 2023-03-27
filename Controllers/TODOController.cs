using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace TODO.Controllers
{
    [ApiController]//to show that this is an api we write
    [Route("[controller]")]
    public class TODOController : ControllerBase
    {

        private readonly ILogger<TODOController> _logger;

        public TODOController(ILogger<TODOController> logger)
        {
            _logger = logger;
        }

        //FIRST END POINT
        [HttpGet(Name = "GetTODOList")]
        public IActionResult GetTODOList(int num_of_tasks_to_generate)
        {
            if (num_of_tasks_to_generate <= 0)
            {
                return StatusCode(402, "Invalid Input");
            }
            var list_of_tasks = new List<TODO_Model>();

            for (int i = 0; i < num_of_tasks_to_generate; i++)
            {
                list_of_tasks.Add(new TODO_Model()
                {
                    Id = i,
                    Title = $"task {i}",
                    Description = "This is your task to complete",
                    Is_completed = false
                });
            }

            return Ok(list_of_tasks);
        }



        [HttpPost]
        [ActionName("MakeTODOTask")]
        public IActionResult MakeTODOTask(TODO_Model given_task)
        {
            if(given_task == null)
            {
                return StatusCode(402, "Null Task given.");
            }

            TODO_Model created_task = new TODO_Model() {Id = given_task.Id, Title = given_task.Title,
            Description = given_task.Description, Is_completed = given_task.Is_completed};

            return Ok(created_task);
        }
    
    }
}