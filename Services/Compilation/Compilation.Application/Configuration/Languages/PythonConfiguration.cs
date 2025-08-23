using System.Diagnostics;
using Compilation.Application.Configuration.Languages.Base;

namespace Compilation.Application.Configuration.Languages;

public class PythonConfiguration : LanguageConfigurationBase
{
    public override string Name => "Python";
    public override string FileExtension => ".py";
    public override bool RequiresCompilation => false;

    public override string GetFileName(string fileId) => $"python-file_{fileId}.py";

    public override string GetExecutableFileName(string fileId) => GetFileName(fileId);

    public override ProcessStartInfo GetCompileProcessStartInfo(string sourceFileName, string executableFileName)
    {
        throw new Exception("Python does not require compilation");
    }

    public override ProcessStartInfo GetExecuteProcessStartInfo(string executableFileName)
    {
        return new ProcessStartInfo
        {
            FileName = "python",
            Arguments = executableFileName,
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };
    }
}