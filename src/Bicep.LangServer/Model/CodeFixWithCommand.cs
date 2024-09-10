// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.CodeAction;
using System.Security.Policy;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Collections.Immutable;

namespace Bicep.LanguageServer.Model;

public class CodeFixWithCommand : CodeFix
{
    public Command? Command { init; get; }

    public CodeFixWithCommand(string title, bool isPreferred, CodeFixKind kind, CodeReplacement[] replacements, Command? Command)
        : base(title, isPreferred, kind, replacements[0], replacements[1..])
    {
        this.Command = Command;
    }

    public static CodeFixWithCommand CreateWithRenameCommand(string title, bool isPreferred, CodeFixKind kind, IEnumerable< CodeReplacement> replacements, Uri uri, Position renamePosition)
    {
        var renameCommand = renamePosition == null ? null :
            new Command()
            {
                Name = "bicep.internal.startRename",
                Title = "Rename new identifier"
            }
            .WithArguments(
                uri.ToString(),
                new
                {
                    line = renamePosition.Line,
                    character = renamePosition.Character,
                }
            );
        return new CodeFixWithCommand(title, isPreferred, kind, replacements.ToArray(), renameCommand);
    }
}
