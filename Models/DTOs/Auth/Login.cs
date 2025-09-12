namespace ToDoList.Models.DTOs.Auth
{
    public class LoginDTO
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }

    public class LoginResponseDTO
    {
        public string Token { get; set; }
        public string Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }

    }

    public class RegisterDTO
    {
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
