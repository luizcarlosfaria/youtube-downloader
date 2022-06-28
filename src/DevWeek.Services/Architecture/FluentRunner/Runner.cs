using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevWeek.Architecture.FluentRunner;
public class RunnerBuider
{
    private string processName;
    private StringBuilder args = new();


    public RunnerBuider()
    {

    }
        

    public RunnerBuider Process(string processName) { this.processName = processName; return this; }

    public RunnerBuider Arg(string arg) { args.Append(" " + arg + " "); return this; }


    public Task<ExecutionResult> RunAsync()
    {
        var processStartInfo = new ProcessStartInfo(this.processName, this.args.ToString());
        return this.Run(processStartInfo, true);
    }

    protected async Task<ExecutionResult> Run(ProcessStartInfo processStartInfo, bool throwExceptionOnFalure = true)
    {
        ExecutionResult executionResult = await this.PermissiveRunAsync(processStartInfo);

        if (executionResult.ExitCode != 0 && throwExceptionOnFalure)
        {
            throw new ApplicationException("Download Failure", new Exception(executionResult.StandardError + Environment.NewLine + executionResult.StandardOutput));
        }

        return executionResult;
    }

    protected async Task<ExecutionResult> PermissiveRunAsync(ProcessStartInfo processStartInfo)
    {
        processStartInfo.RedirectStandardError = true;
        processStartInfo.RedirectStandardOutput = true;

        var process = System.Diagnostics.Process.Start(processStartInfo);

        process.WaitForExit();

        string standardError = await process.StandardError.ReadToEndAsync();
        string standardOutput = await process.StandardOutput.ReadToEndAsync();

        return new ExecutionResult(standardOutput, standardError, process.ExitCode);
    }


}

public class ExecutionResult
{
    public string StandardError { get; }

    public string StandardOutput { get; }

    public int ExitCode { get; }

    public ExecutionResult( string standardOutput, string standardError, int exitCode)
    {
        this.StandardError = standardError;
        this.StandardOutput = standardOutput;
        this.ExitCode = exitCode;
    }
}
