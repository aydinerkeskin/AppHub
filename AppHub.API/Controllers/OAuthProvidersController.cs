using Microsoft.AspNetCore.Mvc;
using AppHub.Application.DTOs.OAuthProvider;
using AppHub.Application.Services;

namespace AppHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OAuthProvidersController : ControllerBase
{
    private readonly IOAuthProviderService _service;

    public OAuthProvidersController(IOAuthProviderService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OAuthProviderDto>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
            return NotFound();
        
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OAuthProviderDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<OAuthProviderDto>> Create([FromBody] CreateOAuthProviderDto createDto)
    {
        var result = await _service.CreateAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<OAuthProviderDto>> Update(int id, [FromBody] UpdateOAuthProviderDto updateDto)
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

