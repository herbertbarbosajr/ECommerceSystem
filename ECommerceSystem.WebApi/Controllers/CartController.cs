using ECommerceSystem.Application.Commands;
using ECommerceSystem.Application.DTOs;
using ECommerceSystem.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceSystem.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly IMediator _mediator;

    public CartController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();

        var query = new GetCartQuery { UserId = userId };
        var cart = await _mediator.Send(query);

        if (cart == null)
            return Ok(new CartDto()); // Return empty cart

        return Ok(cart);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddCartItem([FromBody] AddCartItemDto cartItemDto)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();

        var command = new AddCartItemCommand { UserId = userId, CartItem = cartItemDto };
        var cartItemId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetCart), new { }, null);
    }

    [HttpPut("items/{id}")]
    public async Task<IActionResult> UpdateCartItem(int id, [FromBody] UpdateCartItemDto cartItemDto)
    {
        var command = new UpdateCartItemCommand { CartItemId = id, CartItem = cartItemDto };
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("items/{id}")]
    public async Task<IActionResult> RemoveCartItem(int id)
    {
        var command = new RemoveCartItemCommand { CartItemId = id };
        await _mediator.Send(command);
        return NoContent();
    }
}
