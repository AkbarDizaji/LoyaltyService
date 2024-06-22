using Application.User.Commands;
using Application.User.Queries;

namespace Application;

[ExcludeFromCodeCoverage]

public class ApplicationDependencyRegistration
{
    public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(typeof(EarnPointCommand).Assembly);
        services.AddMediatR(typeof(GetUserPointsQuery).Assembly);

        services.AddMapster();
        MappingConfigs.Configure();
    }
}
