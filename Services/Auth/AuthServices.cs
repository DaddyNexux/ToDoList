using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToDoList.Data;
using ToDoList.Helpers;
using ToDoList.Models.DTOs.Auth;
using ToDoList.Models.DTOs.Common;
using ToDoList.Models.Entities;

namespace   ToDoList.Services.Auth
{
    public interface IAuthServices
    {
        Task<ApiResponse<LoginResponseDTO?>> Login(LoginDTO form);
        Task<ApiResponse<LoginResponseDTO>> Register(RegisterDTO form);
    }
    public class AuthServices : IAuthServices
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppData _context;

        private readonly IConfiguration _configuration;

        public AuthServices(UserManager<User> u,
            SignInManager<User> s, IConfiguration configuration,
            RoleManager<IdentityRole> role,
            AppData context)
        {
            _userManager = u;
            _signInManager = s;
            _configuration = configuration;
            _roleManager = role;
            _context = context;
        }

        public async Task<ApiResponse<LoginResponseDTO?>> Login(LoginDTO form)
        {
            // Find user by username
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.UserName == form.Username);

            if (user == null)
                return ApiResponse<LoginResponseDTO?>.Fail("USER_NOT_FOUND", 404);

            // Verify password
            var result = await _signInManager.CheckPasswordSignInAsync(user, form.Password, false);
            if (!result.Succeeded)
                return ApiResponse<LoginResponseDTO?>.Fail("INVALID_PASSWORD", 400);

            // Get first role assigned to the user
            var roles = await _userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault();

            if (string.IsNullOrEmpty(userRole))
                return ApiResponse<LoginResponseDTO?>.Fail("USER_HAS_NO_ROLE", 400);

            // Read secret key from configuration
            var secretKey = _configuration["Jwt:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
                throw new InvalidOperationException("JWT SecretKey is missing in configuration");

            // Generate JWT token
            var token = JwtToken.GenToken(
                Guid.Parse(user.Id),
                userRole,
                "Supernova-iq.com",
                30,
                secretKey
            );

            // (Optional) load role entity if you need extra info
            var roleEntity = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == userRole);

            // Build response DTO
            var response = new LoginResponseDTO
            {
                Token = token,
                Id = user.Id,
                Username = user.UserName,
                Role = userRole,
                FullName = user.FullName
            };

            return ApiResponse<LoginResponseDTO?>.Success(response, "Login successful", 200);
        }

        public async Task<ApiResponse<LoginResponseDTO>> Register(RegisterDTO form)
        {
            var existingUser = await _userManager.FindByNameAsync(form.Username);
            if (existingUser != null)
                return ApiResponse<LoginResponseDTO>.Fail("Username already exists", 400);
            var user = new User
            {
                UserName = form.Username,
                FullName = form.FullName,
            };
            var createUserResult = await _userManager.CreateAsync(user, form.Password);
            if (!createUserResult.Succeeded)
            {
                var errors = string.Join(", ", createUserResult.Errors.Select(e => e.Description));
                return ApiResponse<LoginResponseDTO>.Fail($"User creation failed: {errors}", 400);
            }
            var roleExists = await _roleManager.RoleExistsAsync("User");
            if (!roleExists)
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole { Name = "User" });
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    return ApiResponse<LoginResponseDTO>.Fail($"Role creation failed: {errors}", 500);
                }
            }
            await _userManager.AddToRoleAsync(user, "User");
          
            var loginResponse = await Login(new LoginDTO
            {
                Username = form.Username,
                Password = form.Password
            });
            if (loginResponse == null || loginResponse.Data.Token == "INVALID_PASSWORD")
                return ApiResponse<LoginResponseDTO>.Fail("Registration succeeded but login failed.", 500);
            return ApiResponse<LoginResponseDTO>.Success(loginResponse.Data, "Registration and login successful", 201);

        }

       
    }
}
