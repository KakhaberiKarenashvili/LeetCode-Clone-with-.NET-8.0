namespace MainApp.Application.Features.Users.Queries.GetUsers;

public record GetUserResponse(
    string? Id,
    string? UserName,
    string? Email,
    string? Role);
