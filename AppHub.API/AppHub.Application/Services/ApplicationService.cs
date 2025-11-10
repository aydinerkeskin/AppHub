using AutoMapper;
using AppHub.Application.DTOs.Application;
using AppHub.Domain.Entities;
using AppHub.Infrastructure.UnitOfWork;
using ApplicationEntity = AppHub.Domain.Entities.Application;

namespace AppHub.Application.Services;

public class ApplicationService : IApplicationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ApplicationService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApplicationDto?> GetByIdAsync(int id)
    {
        var entity = await _unitOfWork.Repository<ApplicationEntity>().GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<ApplicationDto>(entity);
    }

    public async Task<IEnumerable<ApplicationDto>> GetAllAsync()
    {
        var entities = await _unitOfWork.Repository<ApplicationEntity>().GetAllAsync();
        return _mapper.Map<IEnumerable<ApplicationDto>>(entities);
    }

    public async Task<ApplicationDto> CreateAsync(CreateApplicationDto createDto)
    {
        var entity = _mapper.Map<ApplicationEntity>(createDto);
        await _unitOfWork.Repository<ApplicationEntity>().AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<ApplicationDto>(entity);
    }

    public async Task<ApplicationDto?> UpdateAsync(int id, UpdateApplicationDto updateDto)
    {
        var entity = await _unitOfWork.Repository<ApplicationEntity>().GetByIdAsync(id);
        if (entity == null) return null;

        _mapper.Map(updateDto, entity);
        _unitOfWork.Repository<ApplicationEntity>().Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<ApplicationDto>(entity);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _unitOfWork.Repository<ApplicationEntity>().GetByIdAsync(id);
        if (entity == null) return false;

        _unitOfWork.Repository<ApplicationEntity>().Remove(entity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}

