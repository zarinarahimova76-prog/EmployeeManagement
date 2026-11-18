
namespace Domain.Entities;

public class Position
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public decimal MinSalary { get; set; }

    public decimal MaxSalary { get; set; }

    // Navigation Property
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
