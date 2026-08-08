using System.ComponentModel.DataAnnotations;

namespace ShopZone.Api.DTOs.Categories;

public record CategoryResponseDto(int Id, string Name, int ProductCount);

public record CreateCategoryDto([Required] string Name);
public record UpdateCategoryDto([Required] string Name);