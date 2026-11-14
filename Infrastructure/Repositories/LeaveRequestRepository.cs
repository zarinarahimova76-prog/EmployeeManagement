
using Application.Interfaces;
using Application.DTOs.LeaveRequests;
using Infrastructure.Data;
using Dapper;

namespace Infrastructure.Repositories;

public class LeaveRequestRepository : ILeaveRequestRepository
{
    private readonly Database _database;

    public LeaveRequestRepository(Database database)
    {
        _database = database;
    }

    public async Task<int> CreateAsync(LeaveRequestCreateDto dto)
    {
        using var connection = _database.Connection();

        string sql = @"
            insert into leave_requests (
                employee_id,
                leave_type,
                start_date,
                end_date,
                reason
            )
            values (
                @EmployeeId,
                @LeaveType,
                @StartDate,
                @EndDate,
                @Reason
            )
            returning id;
        ";

        return await connection.QuerySingleAsync<int>(sql, dto);
    }

    public async Task<List<LeaveRequestResponseDto>> GetAllAsync(int? status)
    {
        using var connection = _database.Connection();

        string sql = @"
            select *
            from leave_requests
            where (@Status is null or status = @Status)
            order by id;
        ";

        var requests = await connection.QueryAsync<LeaveRequestResponseDto>(
            sql,
            new { Status = status }
        );

        return requests.ToList();
    }

    public async Task<bool> ApproveAsync(int id)
    {
        using var connection = _database.Connection();

        string sql = @"
            update leave_requests
            set status = 2
            where id = @Id and status = 1;
        ";

        int rows = await connection.ExecuteAsync(sql, new { Id = id });

        return rows > 0;
    }

    public async Task<bool> RejectAsync(int id)
    {
        using var connection = _database.Connection();

        string sql = @"
            update leave_requests
            set status = 3
            where id = @Id and status = 1;
        ";

        int rows = await connection.ExecuteAsync(sql, new { Id = id });

        return rows > 0;
    }
}
