// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.IO.Abstraction;
using Newtonsoft.Json;

namespace Bicep.Wasm;

public partial class Interop
{
    private ReplEvaluator? consoleEvaluator;
    private string? consoleSessionId;

    public record ConsoleDiagnostic(string Code, string Level, string Message);

    public record ConsoleResult(string? Output, ConsoleDiagnostic[] Diagnostics);

    public string ResetConsole()
    {
        consoleEvaluator = new ReplEvaluator(CompileConsole);
        consoleSessionId = Guid.NewGuid().ToString();
        return consoleSessionId;
    }

    public ConsoleResult EvaluateConsole(string input, string sessionId)
    {
        if (consoleEvaluator is null || consoleSessionId != sessionId)
        {
            throw new InvalidOperationException("The console session is no longer available. Reset the console to start a new session.");
        }

        var result = consoleEvaluator.Evaluate(input, program =>
        {
            if (program.Children.Count(child => child is not Token { Type: TokenType.NewLine }) > 1)
            {
                throw new InvalidOperationException("Enter one expression or declaration at a time.");
            }
        });

        return new(
            result.Value is { } value ? JsonConvert.SerializeObject(value, Formatting.Indented) : null,
            [.. result.Diagnostics.Select(d => new ConsoleDiagnostic(d.Code, d.Level.ToString(), d.Message))]);
    }

    private Compilation CompileConsole(string content)
    {
        using var scope = serviceProvider.CreateScope();
        var compiler = scope.ServiceProvider.GetRequiredService<BicepCompiler>();
        var explorer = scope.ServiceProvider.GetRequiredService<IFileExplorer>();
        var uri = IOUri.FromFilePath("/console/session.biceprepl");
        var file = compiler.SourceFileFactory.CreateBicepReplFile(
            explorer.GetFile(uri),
            explorer.GetDirectory(IOUri.FromFilePath("/console")),
            content);
        var workspace = new ActiveSourceFileSet();
        workspace.UpsertSourceFile(file);
        return compiler.CreateCompilationWithoutRestore(uri, workspace);
    }
}
