// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.UnitTests;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using FluentAssertions;
using System;
using System.Collections.Generic;

namespace Bicep.Cli.IntegrationTests
{
    public class TestBase
    {
        protected const string BuildSummaryFailedRegex = @"Build failed: (\d*) Warning\(s\), ([1-9][0-9]*) Error\(s\)";
        protected const string BuildSummarySucceededRegex = @"Build succeeded: (\d*) Warning\(s\), 0 Error\(s\)";

        protected static (string output, string error, int result) Bicep(params string[] args)
        {
            return TextWriterHelper.InvokeWriterAction((@out, err) =>
            {
                return new Program(new InvocationContext(TestTypeHelper.CreateEmptyProvider(), @out, err, EmitterSettingsHelper.DefaultTestSettings)).Run(args);
            });
        }

        protected static void AssertNoErrors(string error)
        {
            foreach (var line in error.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                line.Should().NotContain(") : Error ");
            }
        }

        protected static IEnumerable<string> GetAllDiagnostics(string bicepFilePath)
        {
            var dispatcher = new ModuleDispatcher(new DefaultModuleRegistryProvider(BicepTestConstants.FileResolver));
            var sourceFileGrouping = SourceFileGroupingBuilder.Build(BicepTestConstants.FileResolver, dispatcher, new Workspace(), PathHelper.FilePathToFileUrl(bicepFilePath));
            var compilation = new Compilation(TestTypeHelper.CreateEmptyProvider(), sourceFileGrouping);

            var output = new List<string>();
            foreach (var (bicepFile, diagnostics) in compilation.GetAllDiagnosticsByBicepFile())
            {
                foreach (var diagnostic in diagnostics)
                {
                    var (line, character) = TextCoordinateConverter.GetPosition(bicepFile.LineStarts, diagnostic.Span.Position);
                    var codeDescription = diagnostic.Uri == null ? string.Empty : $" [{diagnostic.Uri.AbsoluteUri}]";
                    output.Add($"{bicepFile.FileUri.LocalPath}({line + 1},{character + 1}) : {diagnostic.Level} {diagnostic.Code}: {diagnostic.Message}{codeDescription}");
                }
            }

            return output;
        }
    }
}
