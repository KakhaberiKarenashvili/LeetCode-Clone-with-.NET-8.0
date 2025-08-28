using System.Diagnostics;

namespace Compilation.Application.Configuration.Languages.Base;

public abstract class LanguageConfigurationBase
{
    public abstract string Name { get; }
    public abstract string FileExtension { get; }
    public abstract bool RequiresCompilation { get; }
    public abstract string GetFileName(string fileId);
    public abstract string GetExecutableFileName(string fileId);
    public abstract ProcessStartInfo GetCompileProcessStartInfo(string sourceFileName,string executableFileName);
    public abstract ProcessStartInfo GetExecuteProcessStartInfo(string executableFileName);
    
}