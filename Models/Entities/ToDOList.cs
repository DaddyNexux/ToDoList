using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoList.Models.Entities
{
    public class ToDOList : BaseEntity
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool IsCompleted { get; set; } = false;

        [ForeignKey("User")]
        public string UserId { get; set; }
        public User? User { get; set; }
    }
}
