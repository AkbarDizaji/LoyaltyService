using Domain.Enums;

namespace Domain;

public class User
{
    private User() { } // For EF Core

    public User(string username, string email)
    {
        Username = username;
        Email = email;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public int Id { get; private set; }
    public string Username { get; private set; }
    public string Email { get; private set; }
    public int Points { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public bool IsActive { get; private set; }
    public List<PointTransaction> Transactions { get; private set; } = new List<PointTransaction>();

    public void EarnPoints(int points)
    {
        if (points <= 0)
            throw new ArgumentException("Points must be positive.", nameof(points));

        Points += points;
        Transactions.Add(new PointTransaction(this, points, TransactionType.Earn));
    }

    public void SpendPoints(int points)
    {
        if (points <= 0)
            throw new ArgumentException("Points must be positive.", nameof(points));
        if (Points < points)
            throw new InvalidOperationException("Insufficient points.");

        Points -= points;
        Transactions.Add(new PointTransaction(this, points, TransactionType.Spend));
    }

    public void UpdateLoginTime()
    {
        LastLoginAt = DateTime.UtcNow;
    }
}