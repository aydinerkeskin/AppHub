using AppHub.Application.DTOs.User;

namespace AppHub.Application.Services;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(int id);
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto> CreateAsync(CreateUserDto createDto);
    Task<UserDto?> UpdateAsync(int id, UpdateUserDto updateDto);
    Task<bool> DeleteAsync(int id);
}

