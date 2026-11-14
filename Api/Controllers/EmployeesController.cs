using Application.DTOs.Employees;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeRepository _repository;

    public EmployeesController(IEmployeeRepository repository)
    {
        _repository = repository;
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
    bool updated = await _repository.UpdateAsync(id, dto);

    if (!updated)
    {
        return NotFound();
    }

    return NoContent();
}


  [HttpPost("{id}/terminate")]
public async Task<IActionResult> Terminate(int id)
{
    bool terminated = await _repository.TerminateAsync(id);

    if (!terminated)
    {
        return NotFound();
    }

    return NoContent();
}

  [HttpGet("{id}/leave-requests")]
public async Task<IActionResult> GetLeaveRequests(int id)
{
    var requests = await _repository.GetLeaveRequestsAsync(id);

    return Ok(requests);
}

}
