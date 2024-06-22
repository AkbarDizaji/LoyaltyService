using Serilog;

namespace Application.User.Queries;

public record GetUserPointsQuery(int UserId) : IRequest<Result<int>>;

public class GetUserPointsQueryHandler(LoyaltyContext context) 
    : IRequestHandler<GetUserPointsQuery, Result<int>>
{
    public async Task<Result<int>> Handle(GetUserPointsQuery request, CancellationToken cancellationToken)
    {
        Log.Information("in queries");
        var user = await context.Users
            .FindAsync(new object[] { request.UserId }, cancellationToken);

        return user == null ? Result<int>.Failure("User with this Id cannot be found") 
            : Result<int>.Success(user.Points);
    }
}
