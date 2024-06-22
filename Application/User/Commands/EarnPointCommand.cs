using Application.User.DTOs;
using Serilog;

namespace Application.User.Commands;

public record EarnPointCommand(int UserId, int Points) : IRequest<Result<PointsDto>>;


public class EarnPointCommandHandler(LoyaltyContext context) : IRequestHandler<EarnPointCommand, Result<PointsDto>>
{
    public async Task<Result<PointsDto>> Handle(EarnPointCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users.FindAsync(new object[] { request.UserId }, cancellationToken);

        if (user == null)
        {
            return Result<PointsDto>.Failure("User with this Id cannot be found");
        }

        user.EarnPoints(request.Points);
        await context.SaveChangesAsync(cancellationToken);

        var pointDto = new PointsDto()
        {
            PointsEarned = request.Points,
            UserId = request.UserId,
            TotalPoints = user.Points
        };

        var result = Result<PointsDto>.Success(pointDto);

        if (result.IsSuccess)
        {
            Log.Information($"The user with id {request.UserId} earn {request.Points} points successfully");
        }
        else
        {
            Log.Error($"The user with id {request.UserId} failed earn because of {result.Error}");
        }

        return result;
    }
}