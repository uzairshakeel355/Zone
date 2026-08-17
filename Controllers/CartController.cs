using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopZone.Api.Data;
using ShopZone.Api.DTOs.Cart;
using ShopZone.Api.Models;

namespace ShopZone.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public CartController(ApplicationDbContext db) => _db = db;

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    private async Task<Cart> GetOrCreateCartAsync()
    {
        var cart = await _db.Carts
            .Include(c => c.CartItems).ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.UserId == UserId);

        if (cart is not null) return cart;

        cart = new Cart { UserId = UserId };
        _db.Carts.Add(cart);
        await _db.SaveChangesAsync();
        return cart;
    }

    private static CartResponseDto ToDto(Cart cart)
    {
        var items = cart.CartItems.Select(ci => new CartItemResponseDto(
            ci.ProductId, ci.Product.Name, ci.Product.ImageUrl, ci.Product.Price, ci.Quantity, ci.Product.Price * ci.Quantity
        )).ToList();

        return new CartResponseDto(cart.Id, items, items.Sum(i => i.LineTotal));
    }

    [HttpGet]
    public async Task<ActionResult<CartResponseDto>> GetCart()
        => Ok(ToDto(await GetOrCreateCartAsync()));

    [HttpPost("items")]
    public async Task<ActionResult<CartResponseDto>> AddItem(AddCartItemDto dto)
    {
        var product = await _db.Products.FindAsync(dto.ProductId);
        if (product is null) return NotFound(new { message = "Product not found." });

        var cart = await GetOrCreateCartAsync();
        var existing = cart.CartItems.FirstOrDefault(ci => ci.ProductId == dto.ProductId);
        var requestedQuantity = (existing?.Quantity ?? 0) + dto.Quantity;

        if (requestedQuantity > product.StockQuantity)
            return BadRequest(new { message = $"Only {product.StockQuantity} in stock." });

        if (existing is not null)
            existing.Quantity = requestedQuantity;
        else
            _db.CartItems.Add(new CartItem { CartId = cart.Id, ProductId = dto.ProductId, Quantity = dto.Quantity });

        await _db.SaveChangesAsync();
        return Ok(ToDto(await GetOrCreateCartAsync()));
    }

    [HttpPut("items/{productId}")]
    public async Task<ActionResult<CartResponseDto>> UpdateItem(int productId, UpdateCartItemDto dto)
    {
        var cart = await GetOrCreateCartAsync();
        var item = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
        if (item is null) return NotFound(new { message = "Item not in cart." });

        if (dto.Quantity > item.Product.StockQuantity)
            return BadRequest(new { message = $"Only {item.Product.StockQuantity} in stock." });

        item.Quantity = dto.Quantity;
        await _db.SaveChangesAsync();
        return Ok(ToDto(await GetOrCreateCartAsync()));
    }

    [HttpDelete("items/{productId}")]
    public async Task<ActionResult<CartResponseDto>> RemoveItem(int productId)
    {
        var cart = await GetOrCreateCartAsync();
        var item = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
        if (item is null) return NotFound(new { message = "Item not in cart." });

        _db.CartItems.Remove(item);
        await _db.SaveChangesAsync();
        return Ok(ToDto(await GetOrCreateCartAsync()));
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        var cart = await GetOrCreateCartAsync();
        _db.CartItems.RemoveRange(cart.CartItems);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}