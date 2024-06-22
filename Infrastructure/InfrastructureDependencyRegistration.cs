namespace Infrastructure;

[ExcludeFromCodeCoverage]
public class InfrastructureDependencyRegistration
{

    public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {

        services.AddDbContext<LoyaltyContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("LoyaltyDatabase")));

    }
}