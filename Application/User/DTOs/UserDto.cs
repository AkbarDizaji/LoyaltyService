namespace Application.User.DTOs;

public record UserDto(int Id, string Username, string Email, int Points)
{
    public int Id { get; private set; } = Id;
    public string Username { get; private set; } = Username;
    public string Email { get; private set; } = Email;
    public int Points { get; private set; } = Points;
}