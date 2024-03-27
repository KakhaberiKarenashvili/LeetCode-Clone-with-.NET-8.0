using System.Diagnostics;
using System.IO;
using System.Text;
using informaticsge.models;
using Microsoft.AspNetCore.Mvc;

namespace informaticsge.Controllers;

[Route("/api/compiler")]
[ApiController]
public class Compiler : ControllerBase
{
    
    [HttpPost("/compile")]
    [Consumes("text/plain")]
    public async Task<CompilationResult> CompileAndRunCppCodeAsync([FromBody]string code,string input)
    {
        
    Console.WriteLine(code);
    var cppFileName = "temp.cpp";
    var exeFileName = "temp.exe";
    
    // Write the C++ code to a temporary file
    await System.IO.File.WriteAllTextAsync(cppFileName, code);

    // Execute the C++ code file using the command prompt
    var processInfo = new ProcessStartInfo
    {
        FileName = "cmd.exe",
        Arguments = $"/c g++ {cppFileName} -o {exeFileName} && {exeFileName}", // Compile and execute the code file
        RedirectStandardInput = true,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };

    using (var process = new Process())
    {
        process.StartInfo = processInfo;
        process.Start();

        // Pass the input string to the running process
        process.StandardInput.WriteLine(input);
        process.StandardInput.Close();

        // Read the output from the process
        var output = await process.StandardOutput.ReadToEndAsync();

        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            var errorOutput = await process.StandardError.ReadToEndAsync();
            return new CompilationResult { Success = false, Error = errorOutput };
        }
        else
        {
            return new CompilationResult { Success = true, Output = output };
        }
    }
}
    
    
    
}
