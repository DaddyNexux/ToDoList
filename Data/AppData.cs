using ToDoList.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ToDoList.Data
{
    public class AppData : IdentityDbContext<User>
    {
        public AppData(DbContextOptions<AppData> options) : base(options) { }

     

        public DbSet<ToDOList> ToDOLists { get; set; }


    }
}
