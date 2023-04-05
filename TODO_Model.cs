using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;


namespace TODO
{
    public class TODO_Model
    {
        public ObjectId Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public bool Is_completed { get; set; }

    }

    public class Filter
    {
        [Required(ErrorMessage = "_id is required")]
        public string _id { get; set; }

    } 
    public class TODO_Update_Model
    {
        [Required(ErrorMessage = "Filter is required")]
        public Filter Filter { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        public bool? Is_completed { get; set; }
    }
    public class TODO_Response_Model
    {
        public string Id { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        public bool Is_completed { get; set; }

    }
}

