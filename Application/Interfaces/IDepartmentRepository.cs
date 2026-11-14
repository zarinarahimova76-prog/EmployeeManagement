using Application.DTOs.Departments;

namespace Application.Interfaces;

public interface IDepartmentRepository
{
    Task<List<DepartmentResponseDto>> GetAllAsync();

    Task<DepartmentResponseDto?> GetByIdAsync(int id);

    Task<int> CreateAsync(DepartmentCreateDto dto);

    Task<bool> UpdateAsync(int id, DepartmentUpdateDto dto);

    Task<bool> DeleteAsync(int id);
}