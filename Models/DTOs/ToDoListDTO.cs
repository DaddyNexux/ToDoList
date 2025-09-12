namespace ToDoList.Models.DTOs
{
    public class ToDoListResponseDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool IsCompleted { get; set; } = false;
    }
}
