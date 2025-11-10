using Microsoft.AspNetCore.Mvc;
using AppHub.Application.DTOs.UserOAuth;
using AppHub.Application.Services;

namespace AppHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserOAuthsController : ControllerBase
{
    private readonly IUserOAuthService _service;

    public UserOAuthsController(IUserOAuthService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserOAuthDto>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
            return NotFound();
        
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserOAuthDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<UserOAuthDto>> Create([FromBody] CreateUserOAuthDto createDto)
    {
        var result = await _service.CreateAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserOAuthDto>> Update(int id, [FromBody] UpdateUserOAuthDto updateDto)
    {
        var result = await _service.UpdateAsync(id, updateDto);
        if (result == null)
            return NotFound();
        
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result)
            return NotFound();
        
        return NoContent();
    }
}

