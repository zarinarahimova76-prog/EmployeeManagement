
using Domain.Enums;

namespace Application.DTOs.LeaveRequests;

public class LeaveRequestResponseDto
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public LeaveType LeaveType { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public LeaveStatus Status { get; set; }

    public string? Reason { get; set; }

    public DateTime CreatedAt { get; set; }
}
