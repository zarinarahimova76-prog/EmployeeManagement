
using Application.DTOs.Departments;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentRepository _repository;

    public DepartmentsController(IDepartmentRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var departments = await _repository.GetAllAsync();

        return Ok(departments);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var department = await _repository.GetByIdAsync(id);

        if (department == null)
        {
            return NotFound();
        }

        return Ok(department);
    }

    [HttpPost]
public async Task<IActionResult> Create(DepartmentCreateDto dto)
{
    bool nameExists = await _repository.NameExistsAsync(dto.Name);

    if (nameExists)
    {
        return Conflict("Department with this name already exists.");
    }

    int id = await _repository.CreateAsync(dto);

    return CreatedAtAction(
        nameof(GetById),
        new { id = id },
        new { id = id }
    );
}

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, DepartmentUpdateDto dto)
    {
        bool updated = await _repository.UpdateAsync(id, dto);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
public async Task<IActionResult> Delete(int id)
{
    var department = await _repository.GetByIdAsync(id);

    if (department == null)
    {
        return NotFound("Department not found.");
    }

    bool hasActiveEmployees = await _repository.HasActiveEmployeesAsync(id);

    if (hasActiveEmployees)
    {
        return Conflict("Cannot delete department with active employees.");
    }

    await _repository.DeleteAsync(id);

    return NoContent();
}
}
