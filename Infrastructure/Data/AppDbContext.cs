
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Department> Departments { get; set; }

    public DbSet<Position> Positions { get; set; }

    public DbSet<Employee> Employees { get; set; }

    public DbSet<LeaveRequest> LeaveRequests { get; set; }
}
