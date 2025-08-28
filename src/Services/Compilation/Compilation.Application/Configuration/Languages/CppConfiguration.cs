using System.Diagnostics;
using Compilation.Application.Configuration.Languages.Base;

namespace Compilation.Application.Configuration.Languages;

public class CppConfiguration : LanguageConfigurationBase
{
    public override string Name => "C++";
    public override string FileExtension => ".cpp";
    public override bool RequiresCompilation => true;

    public override string GetFileName(string fileId) => $"cpp-file_{fileId}.cpp";

    public override string GetExecutableFileName(string fileId)=> $"cpp-file_{fileId}.exe";

    public override ProcessStartInfo GetCompileProcessStartInfo(string sourceFileName, string executableFileName)
    {
        return new ProcessStartInfo
        {
            FileName = "g++",
            ArgumentList = {sourceFileName, "-o", executableFileName},
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };
    }

    public override ProcessStartInfo GetExecuteProcessStartInfo(string executableFileName)
    {
        return new ProcessStartInfo
        {
            FileName = executableFileName,
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };
        
    }
    
}