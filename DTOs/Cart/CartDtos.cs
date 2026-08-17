using System.ComponentModel.DataAnnotations;

namespace ShopZone.Api.DTOs.Cart;

public record CartItemResponseDto(
    int ProductId,
    string ProductName,
    string ImageUrl,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal
);

public record CartResponseDto(int Id, List<CartItemResponseDto> Items, decimal Total);

public record AddCartItemDto(
    [Required] int ProductId,
    [Range(1, int.MaxValue)] int Quantity
);

public record UpdateCartItemDto([Range(1, int.MaxValue)] int Quantity);