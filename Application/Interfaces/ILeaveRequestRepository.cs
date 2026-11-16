using Application.DTOs.LeaveRequests;

namespace Application.Interfaces;

public interface ILeaveRequestRepository
{
    Task<int> CreateAsync(LeaveRequestCreateDto dto);

    Task<List<LeaveRequestResponseDto>> GetAllAsync(int? status);

    Task<bool> ApproveAsync(int id);

    Task<bool> RejectAsync(int id);
    Task<bool> IsEmployeeActiveAsync(int employeeId);
    Task<bool> HasOverlappingLeaveAsync(
    int employeeId,
    DateTime startDate,
    DateTime endDate
);
}