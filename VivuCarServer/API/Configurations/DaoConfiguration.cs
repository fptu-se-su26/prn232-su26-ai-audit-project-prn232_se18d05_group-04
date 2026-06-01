using DataAccess.Configurations;

namespace API.Configurations;

public static class DaoConfiguration
{
    public static IServiceCollection AddVivuCarDataAccess(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddVivuCarDbContext(configuration);
    }
}
