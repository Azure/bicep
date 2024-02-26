// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.UnitTests;
using Bicep.Core;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Text;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Utils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Bicep.Cli.IntegrationTests
{
    public abstract class TestBase : Bicep.Core.UnitTests.TestBase
    {
        private static BicepCompiler CreateCompiler(IContainerRegistryClientFactory clientFactory, ITemplateSpecRepositoryFactory templateSpecRepositoryFactory)
            => ServiceBuilder.Create(
                x => x.AddSingleton(clientFactory).AddSingleton(templateSpecRepositoryFactory)).GetCompiler();

        protected const string BuildSummaryFailedRegex = @"Build failed: (\d*) Warning\(s\), ([1-9][0-9]*) Error\(s\)";
        protected const string BuildSummarySucceededRegex = @"Build succeeded: (\d*) Warning\(s\), 0 Error\(s\)";

        protected static readonly MockRepository Repository = new(MockBehavior.Strict);

        protected record InvocationSettings(
            FeatureProviderOverrides? FeatureOverrides,
            IContainerRegistryClientFactory ClientFactory,
            ITemplateSpecRepositoryFactory TemplateSpecRepositoryFactory,
            IEnvironment? Environment = null);

        protected static Task<CliResult> Bicep(params string[] args) => Bicep(CreateDefaultSettings(), args);

        protected static InvocationSettings CreateDefaultSettings() => new(
            FeatureOverrides: null,
            ClientFactory: Repository.Create<IContainerRegistryClientFactory>().Object,
            TemplateSpecRepositoryFactory: Repository.Create<ITemplateSpecRepositoryFactory>().Object);

        protected static Task<CliResult> Bicep(Action<IServiceCollection> registerAction, CancellationToken cancellationToken, params string[] args)
            => TextWriterHelper.InvokeWriterAction((@out, err)
                => new Program(new(Output: @out, Error: err), registerAction)
                    .RunAsync(args, cancellationToken));

        protected static Task<CliResult> Bicep(Action<IServiceCollection> registerAction, params string[] args)
            => Bicep(registerAction, CancellationToken.None, args);

        protected static Task<CliResult> Bicep(InvocationSettings settings, params string?[] args /*null args are ignored*/)
            => Bicep(services =>
                {
                    if (settings.FeatureOverrides is { })
                    {
                        services.WithFeatureOverrides(settings.FeatureOverrides);
                    }

                    services
                        .AddSingleton(settings.Environment ?? BicepTestConstants.EmptyEnvironment)
                        .AddSingleton(settings.ClientFactory)
                        .AddSingleton(settings.TemplateSpecRepositoryFactory);
                }, CancellationToken.None, args.ToArrayExcludingNull());

        protected static void AssertNoErrors(string error)
        {
            foreach (var line in error.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries))
            {
                line.Should().NotContain(") : Error ");
            }
        }

        protected static async Task<IEnumerable<string>> GetAllDiagnostics(string bicepFilePath, IContainerRegistryClientFactory clientFactory, ITemplateSpecRepositoryFactory templateSpecRepositoryFactory)
        {
            var compilation = await CreateCompiler(clientFactory, templateSpecRepositoryFactory).CreateCompilation(PathHelper.FilePathToFileUrl(bicepFilePath));

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

        protected static async Task<IEnumerable<string>> GetAllParamDiagnostics(string paramFilePath)
        {
            var compiler = new ServiceBuilder().Build().GetCompiler();

            var compilation = await compiler.CreateCompilation(PathHelper.FilePathToFileUrl(paramFilePath));

            var semanticModel = compilation.GetEntrypointSemanticModel();

            var output = new List<string>();
            foreach (var diagnostic in semanticModel.GetAllDiagnostics())
            {
                var (line, character) = TextCoordinateConverter.GetPosition(semanticModel.SourceFile.LineStarts, diagnostic.Span.Position);
                var codeDescription = diagnostic.Uri == null ? string.Empty : $" [{diagnostic.Uri.AbsoluteUri}]";
                output.Add($"{paramFilePath}({line + 1},{character + 1}) : {diagnostic.Level} {diagnostic.Code}: {diagnostic.Message}{codeDescription}");
            }

            return output;
        }
    }
}
