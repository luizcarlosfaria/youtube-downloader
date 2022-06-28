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


    public (string output, string error, int exitCode) Run()
    {
        var processStartInfo = new ProcessStartInfo(this.processName, this.args.ToString());
        return this.Run(processStartInfo, true);
    }

    protected (string output, string error, int exitCode) Run(ProcessStartInfo processStartInfo, bool throwExceptionOnFalure = true)
    {
        (string standardOutput, string standardError, int exitCode) = this.PermissiveRun(processStartInfo);

        if (exitCode != 0 && throwExceptionOnFalure)
        {
            throw new ApplicationException("Download Failure", new Exception(standardError + Environment.NewLine + standardOutput));
        }

        return (standardOutput, standardError, exitCode);
    }

    protected (string output, string error, int exitCode) PermissiveRun(ProcessStartInfo processStartInfo)
    {
        processStartInfo.RedirectStandardError = true;
        processStartInfo.RedirectStandardOutput = true;

        var process = System.Diagnostics.Process.Start(processStartInfo);

        process.WaitForExit();

        string standardError = process.StandardError.ReadToEndAsync().GetAwaiter().GetResult();
        string standardOutput = process.StandardOutput.ReadToEndAsync().GetAwaiter().GetResult();

        return (standardOutput, standardError, process.ExitCode);
    }


}
