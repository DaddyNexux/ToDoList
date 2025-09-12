using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models.Entities
{
    public class BaseEntity
    {
        [Key] public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; } = null;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; } = null;
    }
}
