
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Entities;

[Table("LeaveRequests")]
public class LeaveRequest
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [Required]
    public LeaveType LeaveType { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;

    [MaxLength(500)]
    public string? Reason { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    public Employee? Employee { get; set; }
}
