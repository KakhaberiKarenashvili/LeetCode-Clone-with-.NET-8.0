using MediatR;

namespace MainApp.Application.Features.Site.Profile.ChangePassword;

public record ChangePasswordCommand(
    string UserId,
    string CurrentPassword,
    string NewPassword) : IRequest<Unit>;
