namespace backend.Services;
using Microsoft.EntityFrameworkCore;
using backend.Extensions;
using backend.Models;

public class AuthService(AppDbContext context, IJwtService jwtService, PasswordHasher passwordHasher) : IAuthService
{
    private readonly AppDbContext _context= context;
    private readonly IJwtService _jwtService= jwtService;
    private readonly PasswordHasher _passwordHasher= passwordHasher;
    
    public async Task<AuthResponse> LoginAsync(LoginRequest request){

        if (request == null)
            throw new ArgumentNullException(nameof(request));
            
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Credenciais inválidas");

        var token = _jwtService.GenerateToken(user);
        return new AuthResponse 
        { 
            Token = token, 
            UserId = user.Id,
            Email = user.Email,
            Name = user.Name
        };
    }
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request){
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        // Verificar se email já existe
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            throw new ArgumentException("Email já está em uso");

        // Validar password
        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
            throw new ArgumentException("Password deve ter pelo menos 6 caracteres");

        // Validar idade (opcional)
        if (DateTime.Now.Year - request.DateOfBirth.Year < 13)
            throw new ArgumentException("Utilizador deve ter pelo menos 13 anos");

        // Criar novo user
        var user = new User
        {
            Email = request.Email.Trim().ToLower(),
            PasswordHash = _passwordHasher.Hash(request.Password),
            Name = request.Name.Trim(),
            DateOfBirth = request.DateOfBirth,
            CreatedAt = DateTime.UtcNow
        };

        // Guardar na BD
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Gerar token
        var token = _jwtService.GenerateToken(user);
        return new AuthResponse 
        { 
            Token = token, 
            UserId = user.Id,
            Email = user.Email,
            Name = user.Name
        };
    }
    public async Task<AuthResponse> RefreshTokenAsync(string token){
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token inválido");

        try
        {
            // Validar token atual
            var userId = _jwtService.ValidateToken(token);
            if (userId == null)
                throw new UnauthorizedAccessException("Token inválido");

            // Buscar user
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new UnauthorizedAccessException("Utilizador não encontrado");

            // Gerar novo token
            var newToken = _jwtService.GenerateToken(user);
            return new AuthResponse 
            { 
                Token = newToken, 
                UserId = user.Id,
                Email = user.Email,
                Name = user.Name
            };
        }
        catch (Exception ex) when (ex is not UnauthorizedAccessException)
        {
            throw new UnauthorizedAccessException("Token inválido ou expirado");
        }
    }
    

}