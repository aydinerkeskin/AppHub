using AppHub.Application.DTOs.UserOAuth;

namespace AppHub.Application.Services;

public interface IUserOAuthService
{
    Task<UserOAuthDto?> GetByIdAsync(int id);
    Task<IEnumerable<UserOAuthDto>> GetAllAsync();
    Task<UserOAuthDto> CreateAsync(CreateUserOAuthDto createDto);
    Task<UserOAuthDto?> UpdateAsync(int id, UpdateUserOAuthDto updateDto);
    Task<bool> DeleteAsync(int id);
}

