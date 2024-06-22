using Application.User.DTOs;

namespace Application.Commons;

public class MappingConfigs
{
    public static void Configure()
    {
        TypeAdapterConfig.GlobalSettings.Default.IgnoreNullValues(true);

        TypeAdapterConfig<Domain.User, UserDto>
            .NewConfig().TwoWays();
    }
}
