using AutoMapper;
using AppHub.Application.DTOs.UserOAuth;
using AppHub.Domain.Entities;
using AppHub.Infrastructure.UnitOfWork;

namespace AppHub.Application.Services;

public class UserOAuthService : IUserOAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserOAuthService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<UserOAuthDto?> GetByIdAsync(int id)
    {
        var entity = await _unitOfWork.Repository<UserOAuth>().GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<UserOAuthDto>(entity);
    }

    public async Task<IEnumerable<UserOAuthDto>> GetAllAsync()
    {
        var entities = await _unitOfWork.Repository<UserOAuth>().GetAllAsync();
        return _mapper.Map<IEnumerable<UserOAuthDto>>(entities);
    }

    public async Task<UserOAuthDto> CreateAsync(CreateUserOAuthDto createDto)
    {
        var entity = _mapper.Map<UserOAuth>(createDto);
        await _unitOfWork.Repository<UserOAuth>().AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<UserOAuthDto>(entity);
    }

    public async Task<UserOAuthDto?> UpdateAsync(int id, UpdateUserOAuthDto updateDto)
    {
        var entity = await _unitOfWork.Repository<UserOAuth>().GetByIdAsync(id);
        if (entity == null) return null;

        _mapper.Map(updateDto, entity);
        _unitOfWork.Repository<UserOAuth>().Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<UserOAuthDto>(entity);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _unitOfWork.Repository<UserOAuth>().GetByIdAsync(id);
        if (entity == null) return false;

        _unitOfWork.Repository<UserOAuth>().Remove(entity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}

