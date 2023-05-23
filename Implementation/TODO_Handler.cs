using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Operations;
using System.ComponentModel.DataAnnotations;
using TODO.Controllers;
using TODO.Services;

namespace TODO.Implementation
{
    public class TODO_Handler : ITODO
    {
        private readonly ILogger<TODOController> _logger;
        private readonly IConfiguration _configuration;
        private readonly string mdb_conn_string;
        private readonly MongoClient client;
        private readonly IMongoDatabase database;
        private readonly IMongoCollection<TODO_Model> collection;

        public TODO_Handler(ILogger<TODOController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            mdb_conn_string = _configuration.GetValue<string>("MongoDbConnectionString");

            client = new MongoClient(mdb_conn_string);
            database = client.GetDatabase("SyedMongoDB");
            collection = database.GetCollection<TODO_Model>("TODO_List");
        }
        public async Task<TODO_Model> Insert(TODO_Model given_task)
        {
            //get count of items based a certain list index.
            var filter = Builders<TODO_Model>.Filter.Eq("List_index", given_task.List_index);

            var count = await collection.CountDocumentsAsync(filter);
            given_task.List_position = (int)count;

            var result = InsertMongodb(given_task).Result;
            return (result);

        }
        public UpdateResult Update(TODO_Update_Model update_request)
        {

            ObjectId update_req_obj_id = new ObjectId(update_request._id);
            var filter = Builders<TODO_Model>.Filter.Eq("_id", update_req_obj_id);

            var filtered_task = collection.Find(x => x.Id == update_req_obj_id).ToList<TODO_Model>();

            if (filtered_task != null)
            {
                var update = Builders<TODO_Model>.Update
                    .Set("Description", update_request.Description == "" ? filtered_task.FirstOrDefault().Description : update_request.Description)
                    .Set("Title", update_request.Title == "" ? filtered_task.FirstOrDefault().Title : update_request.Title)
                    .Set("Status", update_request.Status == "" ? filtered_task.FirstOrDefault().Status : update_request.Status)
                    .Set("List_index", update_request.List_index == null ? filtered_task.FirstOrDefault().List_index : update_request.List_index)
                    .Set("List_position", update_request.List_position == null ? filtered_task.FirstOrDefault().List_position : update_request.List_position);
                var result = collection.UpdateOne(filter, update);
                return result;
            }

            return null;
        }
        public async Task<List<TODO_Response_Model>> Get_All()
        {
            var collection = database.GetCollection<TODO_Model>("TODO_List");
            var list_of_tasks = await collection.Find(x => true).ToListAsync();

            var list = new List<TODO_Response_Model>();

            foreach (var val in list_of_tasks)
            {
                var obj = new TODO_Response_Model();
                obj.Id = val.Id.ToString();
                obj.Description = val.Description;
                obj.Title = val.Title;
                obj.Status = val.Status;
                obj.List_index = val.List_index;
                obj.List_position = val.List_position;
                list.Add(obj);


            }


            return list;
        }
        private async Task<TODO_Model> InsertMongodb(TODO_Model given_task)
        {
            try
            {
                await collection.InsertOneAsync(given_task);
                return given_task;
            }
            catch (Exception ex)
            {
                var erro = ex.Message;
            }
            return given_task;
        }


        public async Task<DeleteResult> Delete_Task(string Todo_Id)
        {
            var filter = Builders<TODO_Model>.Filter.Eq("_id", ObjectId.Parse(Todo_Id));

            var result = await collection.DeleteOneAsync(filter);
                
                //({ _id: ObjectId(Todo_Id)});

            return result;
        }

        public async Task<TODO_Response_Model> Get_Todo_by_Id(string Todo_Id)
        {
            var collection = database.GetCollection<TODO_Model>("TODO_List");

            ObjectId _id = new ObjectId(Todo_Id);

            var filter = Builders<TODO_Model>.Filter.Eq("_id", _id);

            var result = collection.Find(filter).FirstOrDefault();

            TODO_Response_Model ret_value = new TODO_Response_Model();
            ret_value.Id = result.Id.ToString();
            ret_value.Status = result.Status;
            ret_value.Title = result.Title;
            ret_value.Description = result.Description;
            ret_value.List_index = result.List_index;
            ret_value.List_position = result.List_position;

            return ret_value;
        }

        public UpdateResult DragAndDrop(TODO_Update_Model update_request, DragAndDropModel source, DragAndDropModel destination)
        {


            var sss = "";


            //update the list position of objects in the destination list index >

            //list index


            #region "Desgination List Postion Increment By 1"
            var listIndexFilter = Builders<TODO_Model>.Filter.Eq("List_index", destination.droppableId);
            var listPositionFilter = Builders<TODO_Model>.Filter.Gte("List_position", destination.index);

            var combinedFilter = Builders<TODO_Model>.Filter.And(listIndexFilter, listPositionFilter);

            // Create an update definition to increment the field by 1
            var update = Builders<TODO_Model>.Update.Inc("List_position", 1);

            // Perform the update
            var result = collection.UpdateMany(combinedFilter, update);
            #endregion

            //update the list position of objects in the source list index >




            

            #region "Move task from once coloumn to Other column"
            //update the object's list index and list position ==> destination index and position

            ObjectId update_req_obj_id = new ObjectId(update_request._id);
            var filter = Builders<TODO_Model>.Filter.Eq("_id", update_req_obj_id);

            // var filtered_task = collection.Find(x => x.Id == update_req_obj_id).ToList<TODO_Model>();

            int listindex = 0;
            int.TryParse(destination.droppableId, out listindex);
            var update_ChangeStatus = Builders<TODO_Model>.Update
                .Set("Status", update_request.Status)
                .Set("List_index", update_request.List_index = listindex)
                .Set("List_position", update_request.List_position = destination.index);
            var Updatedresult = collection.UpdateOne(filter, update_ChangeStatus);

            #endregion

            #region "Source List Postion decrement By 1"
             listIndexFilter = Builders<TODO_Model>.Filter.Eq("List_index", source.droppableId);
             listPositionFilter = Builders<TODO_Model>.Filter.Gte("List_position", source.index);

             combinedFilter = Builders<TODO_Model>.Filter.And(listIndexFilter, listPositionFilter);

            // Create an update definition to increment the field by 1
             update = Builders<TODO_Model>.Update.Inc("List_position", -1);

            // Perform the update
             result = collection.UpdateMany(combinedFilter, update);
            #endregion

            return result;
           

        }
    }
}
