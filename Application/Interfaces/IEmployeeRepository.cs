
using Application.DTOs.Employees;
using Application.DTOs.LeaveRequests;
using Application.Responses;

namespace Application.Interfaces;

public interface IEmployeeRepository
{
    Task<PagedResult<EmployeeResponseDto>> GetAllAsync(
        int? departmentId,
        int? positionId,
        string? search,
        int page,
        int pageSize
    );

    Task<EmployeeResponseDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(EmployeeCreateDto dto);
    Task<bool> UpdateAsync(int id, EmployeeUpdateDto dto);
    Task<bool> TerminateAsync(int id);
    Task<List<LeaveRequestResponseDto>> GetLeaveRequestsAsync(int employeeId);
    Task<bool> EmailExistsAsync(string email);
}
