namespace Application.User.DTOs;

public record PointsDto
{
    public int UserId { get; set; }
    public int PointsEarned { get; set; }
    public int TotalPoints { get; set; }
}