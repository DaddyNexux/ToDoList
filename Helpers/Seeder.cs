using Microsoft.AspNetCore.Identity;
using ToDoList.Data;
using ToDoList.Models.Entities;



namespace ToDoList.Helpers;

public class Seeder
{
    private readonly AppData _masterDbContext;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public Seeder(AppData masterDbContext, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _masterDbContext = masterDbContext;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task SeedSuperAdmin(string email, string password)
    {
        // Ensure roles exist first
        await EnsureRolesCreated();

        var superAdmin = await _userManager.FindByEmailAsync(email);
        if (superAdmin == null)
        {
            superAdmin = new User
            {
                UserName = email,
                Email = email,
                PhoneNumber = "123456789",
                FullName = "Osama Faisal"
            };

            var result = await _userManager.CreateAsync(superAdmin, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(superAdmin, Roles.SuperAdmin);
            }
            else
            {
                throw new Exception("Failed to create SuperAdmin user.");
            }
        }
    }
    public async Task EnsureRolesCreated()
    {
        string[] roles = { Roles.SuperAdmin, Roles.Admin, Roles.user };

        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }





}
