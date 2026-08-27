using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopZone.Api.Data;
using ShopZone.Api.DTOs.Orders;
using ShopZone.Api.Models;

namespace ShopZone.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public OrdersController(ApplicationDbContext db) => _db = db;

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpPost]
    public async Task<ActionResult<OrderResponseDto>> Checkout(CheckoutDto dto)
    {
        var cart = await _db.Carts
            .Include(c => c.CartItems).ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.UserId == UserId);

        if (cart is null || cart.CartItems.Count == 0)
            return BadRequest(new { message = "Your cart is empty." });

        foreach (var item in cart.CartItems)
        {
            if (item.Quantity > item.Product.StockQuantity)
                return BadRequest(new { message = $"Only {item.Product.StockQuantity} of {item.Product.Name} left in stock." });
        }

        var order = new Order
        {
            UserId = UserId,
            ShippingAddress = dto.ShippingAddress,
            TotalAmount = cart.CartItems.Sum(ci => ci.Product.Price * ci.Quantity)
        };

        foreach (var item in cart.CartItems)
        {
            order.OrderItems.Add(new OrderItem
            {
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                UnitPrice = item.Product.Price,
                Quantity = item.Quantity
            });

            item.Product.StockQuantity -= item.Quantity;
        }

        _db.Orders.Add(order);
        _db.CartItems.RemoveRange(cart.CartItems);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { message = "Stock changed while you were checking out. Please review your cart and try again." });
        }

        return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, ToDto(order));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderResponseDto>> GetOrderById(int id)
    {
        var order = await _db.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == UserId);

        return order is null ? NotFound() : Ok(ToDto(order));
    }

    private static OrderResponseDto ToDto(Order order) => new(
        order.Id, order.OrderDate, order.Status.ToString(), order.ShippingAddress, order.TotalAmount,
        order.OrderItems.Select(oi => new OrderItemResponseDto(oi.ProductId, oi.ProductName, oi.UnitPrice, oi.Quantity, oi.UnitPrice * oi.Quantity)).ToList()
    );

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetMyOrders()
    {
        var orders = await _db.Orders
            .Include(o => o.OrderItems)
            .Where(o => o.UserId == UserId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return Ok(orders.Select(ToDto));
    }
}