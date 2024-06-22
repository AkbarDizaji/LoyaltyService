using Application.User.Queries;
using Domain;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Test.Loyalty.ApplicationTests;

public class GetUserPointsQueryHandlerTests
{
    private DbContextOptions<LoyaltyContext> CreateNewContextOptions()
    {
        return new DbContextOptionsBuilder<LoyaltyContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task Handle_ShouldReturnUserPoints_WhenUserExists()
    {
        // Arrange
        var options = CreateNewContextOptions();

        await using var context = new LoyaltyContext(options);
        var newUser = new User("tester", "test@example.com");

        context.Users.Add(newUser);
        await context.SaveChangesAsync();

        var handler = new GetUserPointsQueryHandler(context);
        var query = new GetUserPointsQuery(newUser.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newUser.Points, result.Value);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserDoesNotExist()
    {
        // Arrange
        var options = CreateNewContextOptions();
        var nonExistentUserId = 999;

        await using var context = new LoyaltyContext(options);
        var handler = new GetUserPointsQueryHandler(context);
        var query = new GetUserPointsQuery(nonExistentUserId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("User with this Id cannot be found", result.Error);
    }

    [Fact]
    public async Task Handle_ShouldReturnZeroPoints_ForNewUser()
    {
        // Arrange
        var options = CreateNewContextOptions();

        await using var context = new LoyaltyContext(options);
        var newUser = new User("tester", "test@example.com");
        context.Users.Add(newUser);
        await context.SaveChangesAsync();

        var handler = new GetUserPointsQueryHandler(context);
        var query = new GetUserPointsQuery(newUser.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newUser.Points, result.Value);
    }
}