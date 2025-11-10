using AppHub.Application.DTOs.OAuthProvider;

namespace AppHub.Application.Services;

public interface IOAuthProviderService
{
    Task<OAuthProviderDto?> GetByIdAsync(int id);
    Task<IEnumerable<OAuthProviderDto>> GetAllAsync();
    Task<OAuthProviderDto> CreateAsync(CreateOAuthProviderDto createDto);
    Task<OAuthProviderDto?> UpdateAsync(int id, UpdateOAuthProviderDto updateDto);
    Task<bool> DeleteAsync(int id);
}

