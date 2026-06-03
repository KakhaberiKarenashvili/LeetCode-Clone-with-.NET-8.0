using System.ComponentModel.DataAnnotations;
using MediatR;

namespace MainApp.Application.Features.Cms.Users.ChangeEmail;

public record ChangeEmailCommand(
    string UserId,
    [property: Required, EmailAddress] string NewEmail) : IRequest<Unit>;
