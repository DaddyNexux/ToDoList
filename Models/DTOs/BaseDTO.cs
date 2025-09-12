using System.Text.Json.Serialization;

namespace ToDoList.Models.DTOs
{
    public class BaseDTO
    {
        public Guid? Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class BaseFormDTO
    {
    }

    public class BaseUpdateDTO
    {
        [JsonIgnore] public Guid Id { get; set; }
    }

    public class BaseFilter
    {
        public Guid? Id { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
