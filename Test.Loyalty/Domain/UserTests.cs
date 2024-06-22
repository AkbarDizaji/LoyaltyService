using Domain.Enums;
using Domain;

namespace Test.Loyalty.Domain;
public class UserTests
{
    [Fact]
    public void Constructor_ShouldInitializeProperties()
    {
        // Arrange
        string username = "tester";
        string email = "test@example.com";

        // Act
        var user = new User(username, email);

        // Assert
        Assert.Equal(username, user.Username);
        Assert.Equal(email, user.Email);
        Assert.Equal(0, user.Points);
        Assert.True(user.IsActive);
        Assert.True((DateTime.UtcNow - user.CreatedAt).TotalSeconds < 1);
        Assert.Null(user.LastLoginAt);
        Assert.Empty(user.Transactions);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(100)]
    public void EarnPoints_ShouldIncreasePointsAndAddTransaction(int pointsToEarn)
    {
        // Arrange
        var user = new User("tester", "test@example.com");

        // Act
        user.EarnPoints(pointsToEarn);

        // Assert
        Assert.Equal(pointsToEarn, user.Points);
        Assert.Single(user.Transactions);
        Assert.Equal(pointsToEarn, user.Transactions[0].Points);
        Assert.Equal(TransactionType.Earn, user.Transactions[0].Type);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void EarnPoints_ShouldThrowException_WhenPointsAreNotPositive(int invalidPoints)
    {
        // Arrange
        var user = new User("tester", "test@example.com");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => user.EarnPoints(invalidPoints));
        Assert.Equal("Points must be positive. (Parameter 'points')", exception.Message);
    }

    [Fact]
    public void SpendPoints_ShouldDecreasePointsAndAddTransaction()
    {
        // Arrange
        var user = new User("tester", "test@example.com");
        user.EarnPoints(100);

        // Act
        user.SpendPoints(50);

        // Assert
        Assert.Equal(50, user.Points);
        Assert.Equal(2, user.Transactions.Count);
        Assert.Equal(50, user.Transactions[1].Points);
        Assert.Equal(TransactionType.Spend, user.Transactions[1].Type);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void SpendPoints_ShouldThrowException_WhenPointsAreNotPositive(int invalidPoints)
    {
        // Arrange
        var user = new User("tester", "test@example.com");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => user.SpendPoints(invalidPoints));
        Assert.Equal("Points must be positive. (Parameter 'points')", exception.Message);
    }

    [Fact]
    public void SpendPoints_ShouldThrowException_WhenInsufficientPoints()
    {
        // Arrange
        var user = new User("tester", "test@example.com");
        user.EarnPoints(50);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => user.SpendPoints(100));
        Assert.Equal("Insufficient points.", exception.Message);
    }

    [Fact]
    public void UpdateLoginTime_ShouldUpdateLastLoginAt()
    {
        // Arrange
        var user = new User("tester", "test@example.com");

        // Act
        user.UpdateLoginTime();

        // Assert
        Assert.NotNull(user.LastLoginAt);
        Assert.True((DateTime.UtcNow - user.LastLoginAt.Value).TotalSeconds < 1);
    }
}