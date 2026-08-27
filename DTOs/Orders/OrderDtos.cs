using System.ComponentModel.DataAnnotations;

namespace ShopZone.Api.DTOs.Orders;

public record CheckoutDto([Required] string ShippingAddress);

public record OrderItemResponseDto(int? ProductId, string ProductName, decimal UnitPrice, int Quantity, decimal LineTotal);

public record OrderResponseDto(
    int Id,
    DateTime OrderDate,
    string Status,
    string ShippingAddress,
    decimal TotalAmount,
    List<OrderItemResponseDto> Items
);