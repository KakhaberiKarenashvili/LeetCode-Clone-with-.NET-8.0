using Compilation.Application.Configuration.Languages.Base;

namespace Compilation.Application.Configuration.Languages;

public class LanguageConfigurationFactory
{
    private static readonly Dictionary<string, Func<LanguageConfigurationBase>> _languageConfigurations = new()
    {
        {"cpp", () => new CppConfiguration() },
        {"python", () => new PythonConfiguration()}
    };

    public static LanguageConfigurationBase GetLanguageConfiguration(string language)
    {
        if (_languageConfigurations.TryGetValue(language.ToLower(), out var factory))
        {
            return factory();
        }
        
        throw new NotSupportedException($"Language {language} is not supported");
    }
}