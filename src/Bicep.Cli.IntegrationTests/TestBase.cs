// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.UnitTests;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Bicep.Cli.IntegrationTests
{
    public abstract class TestBase
    {
        protected const string BuildSummaryFailedRegex = @"Build failed: (\d*) Warning\(s\), ([1-9][0-9]*) Error\(s\)";
        protected const string BuildSummarySucceededRegex = @"Build succeeded: (\d*) Warning\(s\), 0 Error\(s\)";

        protected static readonly MockRepository Repository = new(MockBehavior.Strict);

        protected record InvocationSettings(IFeatureProvider Features, IContainerRegistryClientFactory ClientFactory, ITemplateSpecRepositoryFactory TemplateSpecRepositoryFactory);

        protected static Task<(string output, string error, int result)> Bicep(params string[] args) => Bicep(CreateDefaultSettings(), args);

        protected static InvocationSettings CreateDefaultSettings() => new(
            Features: BicepTestConstants.Features,
            ClientFactory: Repository.Create<IContainerRegistryClientFactory>().Object,
            TemplateSpecRepositoryFactory: Repository.Create<ITemplateSpecRepositoryFactory>().Object);

        protected static Task<(string output, string error, int result)> Bicep(InvocationSettings settings, params string[] args) =>
            TextWriterHelper.InvokeWriterAction((@out, err) =>
                new Program(new InvocationContext(
                    TestTypeHelper.CreateEmptyAzResourceTypeLoader(),
                    @out,
                    err,
                    features: settings.Features,
                    clientFactory: settings.ClientFactory,
                    templateSpecRepositoryFactory: settings.TemplateSpecRepositoryFactory)).RunAsync(args));

        protected static void AssertNoErrors(string error)
        {
            foreach (var line in error.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                line.Should().NotContain(") : Error ");
            }
        }

        protected static IEnumerable<string> GetAllDiagnostics(string bicepFilePath, IContainerRegistryClientFactory clientFactory, ITemplateSpecRepositoryFactory templateSpecRepositoryFactory)
        {
            var dispatcher = new ModuleDispatcher(new DefaultModuleRegistryProvider(BicepTestConstants.FileResolver, clientFactory, templateSpecRepositoryFactory, BicepTestConstants.Features));
            var sourceFileGrouping = SourceFileGroupingBuilder.Build(BicepTestConstants.FileResolver, dispatcher, new Workspace(), PathHelper.FilePathToFileUrl(bicepFilePath));
            var compilation = new Compilation(TestTypeHelper.CreateEmptyProvider(), sourceFileGrouping, BicepTestConstants.BuiltInConfiguration);

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
