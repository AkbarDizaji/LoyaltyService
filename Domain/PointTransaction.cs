using Domain.Enums;

namespace Domain;

public class PointTransaction
{
    private PointTransaction() { } // For EF Core

    public PointTransaction(User user, int points, TransactionType type)
    {
        UserId = user.Id;
        Points = points;
        Type = type;
        Timestamp = DateTime.UtcNow;
    }

    public int Id { get; private set; }
    public int UserId { get; private set; }
    public User User { get; private set; }
    public int Points { get; private set; }
    public TransactionType Type { get; private set; }
    public DateTime Timestamp { get; private set; }

}