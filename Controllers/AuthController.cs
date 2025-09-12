using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Models.DTOs.Auth;
using ToDoList.Models.DTOs.Common;
using ToDoList.Services.Auth;

namespace ToDoList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authServices;

        public AuthController(IAuthServices authServices)
        {
            _authServices = authServices;
        }

    
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Fail("Invalid login data"));

            var response = await _authServices.Login(form);

            // If response is null, it means user not found (custom handling)
            if (response == null)
                return NotFound(ApiResponse<string>.Fail("User not found", 404));

            return StatusCode(response.StatusCode, response);
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Fail("Invalid registration data"));

            var response = await _authServices.Register(form);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpGet("secure-endpoint")]
        public IActionResult SecureEndpoint()
        {
            return Ok("You are authorized!");
        }
    }
}
