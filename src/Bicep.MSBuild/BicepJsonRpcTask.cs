// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using StreamJsonRpc;
using Task = System.Threading.Tasks.Task;

namespace Azure.Bicep.MSBuild;

#pragma warning disable IDE1006
    public record CompileRequest(
        string path);

    public record CompileResponse(
        bool success,
        ImmutableArray<CompileResponseDiagnostic> diagnostics,
        string? contents);

    public record CompileResponseDiagnostic(
        int line,
        int @char,
        string level,
        string code,
        string message);
#pragma warning restore IDE1006

public class BicepJsonRpcTask : BicepToolTask
{
    [Required]
    public ITaskItem[]? SourceFiles { get; set; }

    private readonly string pipeName;
    private NamedPipeServerStream pipeStream;
    private readonly CancellationTokenSource cancellationTokenSource = new();

    public BicepJsonRpcTask() : base()
    {
        pipeName = GeneratePipeName();
        pipeStream = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
    }

    private static string GeneratePipeName()
    {
        var randomSuffix = Guid.NewGuid().ToString();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return @"\\.\pipe\" + $"bicep-{randomSuffix}-sock";
        }

        return Path.Combine(Path.GetTempPath(), $"bicep-{randomSuffix}.sock");
    }

    protected override string GenerateCommandLineCommands()
    {
        var builder = new CommandLineBuilder(quoteHyphensOnCommandLine: false, useNewLineSeparator: false);

        builder.AppendSwitch("jsonrpc");

        builder.AppendSwitch("--pipe");
        builder.AppendFileNameIfNotNull(pipeName);

        return builder.ToString();
    }

    protected override void ProcessStarted()
    {
        _ = Task.Factory.StartNew(CompileBicepFiles, cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        base.ProcessStarted();
    }

    protected async Task CompileBicepFiles()
    {
        try {
            var cancelToken = cancellationTokenSource.Token;
            await pipeStream.WaitForConnectionAsync(cancelToken);

            var jsonRpc = JsonRpc.Attach(pipeStream);


            var request = new CompileRequest("/Users/ant/Desktop/issue12349/main.bicep");
            var response = await jsonRpc.InvokeWithCancellationAsync<CompileResponse>("bicep/compile", new [] { request }, cancelToken);

            if (response.contents is {})
            {
                File.WriteAllText("/Users/ant/Desktop/issue12349/main.json", response.contents);
            }
        }
        finally {
            CloseStream();
        }
    }

    public override void Cancel()
    {
        CloseStream();
        base.Cancel();
    }

    public override bool Execute()
    {
        try {
            return base.Execute();
        }
        finally {
            CloseStream();
        }
    }

    private void CloseStream()
    {
        cancellationTokenSource.Cancel();
        pipeStream.Dispose();
    }
}
