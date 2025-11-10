using Microsoft.AspNetCore.Mvc;
using AppHub.Application.DTOs.User;
using AppHub.Application.Services;

namespace AppHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;

    public UsersController(IUserService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
            return NotFound();
        
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto createDto)
    {
        var result = await _service.CreateAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserDto>> Update(int id, [FromBody] UpdateUserDto updateDto)
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

