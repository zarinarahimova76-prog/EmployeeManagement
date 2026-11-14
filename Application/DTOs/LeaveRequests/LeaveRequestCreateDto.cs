
using Domain.Enums;

namespace Application.DTOs.LeaveRequests;

public class LeaveRequestCreateDto
{
    public int EmployeeId { get; set; }

    public LeaveType LeaveType { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string? Reason { get; set; }
}

