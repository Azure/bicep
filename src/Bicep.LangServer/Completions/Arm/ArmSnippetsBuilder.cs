// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Reflection;
using Bicep.Core.FileSystem;
using Bicep.Core.PrettyPrint;
using Bicep.Core.PrettyPrint.Options;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;
using Bicep.Decompiler;
using Newtonsoft.Json.Linq;

namespace Bicep.LanguageServer.Completions.Arm
{
    public static class ArmSnippetsBuilder
    {
        public static Dictionary<string, (string?, string?)> GetSnippets()
        {
            string? currentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
            string armTemplatesFolder = Path.Combine(currentDirectory!, "Completions\\Arm\\ArmTemplates");
            string[] armTemplates = Directory.GetFiles(armTemplatesFolder);

            Dictionary<string, (string?, string?)> snippets = new Dictionary<string, (string?, string?)>();

            foreach (string template in armTemplates)
            {
                string content = File.ReadAllText(template);

                JObject jObject = JObject.Parse(content, new JsonLoadSettings
                {
                    CommentHandling = CommentHandling.Ignore,
                    LineInfoHandling = LineInfoHandling.Load,
                });

                string? context = jObject["context"]?.Value<string>();
                string? prefix = jObject["prefix"]?.Value<string>();
                string? description = jObject["description"]?.Value<string>();

                if (context is null || !context.Equals("resources") ||
                    prefix is null)
                {
                    continue;
                }

                JToken? resources = jObject["resources"];

                if (resources != null)
                {
                    Uri uri = new Uri(template);
                    ProgramSyntax program = TemplateConverter.DecompileTemplate(new Workspace(), new FileResolver(), uri, content);
                    var syntaxTree = new SyntaxTree(uri, ImmutableArray<int>.Empty, program);
                    string decompiledString = PrettyPrinter.PrintProgram(syntaxTree.ProgramSyntax, new PrettyPrintOptions(NewlineOption.Auto, IndentKindOption.Space, 2, false));

                    if (!string.IsNullOrWhiteSpace(decompiledString))
                    {
                        snippets.Add(prefix, (decompiledString, description));
                    }
                }
            }

            return snippets;
        }
    }
}
