
namespace Domain.Entities;

public class Employee
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public DateTime HireDate { get; set; }

    public decimal Salary { get; set; }

    public bool IsActive { get; set; } = true;

    public int DepartmentId { get; set; }

    public int PositionId { get; set; }
    public Department? Department { get; set; }

    public Position? Position { get; set; }
}
