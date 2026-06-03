using MediatR;

namespace MainApp.Application.Features.Site.Auth.ConfirmEmail;

public record ConfirmEmailCommand(string Email, string Token) : IRequest<Unit>;
