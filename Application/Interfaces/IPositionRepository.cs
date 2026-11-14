using Application.DTOs.Positions;

namespace Application.Interfaces;

public interface IPositionRepository
{
    Task<List<PositionResponseDto>> GetAllAsync();

    Task<PositionResponseDto?> GetByIdAsync(int id);

    Task<int> CreateAsync(PositionCreateDto dto);
}