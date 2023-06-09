using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;


namespace TODO
{
    public class TODO_Model
    {
        public ObjectId Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }
        public int List_index { get; set; }
        public int List_position { get; set; }

    }

    public class Filter
    {
        [Required(ErrorMessage = "_id is required")]
        public string _id { get; set; }

    } 
    public class TODO_Update_Model
    {
        [Required(ErrorMessage = "Filter is required")]
        public string _id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int? List_index { get; set; }

        public int List_position { get; set; }

    }
    public class TODO_Response_Model
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int? List_index { get; set; }
        public int List_position { get; set; }
    }

    public class DragAndDropModel { 
        public string droppableId { get; set; }
        public int index { get; set; }
    }

    public class DragAndDropRrequest {
       public TODO_Update_Model UpdateRequest { get; set; }
        public DragAndDropModel source { get; set; }
        public DragAndDropModel Designation { get; set; }
    }
}