using MongoDB.Bson;

namespace TODO
{
    public class TODO_Model
    {
        public ObjectId Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public bool Is_completed { get; set; }

    }
}