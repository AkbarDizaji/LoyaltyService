using Application.User.DTOs;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Application.User.Commands;

public record CreateUserCommand(string Username, string Email) : IRequest<Result<UserDto>>;


public class CreateUserCommandHandler(LoyaltyContext context, IMapper mapper)
    : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {


        var user = new Domain.User(request.Username, request.Email);
        await context.Users.AddAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);


        var result = Result<UserDto>.Success(mapper.Map<UserDto>(user));

        if (result.IsSuccess)
        {
            Log.Information($"The create user with email {request.Email} done successfully");
        }
        else
        {
            Log.Error($"The create user with email {request.Email} failed because of {result.Error}");
        }

        return result;
    }
}