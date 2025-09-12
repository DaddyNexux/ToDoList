using Microsoft.AspNetCore.Identity;

namespace ToDoList.Models.Entities
{
    public class User : IdentityUser
    {
        public required string FullName { get; set; }
    }
}
