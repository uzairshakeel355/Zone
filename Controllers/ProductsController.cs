using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopZone.Api.Data;
using ShopZone.Api.DTOs.Products;
using ShopZone.Api.Models;

namespace ShopZone.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public ProductsController(ApplicationDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetAll()
    {
        var products = await _db.Products
            .Include(p => p.Category)
            .Select(p => new ProductResponseDto(p.Id, p.Name, p.Sku, p.Description, p.Price, p.StockQuantity, p.ImageUrl, p.CategoryId, p.Category.Name))
            .ToListAsync();

        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponseDto>> GetById(int id)
    {
        var product = await _db.Products
            .Where(p => p.Id == id)
            .Select(p => new ProductResponseDto(p.Id, p.Name, p.Sku, p.Description, p.Price, p.StockQuantity, p.ImageUrl, p.CategoryId, p.Category.Name))
            .FirstOrDefaultAsync();

        return product is null ? NotFound() : Ok(product);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ProductResponseDto>> Create(CreateProductDto dto)
    {
        if (!await _db.Categories.AnyAsync(c => c.Id == dto.CategoryId))
            return BadRequest(new { message = "CategoryId does not exist." });

        if (await _db.Products.AnyAsync(p => p.Sku == dto.Sku))
            return Conflict(new { message = "SKU already exists." });

        var product = new Product
        {
            Name = dto.Name,
            Sku = dto.Sku,
            Description = dto.Description,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            ImageUrl = dto.ImageUrl,
            CategoryId = dto.CategoryId
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        var category = await _db.Categories.FindAsync(dto.CategoryId);
        var response = new ProductResponseDto(product.Id, product.Name, product.Sku, product.Description, product.Price, product.StockQuantity, product.ImageUrl, product.CategoryId, category!.Name);

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, response);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateProductDto dto)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null) return NotFound();

        if (!await _db.Categories.AnyAsync(c => c.Id == dto.CategoryId))
            return BadRequest(new { message = "CategoryId does not exist." });

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.StockQuantity = dto.StockQuantity;
        product.ImageUrl = dto.ImageUrl;
        product.CategoryId = dto.CategoryId;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null) return NotFound();

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}