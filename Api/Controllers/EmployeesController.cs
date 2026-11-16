using Application.DTOs.Employees;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeRepository _repository;
    private readonly IPositionRepository _positionRepository;
    private readonly IDepartmentRepository _departmentRepository;

    public EmployeesController(
    IEmployeeRepository repository,
    IPositionRepository positionRepository,
    IDepartmentRepository departmentRepository
)
{
    _repository = repository;
    _positionRepository = positionRepository;
    _departmentRepository = departmentRepository;
}

    [HttpGet]
    public async Task<IActionResult> GetAll(
        int? departmentId,
        int? positionId,
        string? search,
        int page = 1,
        int pageSize = 20
    )
    {
        if (page < 1)
        {
            return BadRequest("Page must be greater than 0.");
        }

        if (pageSize < 1 || pageSize > 100)
        {
            return BadRequest("PageSize must be between 1 and 100.");
        }

        var employees = await _repository.GetAllAsync(
            departmentId,
            positionId,
            search,
            page,
            pageSize
        );

        return Ok(employees);
    }
    

[HttpGet("{id}")]
public async Task<IActionResult> GetById(int id)
{
    var employee = await _repository.GetByIdAsync(id);

    if (employee == null)
    {
        return NotFound();
    }

    return Ok(employee);
}
   

[HttpPost]
public async Task<IActionResult> Create(EmployeeCreateDto dto)
{

    if (dto.HireDate.Date > DateTime.Today)
{
    return BadRequest("Hire date cannot be in the future.");
}
var department = await _departmentRepository.GetByIdAsync(dto.DepartmentId);

if (department == null)
{
    return NotFound("Department not found.");
}

if (department == null)
{
    return NotFound("Department not found.");
}
    var position = await _positionRepository.GetByIdAsync(dto.PositionId);

    if (position == null)
    {
        return NotFound("Position not found.");
    }

    if (dto.Salary < position.MinSalary || dto.Salary > position.MaxSalary)
    {
        return BadRequest("Salary must be within the position salary range.");
    }


    bool emailExists = await _repository.EmailExistsAsync(dto.Email);

if (emailExists)
{
    return Conflict("Employee with this email already exists.");
}

    int id = await _repository.CreateAsync(dto);

    return CreatedAtAction(
        nameof(GetById),
        new { id = id },
        new { id = id }
    );
}


[HttpPut("{id}")]
public async Task<IActionResult> Update(int id, EmployeeUpdateDto dto)
{
    if (dto.HireDate.Date > DateTime.Today)
    {
        return BadRequest("Hire date cannot be in the future.");
    }

    var existingEmployee = await _repository.GetByIdAsync(id);

    if (existingEmployee == null)
    {
        return NotFound("Employee not found.");
    }

    if (existingEmployee.Email != dto.Email)
    {
        bool emailExists = await _repository.EmailExistsAsync(dto.Email);

        if (emailExists)
        {
            return Conflict("Employee with this email already exists.");
        }
    }

    var department = await _departmentRepository.GetByIdAsync(dto.DepartmentId);

    if (department == null)
    {
        return NotFound("Department not found.");
    }

    var position = await _positionRepository.GetByIdAsync(dto.PositionId);

    if (position == null)
    {
        return NotFound("Position not found.");
    }

    if (dto.Salary < position.MinSalary || dto.Salary > position.MaxSalary)
    {
        return BadRequest("Salary must be within the position salary range.");
    }

    await _repository.UpdateAsync(id, dto);

    return NoContent();
}


  [HttpPost("{id}/terminate")]
public async Task<IActionResult> Terminate(int id)
{
    var employee = await _repository.GetByIdAsync(id);

    if (employee == null)
    {
        return NotFound("Employee not found.");
    }

    if (!employee.IsActive)
    {
        return Conflict("Employee is already terminated.");
    }

    await _repository.TerminateAsync(id);

    return NoContent();
}

  [HttpGet("{id}/leave-requests")]
public async Task<IActionResult> GetLeaveRequests(int id)
{
    var requests = await _repository.GetLeaveRequestsAsync(id);

    return Ok(requests);
}

}
