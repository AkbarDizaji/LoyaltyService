using Application.User.Commands;
using Application.User.Queries;
using Microsoft.AspNetCore.Authorization;
using Serilog;

namespace LoyaltyService.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]

public class UsersController(IMediator mediator, IConfiguration configuration) : ControllerBase
{
    [HttpPost("{id}/earn")]
    public async Task<IActionResult> EarnPoints(int id, [FromBody] int points)
    {
       Log.Information("Attempting to earn {Points} points for user {UserId}", points, id);

        var command = new EarnPointCommand(id, points);
        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
           Log.Warning("Failed to earn points for user {UserId}. Error: {Error}", id, result.Error);
            return BadRequest(result.Error);
        }

       Log.Information("Successfully earned {Points} points for user {UserId}", points, id);
        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
    {
       Log.Information("Attempting to create new user: {@Command}", command);
        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
           Log.Warning("Failed to create user. Error: {Error}", result.Error);
            return BadRequest(result.Error);
        }

       Log.Information("Successfully created user with ID: {UserId}", result.Value);
        return Ok(result.Value);
    }


    [HttpGet("{id}/points")]
    public async Task<IActionResult> GetUserPoints(int id)
    {
       Log.Information("Retrieving points for user {UserId}", id);

        var query = new GetUserPointsQuery(id);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
           Log.Warning("Failed to retrieve points for user {UserId}. Error: {Error}", id, result.Error);
            return NotFound(result.Error);
        }

       Log.Information("Successfully retrieved {Points} points for user {UserId}", result.Value, id);
        return Ok(result.Value);
    }

    [AllowAnonymous]
    [HttpGet("get-token")]
    public async Task<string> GetToken()
    {
       Log.Information("Attempting to get token");
        var client = new HttpClient();

        var baseUrl = configuration["Keycloak:auth-server-url"];
        var realm = configuration["Keycloak:Realm"];
        var tokenEndpoint = configuration["Keycloak:TokenEndpoint"];
        var keycloakAddress = $"{baseUrl}/{realm}{tokenEndpoint}";
        Log.Information(keycloakAddress);
        var request = new HttpRequestMessage(HttpMethod.Post, keycloakAddress);
        var collection = new List<KeyValuePair<string, string>>
        {
            new("grant_type", "client_credentials"),
            new("client_id", "Loyal"),
            new("client_secret", "1k0GC0D57Tpb1c5duCHLAppgGQ7WHmVg")
        };
        var content = new FormUrlEncodedContent(collection);
        request.Content = content;

        Log.Debug("Sending token request to Keycloak");

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}