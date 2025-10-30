namespace frontend.Models
{
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class UserInfo
    {
        public string Id { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public List<string> Roles { get; set; } = new();
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public UserInfo User { get; set; } = new();
    }
}

