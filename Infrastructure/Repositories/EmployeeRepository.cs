using Domain.Entities;
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
        where
            (@DepartmentId is null or e.department_id = @DepartmentId)
            and (@PositionId is null or e.position_id = @PositionId)
            and (
                @Search is null
                or e.first_name ilike '%' || @Search || '%'
                or e.last_name ilike '%' || @Search || '%'
                or e.email ilike '%' || @Search || '%'
            )
        order by e.id
        limit @PageSize
        offset @Offset;
    ";

    var employees = await connection.QueryAsync<EmployeeResponseDto>(sql, new
    {
        DepartmentId = departmentId,
        PositionId = positionId,
        Search = search,
        PageSize = pageSize,
        Offset = (page - 1) * pageSize
    });


     string countSql = @"
    select count(*)
    from employees e
    where
        (@DepartmentId is null or e.department_id = @DepartmentId)
        and (@PositionId is null or e.position_id = @PositionId)
        and (
            @Search is null
            or e.first_name ilike '%' || @Search || '%'
            or e.last_name ilike '%' || @Search || '%'
            or e.email ilike '%' || @Search || '%'
        );
";

int totalCount = await connection.QuerySingleAsync<int>(countSql, new
{
    DepartmentId = departmentId,
    PositionId = positionId,
    Search = search
});

    return new PagedResult<EmployeeResponseDto>
{
    Items = employees.ToList(),
    Page = page,
    PageSize = pageSize,
    TotalCount = totalCount,
    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
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
            d.id as ""Id"",
            d.name as ""Name"",
            d.description as ""Description"",
            d.created_at as ""CreatedAt"",
            p.id as ""Id"",
            p.title as ""Title"",
            p.min_salary as ""MinSalary"",
            p.max_salary as ""MaxSalary""
        from employees e
        join departments d on e.department_id = d.id
        join positions p on e.position_id = p.id
        where e.id = @Id;
    ";

    var result = await connection.QueryAsync<Employee, Department, Position, Employee>(
        sql,
        (employee, department, position) =>
        {
            employee.Department = department;
            employee.Position = position;

            return employee;
        },
        new { Id = id },
        splitOn: "Id,Id"
    );

    var employeeResult = result.FirstOrDefault();

    if (employeeResult == null)
    {
        return null;
    }

    return new EmployeeResponseDto
    {
        Id = employeeResult.Id,
        FirstName = employeeResult.FirstName,
        LastName = employeeResult.LastName,
        Email = employeeResult.Email,
        Phone = employeeResult.Phone,
        HireDate = employeeResult.HireDate,
        Salary = employeeResult.Salary,
        IsActive = employeeResult.IsActive,
        DepartmentId = employeeResult.DepartmentId,
        DepartmentName = employeeResult.Department?.Name,
        PositionId = employeeResult.PositionId,
        PositionTitle = employeeResult.Position?.Title
    };
}


    public async Task<bool> EmailExistsAsync(string email)
{
    using var connection = _database.Connection();

    string sql = @"
        select exists(
            select 1
            from employees
            where email = @Email
        );
    ";

    return await connection.QuerySingleAsync<bool>(
        sql,
        new { Email = email }
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
