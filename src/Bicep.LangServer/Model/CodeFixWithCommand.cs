// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.CodeAction;
using System.Security.Policy;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Collections.Immutable;
using Bicep.LanguageServer.Telemetry;

namespace Bicep.LanguageServer.Model;

public class CodeFixWithCommand : CodeFix
{
    public Command? Command { init; get; }

    public CodeFixWithCommand(string title, bool isPreferred, CodeFixKind kind, CodeReplacement[] replacements, Command? Command)
        : base(title, isPreferred, kind, replacements[0], replacements[1..])
    {
        this.Command = Command;
    }

    public static CodeFixWithCommand CreateWithPostExtractionCommand(string title, bool isPreferred, CodeFixKind kind, IEnumerable<CodeReplacement> replacements, Uri uri, Position renamePosition, BicepTelemetryEvent telemetryEvent)
    {
        var renameCommand = renamePosition == null ? null :
            new Command()
            {
                Name = "bicep.internal.postExtraction",
                Title = "Post-extraction operations"
            }
            .WithArguments(
                uri.ToString(),
                new
                {
                    line = renamePosition.Line,
                    character = renamePosition.Character,
                },
                telemetryEvent
            );
        return new CodeFixWithCommand(title, isPreferred, kind, replacements.ToArray(), renameCommand);
    }
}
