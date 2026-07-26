using System.ComponentModel.DataAnnotations;

namespace ShopZone.Api.DTOs.Auth;

public record RegisterDto(
    [Required, EmailAddress] string Email,
    [Required, MinLength(8)] string Password,
    [Required] string FirstName,
    [Required] string LastName
);

public record LoginDto(
    [Required, EmailAddress] string Email,
    [Required] string Password
);

public record AuthResponseDto(
    string Token,
    DateTime ExpiresAt,
    string Email,
    string FirstName,
    string LastName,
    IList<string> Roles
);