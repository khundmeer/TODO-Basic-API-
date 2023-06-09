using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using MongoDB.Driver;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System;
using TODO.Services;

namespace TODO.Controllers
{
    [ApiController]//to show that this is an api we write
    [Route("[controller]/[action]")]
    public class TODOController : ControllerBase
    {

        private readonly ILogger<TODOController> _logger;
        private readonly ITODO _todoService;
        public TODOController(ILogger<TODOController> logger, ITODO todo_Service)
        {
            _logger = logger;
            _todoService = todo_Service;
        }

        [HttpGet(Name = "GetAllTODOs")]
        public async Task<IActionResult> Get_All()
        {
            return Ok(await _todoService.Get_All());
        }

        [HttpGet(Name= "GetTodoById")]
        public async Task<IActionResult> Get_Todo_by_Id(string Todo_Id)
        {
            return Ok(await _todoService.Get_Todo_by_Id(Todo_Id));
        }



        [HttpPost]
        [ActionName("Insert")]
        public IActionResult Insert(TODO_Model given_task)
        {
            if (given_task == null)
            {
                return StatusCode(402, "Null Task given.");
            }
            else
            {

                return Ok(_todoService.Insert(given_task));
            }
        }

        [HttpPut]
        [ActionName("UpdateTODOTask")]
        public async Task<IActionResult> Update(TODO_Update_Model update_request)
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

                var res = _todoService.Update(update_request);
                return Ok(res);
            }

        }

        [HttpPut]
        [ActionName("DragAndDrop")]
        public async Task<IActionResult> DragAndDrop(DragAndDropRrequest dragAndDropRequest)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(dragAndDropRequest.UpdateRequest);
            bool isValid = Validator.TryValidateObject(dragAndDropRequest.UpdateRequest, context, results, true);

            if (!isValid)
            {
                return StatusCode(402, "Null is passed for update_request or one of the" +
                    "required elements.");
            }
            else
            {

                var res = _todoService.DragAndDrop(dragAndDropRequest.UpdateRequest, dragAndDropRequest.source, dragAndDropRequest.Designation);
                return Ok(res);
            }

        }

        [HttpDelete]
        [ActionName("DeleteTODOTask")]
        public async Task<DeleteResult> Delete_Task(string Todo_Id)
        {
            return await _todoService.Delete_Task(Todo_Id);

        }

        








    }
}