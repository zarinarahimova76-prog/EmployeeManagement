using Application.DTOs.Employees;
using Application.DTOs.LeaveRequests;
using Application.Interfaces;
using Application.Responses;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _context;

    public EmployeeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<EmployeeResponseDto>> GetAllAsync(
        int? departmentId,
        int? positionId,
        string? search,
        int page,
        int pageSize
    )
    {
        var query = _context.Employees
            .AsNoTracking()
            .AsQueryable();

        if (departmentId.HasValue)
        {
            query = query.Where(e => e.DepartmentId == departmentId.Value);
        }

        if (positionId.HasValue)
        {
            query = query.Where(e => e.PositionId == positionId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            string searchTerm = $"%{search.Trim()}%";

            query = query.Where(e =>
                EF.Functions.ILike(e.FirstName, searchTerm) ||
                EF.Functions.ILike(e.LastName, searchTerm) ||
                EF.Functions.ILike(e.Email, searchTerm)
            );
        }

        int totalCount = await query.CountAsync();

        var employees = await query
            .OrderBy(e => e.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new EmployeeResponseDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                Phone = e.Phone,
                HireDate = e.HireDate,
                Salary = e.Salary,
                IsActive = e.IsActive,
                DepartmentId = e.DepartmentId,
                DepartmentName = e.Department!.Name,
                PositionId = e.PositionId,
                PositionTitle = e.Position!.Title
            })
            .ToListAsync();

        return new PagedResult<EmployeeResponseDto>
        {
            Items = employees,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        };
    }

    public async Task<EmployeeResponseDto?> GetByIdAsync(int id)
    {
        return await _context.Employees
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(e => new EmployeeResponseDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                Phone = e.Phone,
                HireDate = e.HireDate,
                Salary = e.Salary,
                IsActive = e.IsActive,
                DepartmentId = e.DepartmentId,
                DepartmentName = e.Department!.Name,
                PositionId = e.PositionId,
                PositionTitle = e.Position!.Title
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Employees
            .AnyAsync(e => e.Email == email);
    }

    public async Task<int> CreateAsync(EmployeeCreateDto dto)
    {
        var employee = new Domain.Entities.Employee
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            HireDate = dto.HireDate,
            Salary = dto.Salary,
            DepartmentId = dto.DepartmentId,
            PositionId = dto.PositionId,
            IsActive = true
        };

        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();

        return employee.Id;
    }

    public async Task<bool> UpdateAsync(int id, EmployeeUpdateDto dto)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null)
        {
            return false;
        }

        employee.FirstName = dto.FirstName;
        employee.LastName = dto.LastName;
        employee.Email = dto.Email;
        employee.Phone = dto.Phone;
        employee.HireDate = dto.HireDate;
        employee.Salary = dto.Salary;
        employee.DepartmentId = dto.DepartmentId;
        employee.PositionId = dto.PositionId;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> TerminateAsync(int id)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == id && e.IsActive);

        if (employee == null)
        {
            return false;
        }

        employee.IsActive = false;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<LeaveRequestResponseDto>> GetLeaveRequestsAsync(int employeeId)
    {
        return await _context.LeaveRequests
            .AsNoTracking()
            .Where(lr => lr.EmployeeId == employeeId)
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
}
