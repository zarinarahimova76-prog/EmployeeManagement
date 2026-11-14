
using Application.Interfaces;
using Application.DTOs.Departments;
using Infrastructure.Data;
using Dapper;

namespace Infrastructure.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly Database _database;

    public DepartmentRepository(Database database)
    {
        _database = database;
    }

    public async Task<List<DepartmentResponseDto>> GetAllAsync()
    {
        using var connection = _database.Connection();

        string sql = "select * from departments order by id";

        var departments = await connection.QueryAsync<DepartmentResponseDto>(sql);

        return departments.ToList();
    }

    public async Task<DepartmentResponseDto?> GetByIdAsync(int id)
    {
        using var connection = _database.Connection();

        string sql = "select * from departments where id = @Id";

        return await connection.QueryFirstOrDefaultAsync<DepartmentResponseDto>(
            sql,
            new { Id = id }
        );
    }

    public async Task<int> CreateAsync(DepartmentCreateDto dto)
    {
        using var connection = _database.Connection();

        string sql = @"
            insert into departments (name, description)
            values (@Name, @Description)
            returning id;
        ";

        return await connection.QuerySingleAsync<int>(sql, dto);
    }

    public async Task<bool> UpdateAsync(int id, DepartmentUpdateDto dto)
    {
        using var connection = _database.Connection();

        string sql = @"
            update departments
            set name = @Name,
                description = @Description
            where id = @Id;
        ";

        int rows = await connection.ExecuteAsync(sql, new
        {
            Id = id,
            dto.Name,
            dto.Description
        });

        return rows > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = _database.Connection();

        string sql = "delete from departments where id = @Id";

        int rows = await connection.ExecuteAsync(sql, new { Id = id });

        return rows > 0;
    }
}
