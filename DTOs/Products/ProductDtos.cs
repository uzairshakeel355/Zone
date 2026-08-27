using System.ComponentModel.DataAnnotations;

namespace ShopZone.Api.DTOs.Products;

public record ProductResponseDto(
    int Id, string Name, string Sku, string Description, decimal Price,
    int StockQuantity, string ImageUrl, int CategoryId, string CategoryName
);

public record CreateProductDto(
    [Required] string Name,
    [Required] string Sku,
    string Description,
    [Range(0.01, double.MaxValue)] decimal Price,
    [Range(0, int.MaxValue)] int StockQuantity,
    string ImageUrl,
    [Required] int CategoryId
);

public record UpdateProductDto(
    [Required] string Name,
    string Description,
    [Range(0.01, double.MaxValue)] decimal Price,
    [Range(0, int.MaxValue)] int StockQuantity,
    string ImageUrl,
    [Required] int CategoryId
);