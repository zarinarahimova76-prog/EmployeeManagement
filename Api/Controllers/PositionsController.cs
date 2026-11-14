
using Application.DTOs.Positions;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PositionsController : ControllerBase
{
    private readonly IPositionRepository _repository;

    public PositionsController(IPositionRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var positions = await _repository.GetAllAsync();

        return Ok(positions);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var position = await _repository.GetByIdAsync(id);

        if (position == null)
        {
            return NotFound();
        }

        return Ok(position);
    }

    [HttpPost]
    public async Task<IActionResult> Create(PositionCreateDto dto)
    {
        int id = await _repository.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = id },
            new { id = id }
        );
    }
}
