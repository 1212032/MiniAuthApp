namespace backend.Services;
using backend.Models;
using System.Security.Claims;

public interface IJwtService
{
    string GenerateToken(User user);
    int? ValidateToken(string token);
    ClaimsPrincipal GetPrincipalFromToken(string token);
    DateTime GetExpirationDate(string token);
}