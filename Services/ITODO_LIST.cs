using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace TODO.Services
{
    public interface ITODO
    {
        public Task<TODO_Model> Insert(TODO_Model given_task);
        public UpdateResult Update(TODO_Update_Model update_request);
        public Task<List<TODO_Response_Model>> Get_All();
    }
}
