using AppHub.Application.DTOs.Application;

namespace AppHub.Application.Services;

public interface IApplicationService
{
    Task<ApplicationDto?> GetByIdAsync(int id);
    Task<IEnumerable<ApplicationDto>> GetAllAsync();
    Task<ApplicationDto> CreateAsync(CreateApplicationDto createDto);
    Task<ApplicationDto?> UpdateAsync(int id, UpdateApplicationDto updateDto);
    Task<bool> DeleteAsync(int id);
}

