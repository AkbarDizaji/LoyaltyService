using Application.User.Commands;
using Application.User.DTOs;
using Domain;
using Infrastructure;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Test.Loyalty.ApplicationTests;

public class CreateUserCommandHandlerTests
{
    private readonly Mock<IMapper> _mockMapper = new();
    private readonly DbContextOptions<LoyaltyContext> _options = new DbContextOptionsBuilder<LoyaltyContext>()
        .UseInMemoryDatabase(databaseName: "TestLoyaltyDb")
        .Options;

    [Fact]
    public async Task Handle_ShouldCreateUser_WhenValidCommandIsProvided()
    {
        // Arrange
        var command = new CreateUserCommand("tester", "test@example.com");
        var expectedUserDto = new UserDto(1,"tester", "test@example.com", 5);

        await using var context = new LoyaltyContext(_options);

        _mockMapper.Setup(m => m.Map<UserDto>(It.IsAny<User>()))
            .Returns(expectedUserDto);

        var handler = new CreateUserCommandHandler(context, _mockMapper.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedUserDto, result.Value);

        var savedUser = await context.Users.FirstOrDefaultAsync();
        Assert.NotNull(savedUser);
        Assert.Equal(command.Username, savedUser.Username);
        Assert.Equal(command.Email, savedUser.Email);
    }

    [Fact]
    public async Task Handle_ShouldCallMapper_WithCorrectUser()
    {
        // Arrange
        var command = new CreateUserCommand("tester", "test@example.com");
        var expectedUserDto = new UserDto(1, "tester", "test@example.com", 5);

        await using var context = new LoyaltyContext(_options);
        _mockMapper.Setup(m => m.Map<UserDto>(It.IsAny<User>()))
            .Returns(expectedUserDto)
            .Verifiable();

        var handler = new CreateUserCommandHandler(context, _mockMapper.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _mockMapper.Verify(m => m.Map<UserDto>(It.Is<User>(u =>
                u.Username == command.Username &&
                u.Email == command.Email)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldSaveChanges_ToDatabase()
    {
        // Arrange
        var command = new CreateUserCommand("tester", "test@example.com");
        var expectedUserDto = new UserDto(1, "tester", "test@example.com", 5);

        await using var context = new LoyaltyContext(_options);
        _mockMapper.Setup(m => m.Map<UserDto>(It.IsAny<User>()))
            .Returns(expectedUserDto);

        var handler = new CreateUserCommandHandler(context, _mockMapper.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var savedUser = await context.Users.FirstOrDefaultAsync();
        Assert.NotNull(savedUser);
        Assert.Equal(command.Username, savedUser.Username);
        Assert.Equal(command.Email, savedUser.Email);
    }
}