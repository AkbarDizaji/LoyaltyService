using Application.User.Commands;
using Domain;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Test.Loyalty.ApplicationTests;

public class EarnPointCommandHandlerTests
{
    private DbContextOptions<LoyaltyContext> CreateNewContextOptions()
    {
        return new DbContextOptionsBuilder<LoyaltyContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task Handle_ShouldEarnPoints_WhenUserExists()
    {
        // Arrange
        var options = CreateNewContextOptions();
        var initialPoints = 100;
        var pointsToEarn = 50;

        await using var context = new LoyaltyContext(options);
        var newUser = new User("tester", "test@example.com");
        context.Users.Add(newUser);
        await context.SaveChangesAsync();

        var handler = new EarnPointCommandHandler(context);
        var command = new EarnPointCommand(newUser.Id, pointsToEarn);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(pointsToEarn, result.Value.PointsEarned);
        Assert.Equal(newUser.Id, result.Value.UserId);

        var user = await context.Users.FindAsync(newUser.Id);
        Assert.NotNull(user);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserDoesNotExist()
    {
        // Arrange
        var options = CreateNewContextOptions();
        var nonExistentUserId = 999;
        var pointsToEarn = 50;

        await using var context = new LoyaltyContext(options);
        var handler = new EarnPointCommandHandler(context);
        var command = new EarnPointCommand(nonExistentUserId, pointsToEarn);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        Assert.Equal("User with this Id cannot be found", result.Error);
    }

    [Fact]
    public async Task Handle_ShouldHandleInvalidPointValues()
    {
        // Arrange
        var options = CreateNewContextOptions();

        await using var context = new LoyaltyContext(options);
        var newUser = new User("tester", "test@example.com");
        context.Users.Add(newUser);
        await context.SaveChangesAsync();

        var handler = new EarnPointCommandHandler(context);
        var command = new EarnPointCommand(newUser.Id, -100);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));

        var user = await context.Users.FindAsync(newUser.Id);
        Assert.NotNull(user);
        Assert.Equal(user.Points,0); // shouldn't change value
    }
}
