
using Application.Interfaces;
using Application.DTOs.Positions;
using Infrastructure.Data;
using Dapper;

namespace Infrastructure.Repositories;

public class PositionRepository : IPositionRepository
{
    private readonly Database _database;

    public PositionRepository(Database database)
    {
        _database = database;
    }

    public async Task<List<PositionResponseDto>> GetAllAsync()
    {
        using var connection = _database.Connection();

        string sql = @"
            select
                id,
                title,
                min_salary as ""MinSalary"",
                max_salary as ""MaxSalary""
            from positions
            order by id;
        ";

        var positions = await connection.QueryAsync<PositionResponseDto>(sql);

        return positions.ToList();
    }

    public async Task<PositionResponseDto?> GetByIdAsync(int id)
    {
        using var connection = _database.Connection();

        string sql = @"
            select
                id,
                title,
                min_salary as ""MinSalary"",
                max_salary as ""MaxSalary""
            from positions
            where id = @Id;
        ";

        return await connection.QueryFirstOrDefaultAsync<PositionResponseDto>(
            sql,
            new { Id = id }
        );
    }

    public async Task<int> CreateAsync(PositionCreateDto dto)
    {
        using var connection = _database.Connection();

        string sql = @"
            insert into positions (title, min_salary, max_salary)
            values (@Title, @MinSalary, @MaxSalary)
            returning id;
        ";

        return await connection.QuerySingleAsync<int>(sql, dto);
    }
}
