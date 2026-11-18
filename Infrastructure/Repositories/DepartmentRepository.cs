
using Application.Interfaces;
using Application.DTOs.Departments;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly AppDbContext _context;

    public DepartmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DepartmentResponseDto>> GetAllAsync()
    {
        return await _context.Departments
            .OrderBy(d => d.Id)
            .Select(d => new DepartmentResponseDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                CreatedAt = d.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<DepartmentResponseDto?> GetByIdAsync(int id)
    {
        return await _context.Departments
            .Where(d => d.Id == id)
            .Select(d => new DepartmentResponseDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                CreatedAt = d.CreatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> NameExistsAsync(string name)
    {
        return await _context.Departments
            .AnyAsync(d => d.Name == name);
    }

    public async Task<int> CreateAsync(DepartmentCreateDto dto)
    {
        var department = new Department
        {
            Name = dto.Name,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Departments.AddAsync(department);
        await _context.SaveChangesAsync();

        return department.Id;
    }

    public async Task<bool> UpdateAsync(int id, DepartmentUpdateDto dto)
    {
        var department = await _context.Departments
            .FirstOrDefaultAsync(d => d.Id == id);

        if (department == null)
        {
            return false;
        }

        department.Name = dto.Name;
        department.Description = dto.Description;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> HasActiveEmployeesAsync(int departmentId)
    {
        return await _context.Employees
            .AnyAsync(e => e.DepartmentId == departmentId && e.IsActive);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var department = await _context.Departments
            .FirstOrDefaultAsync(d => d.Id == id);

        if (department == null)
        {
            return false;
        }

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();

        return true;
    }
}
