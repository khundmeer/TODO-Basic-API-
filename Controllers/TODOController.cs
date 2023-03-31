using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using MongoDB.Driver;


namespace TODO.Controllers
{
    [ApiController]//to show that this is an api we write
    [Route("[controller]")]
    public class TODOController : ControllerBase
    {

        private readonly ILogger<TODOController> _logger;
        private readonly IConfiguration _configuration;
        private readonly string mdb_conn_string;
        private readonly MongoClient client;
        private readonly IMongoDatabase database;
        public TODOController(ILogger<TODOController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            mdb_conn_string = _configuration.GetValue<string>("MongoDbConnectionString");

            client = new MongoClient(mdb_conn_string);
            database = client.GetDatabase("SyedMongoDB");
        }

        //FIRST END POINT
        [HttpGet(Name = "GetTODOList")]
        public async Task<IActionResult> GetTODOList()
        {
            var collection = database.GetCollection<TODO_Model>("TODO_List");
            var list_of_tasks = await collection.Find(x => true).Skip(0).Limit(10).ToListAsync();


            return Ok(list_of_tasks);
        }



        [HttpPost]
        [ActionName("MakeTODOTask")]
        public IActionResult MakeTODOTask(TODO_Model given_task)
        {
            if (given_task == null)
            {
                return StatusCode(402, "Null Task given.");
            }

           

            var result = InsertMongodb(given_task).Result;
            return Ok(result);
        }

        private async Task<TODO_Model> InsertMongodb(TODO_Model given_task) {

            
            var collection = database.GetCollection<TODO_Model>("TODO_List");

            //var my_todo_1 = new TODO_Model {
   
            //    Title = given_task",
            //    Description = "This is your task to complete",
            //    Is_completed = false
            //};
            try
            {
                await collection.InsertOneAsync(given_task);
                return given_task;
            }
            catch(Exception ex) {
                var erro = ex.Message;
            }
            return given_task;


        }

    }
}