using Application.DTOs.LeaveRequests;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class LeaveRequestRepository : ILeaveRequestRepository
{
    private readonly AppDbContext _context;

    public LeaveRequestRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateAsync(LeaveRequestCreateDto dto)
    {
        var leaveRequest = new LeaveRequest
        {
            EmployeeId = dto.EmployeeId,
            LeaveType = dto.LeaveType,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Reason = dto.Reason,
            Status = LeaveStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _context.LeaveRequests.AddAsync(leaveRequest);
        await _context.SaveChangesAsync();

        return leaveRequest.Id;
    }

    public async Task<List<LeaveRequestResponseDto>> GetAllAsync(int? status)
    {
        var query = _context.LeaveRequests
            .AsNoTracking()
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(lr => (int)lr.Status == status.Value);
        }

        return await query
            .OrderBy(lr => lr.Id)
            .Select(lr => new LeaveRequestResponseDto
            {
                Id = lr.Id,
                EmployeeId = lr.EmployeeId,
                EmployeeName = lr.Employee!.FirstName + " " + lr.Employee.LastName,
                LeaveType = lr.LeaveType,
                StartDate = lr.StartDate,
                EndDate = lr.EndDate,
                Status = lr.Status,
                Reason = lr.Reason,
                CreatedAt = lr.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<bool> ApproveAsync(int id)
    {
        var leaveRequest = await _context.LeaveRequests
            .FirstOrDefaultAsync(lr => lr.Id == id && lr.Status == LeaveStatus.Pending);

        if (leaveRequest == null)
        {
            return false;
        }

        leaveRequest.Status = LeaveStatus.Approved;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RejectAsync(int id)
    {
        var leaveRequest = await _context.LeaveRequests
            .FirstOrDefaultAsync(lr => lr.Id == id && lr.Status == LeaveStatus.Pending);

        if (leaveRequest == null)
        {
            return false;
        }

        leaveRequest.Status = LeaveStatus.Rejected;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> IsEmployeeActiveAsync(int employeeId)
    {
        return await _context.Employees
            .AnyAsync(e => e.Id == employeeId && e.IsActive);
    }

    public async Task<bool> HasOverlappingLeaveAsync(
        int employeeId,
        DateTime startDate,
        DateTime endDate
    )
    {
        return await _context.LeaveRequests
            .AnyAsync(lr =>
                lr.EmployeeId == employeeId &&
                (lr.Status == LeaveStatus.Pending || lr.Status == LeaveStatus.Approved) &&
                lr.StartDate <= endDate &&
                lr.EndDate >= startDate
            );
    }
}
