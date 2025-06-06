namespace MinimalApiToDo.Models
{
    public class TodoItem
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public bool IsComplete { get; set; }

        public DateOnly? DueDate { get; set; }

        public string Category { get; set; }
    }
}