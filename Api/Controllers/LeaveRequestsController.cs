
using Application.DTOs.LeaveRequests;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeaveRequestsController : ControllerBase
{
    private readonly ILeaveRequestRepository _repository;

    public LeaveRequestsController(ILeaveRequestRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int? status)
    {
        var requests = await _repository.GetAllAsync(status);

        return Ok(requests);
    }

    [HttpPost]
    public async Task<IActionResult> Create(LeaveRequestCreateDto dto)
    {
        int id = await _repository.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetAll),
            new { id = id },
            new { id = id }
        );
    }

    [HttpPost("{id}/approve")]
    public async Task<IActionResult> Approve(int id)
    {
        bool approved = await _repository.ApproveAsync(id);

        if (!approved)
        {
            return Conflict("Leave request not found or already processed.");
        }

        return Ok("Leave request approved.");
    }

    [HttpPost("{id}/reject")]
    public async Task<IActionResult> Reject(int id)
    {
        bool rejected = await _repository.RejectAsync(id);

        if (!rejected)
        {
            return Conflict("Leave request not found or already processed.");
        }

        return Ok("Leave request rejected.");
    }
}
