using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoList.Models.DTOs.Common;
using ToDoList.Models.Entities;
using ToDoList.Services;

namespace ToDoList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoListController : ControllerBase
    {
        private readonly IToDoListService _toDoService;
        private readonly UserManager<User> _userManager;

        public ToDoListController(IToDoListService toDoService, UserManager<User> userManager)
        {
            _toDoService = toDoService;
            _userManager = userManager;

        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _toDoService.GetByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }


        [HttpGet("all")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetAllWithUser()
        {
            var result = await _toDoService.GetAllWithUserAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Authorize] // Require any valid token
        public async Task<IActionResult> Create([FromBody] CreateToDoRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Fail("Invalid request data"));

            var user = await GetUser();
            if (user == null)
                return Unauthorized(ApiResponse<string>.Fail("Invalid token or user not found"));

            var result = await _toDoService.CreateAsync(user.Id, request.Title, request.Description);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("my")]
        [Authorize] // Require authentication
        public async Task<IActionResult> GetByUser()
        {
            var user = await GetUser();
            if (user == null)
                return Unauthorized(ApiResponse<string>.Fail("Invalid token or user not found"));

            var result = await _toDoService.GetByUserAsync(user.Id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPatch("{id:guid}/toggle")]
        [Authorize]
        public async Task<IActionResult> ToggleComplete(Guid id)
        {
            var user = await GetUser();
            if (user == null)
                return Unauthorized(ApiResponse<string>.Fail("Invalid token or user not found"));

            var result = await _toDoService.ToggleCompleteAsync(user.Id, id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateToDoRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Fail("Invalid request data"));

            var user = await GetUser();
            if (user == null)
                return Unauthorized(ApiResponse<string>.Fail("Invalid token or user not found"));

            var result = await _toDoService.UpdateAsync(user.Id, id, request.Title, request.Description);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await GetUser();
            if (user == null)
                return Unauthorized(ApiResponse<string>.Fail("Invalid token or user not found"));

            var result = await _toDoService.DeleteAsync(user.Id, id);
            return StatusCode(result.StatusCode, result);
        }

        private async Task<User?> GetUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                return null;

            // Retrieve the full user from UserManager
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return null;
            return user;
        }
    }
    public class CreateToDoRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class UpdateToDoRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
