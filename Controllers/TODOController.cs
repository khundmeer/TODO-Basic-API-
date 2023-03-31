using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using MongoDB.Driver;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System;

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
        private readonly IMongoCollection<TODO_Model> collection;
        public TODOController(ILogger<TODOController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            mdb_conn_string = _configuration.GetValue<string>("MongoDbConnectionString");

            client = new MongoClient(mdb_conn_string);
            database = client.GetDatabase("SyedMongoDB");
            collection = database.GetCollection<TODO_Model>("TODO_List");
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


        [HttpPut]
        [ActionName("UpdateTODOTask")]
        public async Task<IActionResult> UpdateTODOTask(TODO_Update_Model update_request)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(update_request);
            bool isValid = Validator.TryValidateObject(update_request, context, results, true);

            if (!isValid) {
                return StatusCode(402, "Null is passed for update_request or one of the" +
                    "required elements.");
            }
            else
            {
                //dynamic update_value = update_request.Filter.Key == "_id" ? new ObjectId(update_request.Filter.Value) : update_request.Filter.Value; update_value = update_request.Filter.Key == "_id" ? new ObjectId(update_request.Filter.Value) : 
                //    update_request.Filter.Value;
                ObjectId update_req_obj_id = new ObjectId(update_request.Filter._id);
                var filter = Builders<TODO_Model>.Filter.Eq("_id", update_req_obj_id);
                
                var filtered_task = collection.Find(x => x.Id == update_req_obj_id).ToList<TODO_Model>();
                
                if(filtered_task != null)
                {
                    var update = Builders<TODO_Model>.Update
                        .Set("Description", update_request.Description == "" ? filtered_task.FirstOrDefault().Description : update_request.Description)
                        .Set("Title", update_request.Title == "" ? filtered_task.FirstOrDefault().Title : update_request.Title)
                        .Set("Is_completed", update_request.Is_completed == null ? filtered_task.FirstOrDefault().Is_completed : update_request.Is_completed);
                    var result = collection.UpdateOne(filter, update);
                    return(Ok(result));
                }

                //        .Set("Is_Completed", update_request.Is_completed);

                //        .Set("Title", update_request.Title)
                //        .Set("Description", update_request.Description)
                //var update = Builders<TODO_Model>.Update

                return (StatusCode(500, "Unable to update."));
            }

        }
    }
}