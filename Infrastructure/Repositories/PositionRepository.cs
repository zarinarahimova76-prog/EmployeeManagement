
using Application.Interfaces;
using Application.DTOs.Positions;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PositionRepository : IPositionRepository
{
    private readonly AppDbContext _context;

    public PositionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<PositionResponseDto>> GetAllAsync()
    {
        return await _context.Positions
            .OrderBy(p => p.Id)
            .Select(p => new PositionResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                MinSalary = p.MinSalary,
                MaxSalary = p.MaxSalary
            })
            .ToListAsync();
    }

    public async Task<PositionResponseDto?> GetByIdAsync(int id)
    {
        return await _context.Positions
            .Where(p => p.Id == id)
            .Select(p => new PositionResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                MinSalary = p.MinSalary,
                MaxSalary = p.MaxSalary
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> TitleExistsAsync(string title)
    {
        return await _context.Positions
            .AnyAsync(p => p.Title == title);
    }

    public async Task<int> CreateAsync(PositionCreateDto dto)
    {
        var position = new Position
        {
            Title = dto.Title,
            MinSalary = dto.MinSalary,
            MaxSalary = dto.MaxSalary
        };

        await _context.Positions.AddAsync(position);
        await _context.SaveChangesAsync();

        return position.Id;
    }
}
