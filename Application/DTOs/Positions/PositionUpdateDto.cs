
namespace Application.DTOs.Positions;

public class PositionUpdateDto
{
    public string Title { get; set; } = string.Empty;

    public decimal MinSalary { get; set; }

    public decimal MaxSalary { get; set; }
}
