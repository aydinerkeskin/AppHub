using AutoMapper;
using AppHub.Application.DTOs.OAuthProvider;
using AppHub.Domain.Entities;
using AppHub.Infrastructure.UnitOfWork;

namespace AppHub.Application.Services;

public class OAuthProviderService : IOAuthProviderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OAuthProviderService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<OAuthProviderDto?> GetByIdAsync(int id)
    {
        var entity = await _unitOfWork.Repository<OAuthProvider>().GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<OAuthProviderDto>(entity);
    }

    public async Task<IEnumerable<OAuthProviderDto>> GetAllAsync()
    {
        var entities = await _unitOfWork.Repository<OAuthProvider>().GetAllAsync();
        return _mapper.Map<IEnumerable<OAuthProviderDto>>(entities);
    }

    public async Task<OAuthProviderDto> CreateAsync(CreateOAuthProviderDto createDto)
    {
        var entity = _mapper.Map<OAuthProvider>(createDto);
        await _unitOfWork.Repository<OAuthProvider>().AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<OAuthProviderDto>(entity);
    }

    public async Task<OAuthProviderDto?> UpdateAsync(int id, UpdateOAuthProviderDto updateDto)
    {
        var entity = await _unitOfWork.Repository<OAuthProvider>().GetByIdAsync(id);
        if (entity == null) return null;

        _mapper.Map(updateDto, entity);
        _unitOfWork.Repository<OAuthProvider>().Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<OAuthProviderDto>(entity);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _unitOfWork.Repository<OAuthProvider>().GetByIdAsync(id);
        if (entity == null) return false;

        _unitOfWork.Repository<OAuthProvider>().Remove(entity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}

