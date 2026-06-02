namespace MainApp.Application.Common.Services.Email;

public static class EmailTemplateProvider
{
    private const string TemplateDirectory = "EmailTemplates";

    public const string PasswordResetTemplate = "PasswordResetTemplate.html";
    public const string EmailConfirmationTemplate = "EmailConfirmationTemplate.html";
    public const string SetPasswordTemplate = "SetPasswordTemplate.html";

    public static async Task<string> GetEmailBodyAsync(
        string email,
        string templateName,
        string token,
        string baseRoute)
    {
        var encodedToken = System.Web.HttpUtility.UrlEncode(token);

        var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TemplateDirectory, templateName);
        var body = await File.ReadAllTextAsync(templatePath);

        var redirectUrl = $"{baseRoute}?token={encodedToken}&email={email}";
        body = body.Replace("{RedirectUrl}", redirectUrl);

        return body;
    }
}

public static class EmailSubjects
{
    public const string ResetPassword = "Reset Your Password";
    public const string ConfirmEmail = "Confirm Your Email";
    public const string SetPassword = "You've Been Invited — Set Your Password";
}
