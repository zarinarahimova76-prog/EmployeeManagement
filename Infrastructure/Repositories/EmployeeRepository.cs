using Application.DTOs.LeaveRequests;
using Application.Interfaces;
using Application.Responses;
using Application.DTOs.Employees;
using Infrastructure.Data;
using Dapper;

namespace Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly Database _database;

    public EmployeeRepository(Database database)
    {
        _database = database;
    }

    public async Task<PagedResult<EmployeeResponseDto>> GetAllAsync(
        int? departmentId,
        int? positionId,
        string? search,
        int page,
        int pageSize
        
    )
    {
        using var connection = _database.Connection();

        await connection.OpenAsync();

        string sql = @"
            SELECT
                e.id AS Id,
                e.first_name AS FirstName,
                e.last_name AS LastName,
                e.email AS Email,
                e.phone AS Phone,
                e.hire_date AS HireDate,
                e.salary AS Salary,
                e.is_active AS IsActive,
                e.department_id AS DepartmentId,
                d.name AS DepartmentName,
                e.position_id AS PositionId,
                p.title AS PositionTitle
            FROM employees e
            JOIN departments d ON e.department_id = d.id
            JOIN positions p ON e.position_id = p.id
            ORDER BY e.id;
        ";

        var employees = await connection.QueryAsync<EmployeeResponseDto>(sql);

        int totalCount = await connection.QuerySingleAsync<int>(
            "SELECT COUNT(*) FROM employees"
        );

        return new PagedResult<EmployeeResponseDto>
        {
            Items = employees.ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

public async Task<EmployeeResponseDto?> GetByIdAsync(int id)
{
    using var connection = _database.Connection();

    string sql = @"
        select
            e.id as ""Id"",
            e.first_name as ""FirstName"",
            e.last_name as ""LastName"",
            e.email as ""Email"",
            e.phone as ""Phone"",
            e.hire_date as ""HireDate"",
            e.salary as ""Salary"",
            e.is_active as ""IsActive"",
            e.department_id as ""DepartmentId"",
            d.name as ""DepartmentName"",
            e.position_id as ""PositionId"",
            p.title as ""PositionTitle""
        from employees e
        join departments d on e.department_id = d.id
        join positions p on e.position_id = p.id
        where e.id = @Id;
    ";

    return await connection.QueryFirstOrDefaultAsync<EmployeeResponseDto>(
        sql,
        new { Id = id }
    );
}

   
public async Task<int> CreateAsync(EmployeeCreateDto dto)
{
    using var connection = _database.Connection();

    string sql = @"
        insert into employees (
            first_name,
            last_name,
            email,
            phone,
            hire_date,
            salary,
            department_id,
            position_id
        )
        values (
            @FirstName,
            @LastName,
            @Email,
            @Phone,
            @HireDate,
            @Salary,
            @DepartmentId,
            @PositionId
        )
        returning id;
    ";

    return await connection.QuerySingleAsync<int>(sql, dto);
}
   

   public async Task<bool> UpdateAsync(int id, EmployeeUpdateDto dto)
{
    using var connection = _database.Connection();

    string sql = @"
        update employees
        set first_name = @FirstName,
            last_name = @LastName,
            email = @Email,
            phone = @Phone,
            hire_date = @HireDate,
            salary = @Salary,
            department_id = @DepartmentId,
            position_id = @PositionId
        where id = @Id;
    ";

    int rows = await connection.ExecuteAsync(sql, new
    {
        Id = id,
        dto.FirstName,
        dto.LastName,
        dto.Email,
        dto.Phone,
        dto.HireDate,
        dto.Salary,
        dto.DepartmentId,
        dto.PositionId
    });

    return rows > 0;
}
    
     public async Task<bool> TerminateAsync(int id)
{
    using var connection = _database.Connection();

    string sql = @"
        update employees
        set is_active = false
        where id = @Id and is_active = true;
    ";

    int rows = await connection.ExecuteAsync(sql, new { Id = id });

    return rows > 0;
}


    public async Task<List<LeaveRequestResponseDto>> GetLeaveRequestsAsync(int employeeId)
{
    using var connection = _database.Connection();

    string sql = @"
        select
            id,
            employee_id as ""EmployeeId"",
            leave_type as ""LeaveType"",
            start_date as ""StartDate"",
            end_date as ""EndDate"",
            status,
            reason,
            created_at as ""CreatedAt""
        from leave_requests
        where employee_id = @EmployeeId
        order by id;
    ";

    var requests = await connection.QueryAsync<LeaveRequestResponseDto>(
        sql,
        new { EmployeeId = employeeId }
    );

    return requests.ToList();
}
}
