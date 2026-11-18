
using Application.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString
    )
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString)
        );

        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IPositionRepository, PositionRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();

        return services;
    }
}
