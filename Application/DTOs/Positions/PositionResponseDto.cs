
namespace Application.DTOs.Positions;

public class PositionResponseDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public decimal MinSalary { get; set; }

    public decimal MaxSalary { get; set; }
}
