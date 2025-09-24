namespace backend.Models;

public class AuthResponse
{
    public int UserId { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Token { get; set; }
    public int ExpiresIn { get; set; }
}
