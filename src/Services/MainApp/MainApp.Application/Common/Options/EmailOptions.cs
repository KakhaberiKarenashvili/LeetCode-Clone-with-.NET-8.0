namespace MainApp.Application.Common.Options;

public record EmailOptions
{
    public const string Key = nameof(EmailOptions);

    public string SmtpUserEmail { get; set; } = string.Empty;
    public string SmtpPassword { get; set; } = string.Empty;
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; }
    public string FrontendBaseUrl { get; set; } = string.Empty;
    public string ResetPasswordRoute { get; set; } = string.Empty;
    public string ConfirmEmailRoute { get; set; } = string.Empty;
    public string SetPasswordRoute { get; set; } = string.Empty;
}
