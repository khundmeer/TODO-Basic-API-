﻿using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace TODO.Services
{
    public interface ITODO
    {
        public Task<TODO_Model> Insert(TODO_Model given_task);
        public UpdateResult Update(TODO_Update_Model update_request);
        public Task<List<TODO_Response_Model>> Get_All();

        public Task<DeleteResult> Delete_Task(string Todo_Id);

        public Task<TODO_Response_Model> Get_Todo_by_Id(string Todo_Id);

        public UpdateResult DragAndDrop(TODO_Update_Model update_request, DragAndDropModel source, DragAndDropModel designation);

    }
}
