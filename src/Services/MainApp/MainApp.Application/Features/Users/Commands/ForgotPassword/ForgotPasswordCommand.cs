using MediatR;

namespace MainApp.Application.Features.Users.Commands.ForgotPassword;

public record ForgotPasswordCommand(string Email) : IRequest<Unit>;
