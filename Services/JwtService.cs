using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ShopZone.Api.Models;

namespace ShopZone.Api.Services;

public interface IJwtService
{
    (string token, DateTime expiresAt) GenerateToken(ApplicationUser user, IList<string> roles);
}

public class JwtService : IJwtService
{
    private readonly IConfiguration _config;
    public JwtService(IConfiguration config) => _config = config;

    public (string token, DateTime expiresAt) GenerateToken(ApplicationUser user, IList<string> roles)
    {
        var jwt = _config.GetSection("Jwt");
        var keyValue = jwt["Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured.");
        var expiryMinutes = jwt["ExpiryMinutes"] ?? throw new InvalidOperationException("Jwt:ExpiryMinutes is not configured.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyValue));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
    {
        new(JwtRegisteredClaimNames.Sub, user.Id),
        new(JwtRegisteredClaimNames.Email, user.Email!),
        new(ClaimTypes.NameIdentifier, user.Id),
        new(ClaimTypes.GivenName, user.FirstName),
        new(ClaimTypes.Surname, user.LastName),
        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var expiresAt = DateTime.UtcNow.AddMinutes(double.Parse(expiryMinutes));

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}