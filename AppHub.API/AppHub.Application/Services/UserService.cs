using AutoMapper;
using AppHub.Application.DTOs.User;
using AppHub.Domain.Entities;
using AppHub.Infrastructure.UnitOfWork;
using ApplicationEntity = AppHub.Domain.Entities.Application;

namespace AppHub.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var entity = await _unitOfWork.Repository<User>().GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<UserDto>(entity);
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var entities = await _unitOfWork.Repository<User>().GetAllAsync();
        return _mapper.Map<IEnumerable<UserDto>>(entities);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto createDto)
    {
        var entity = _mapper.Map<User>(createDto);
        await _unitOfWork.Repository<User>().AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<UserDto>(entity);
    }

    public async Task<UserDto?> UpdateAsync(int id, UpdateUserDto updateDto)
    {
        var entity = await _unitOfWork.Repository<User>().GetByIdAsync(id);
        if (entity == null) return null;

        _mapper.Map(updateDto, entity);
        _unitOfWork.Repository<User>().Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<UserDto>(entity);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _unitOfWork.Repository<User>().GetByIdAsync(id);
        if (entity == null) return false;

        _unitOfWork.Repository<User>().Remove(entity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<UserDto?> LoginAsync(LoginRequestDto loginDto)
    {
        var user = await _unitOfWork.Repository<User>()
            .FirstOrDefaultAsync(u => u.Username == loginDto.Username 
                                      && u.PasswordHash == loginDto.PasswordHash
                                      && u.ApplicationId == loginDto.ApplicationId);

        return user == null ? null : _mapper.Map<UserDto>(user);
    }

    public async Task<bool> ExistsByApplicationAndUsernameOrEmailAsync(int applicationId, string username, string email)
    {
        return await _unitOfWork.Repository<User>()
            .ExistsAsync(u => u.ApplicationId == applicationId &&
                              (u.Username == username || u.Email == email));
    }
}

