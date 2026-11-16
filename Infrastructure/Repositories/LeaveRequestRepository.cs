
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
    select
        lr.id as ""Id"",
        lr.employee_id as ""EmployeeId"",
        e.first_name || ' ' || e.last_name as ""EmployeeName"",
        lr.leave_type as ""LeaveType"",
        lr.start_date as ""StartDate"",
        lr.end_date as ""EndDate"",
        lr.status as ""Status"",
        lr.reason as ""Reason"",
        lr.created_at as ""CreatedAt""
    from leave_requests lr
    join employees e on e.id = lr.employee_id
    where (@Status is null or lr.status = @Status)
    order by lr.id;
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
    public async Task<bool> IsEmployeeActiveAsync(int employeeId)
{
    using var connection = _database.Connection();

    string sql = @"
        select exists (
            select 1
            from employees
            where id = @EmployeeId
            and is_active = true
        );
    ";

    return await connection.QuerySingleAsync<bool>(
        sql,
        new { EmployeeId = employeeId }
    );
}

  public async Task<bool> HasOverlappingLeaveAsync(
    int employeeId,
    DateTime startDate,
    DateTime endDate
)
{
    using var connection = _database.Connection();

    string sql = @"
        select exists (
            select 1
            from leave_requests
            where employee_id = @EmployeeId
            and status in (1, 2)
            and start_date <= @EndDate
            and end_date >= @StartDate
        );
    ";

    return await connection.QuerySingleAsync<bool>(sql, new
    {
        EmployeeId = employeeId,
        StartDate = startDate,
        EndDate = endDate
    });
}


}
