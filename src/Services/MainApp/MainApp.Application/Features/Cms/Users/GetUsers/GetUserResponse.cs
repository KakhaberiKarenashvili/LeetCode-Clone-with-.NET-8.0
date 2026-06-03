namespace MainApp.Application.Features.Cms.Users.GetUsers;

public record GetUserResponse(
    string? Id,
    string? UserName,
    string? Email,
    string? Role);
