// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Cli.UnitTests;
using Bicep.Core;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Registry.PublicRegistry;
using Bicep.Core.Text;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Utils;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Bicep.Cli.IntegrationTests
{
    public abstract class TestBase : Bicep.Core.UnitTests.TestBase
    {
        private static BicepCompiler CreateCompiler(IContainerRegistryClientFactory clientFactory, ITemplateSpecRepositoryFactory templateSpecRepositoryFactory, IPublicRegistryModuleMetadataClient? moduleMetadataClient)
            => ServiceBuilder.Create(
                services =>
                {
                    services
                        .AddSingleton(clientFactory)
                        .AddSingleton(templateSpecRepositoryFactory)
                        .AddSingleton<IPublicRegistryModuleMetadataProvider, PublicRegistryModuleMetadataProvider>();
                    IServiceCollectionExtensions.AddMockHttpClientIfNonNull(services, moduleMetadataClient); //asdfg does this work?

                }
                ).GetCompiler();

        protected const string BuildSummaryFailedRegex = @"Build failed: (\d*) Warning\(s\), ([1-9][0-9]*) Error\(s\)";
        protected const string BuildSummarySucceededRegex = @"Build succeeded: (\d*) Warning\(s\), 0 Error\(s\)";

        protected static readonly MockRepository Repository = new(MockBehavior.Strict);

        protected record InvocationSettings
        {
            public FeatureProviderOverrides? FeatureOverrides { get; init; }
            public IContainerRegistryClientFactory ClientFactory { get; init; }
            public ITemplateSpecRepositoryFactory TemplateSpecRepositoryFactory { get; init; }
            public IEnvironment? Environment { get; init; }
            public IPublicRegistryModuleMetadataClient ModuleMetadataClient { get; init; }

            public InvocationSettings(
                FeatureProviderOverrides? FeatureOverrides = null,
                IContainerRegistryClientFactory? ClientFactory = null,
                ITemplateSpecRepositoryFactory? TemplateSpecRepositoryFactory = null,
                IEnvironment? Environment = null,
                IPublicRegistryModuleMetadataClient? ModuleMetadataClient = null)
            {
                this.FeatureOverrides = FeatureOverrides;
                this.ClientFactory = ClientFactory ?? Repository.Create<IContainerRegistryClientFactory>().Object;
                this.TemplateSpecRepositoryFactory = TemplateSpecRepositoryFactory ?? Repository.Create<ITemplateSpecRepositoryFactory>().Object;
                this.Environment = Environment;

                // This keeps PublicRegistryModuleMetadataProvider from going to the web during CLI integration tests
                this.ModuleMetadataClient = ModuleMetadataClient ?? StrictMock.Of<IPublicRegistryModuleMetadataClient>().Object;
            }
        }

        protected static Task<CliResult> Bicep(InvocationSettings settings, Action<IServiceCollection>? registerAction, CancellationToken cancellationToken, params string?[] args /*null args are ignored*/)
            => TextWriterHelper.InvokeWriterAction((@out, err)
                => new Program(
                    new(Output: @out, Error: err),
                    services =>
                    {
                        if (settings.FeatureOverrides is { })
                        {
                            services.WithFeatureOverrides(settings.FeatureOverrides);
                        }

                        IServiceCollectionExtensions.AddMockHttpClientIfNonNull(services, settings.ModuleMetadataClient);

                        services
                            .AddSingletonIfNonNull(settings.Environment ?? BicepTestConstants.EmptyEnvironment)
                            .AddSingletonIfNonNull(settings.ClientFactory)
                            .AddSingletonIfNonNull(settings.TemplateSpecRepositoryFactory);

                        registerAction?.Invoke(services);
                    }
                   )
                   .RunAsync(args.ToArrayExcludingNull(), cancellationToken));

        protected static Task<CliResult> Bicep(params string[] args) => Bicep(new InvocationSettings(), args);

        protected static Task<CliResult> Bicep(Action<IServiceCollection> registerAction, params string[] args)
            => Bicep(new InvocationSettings(), registerAction, CancellationToken.None, args);

        protected static Task<CliResult> Bicep(Action<IServiceCollection> registerAction, CancellationToken cancellationToken, params string[] args)
            => Bicep(new InvocationSettings(), registerAction, cancellationToken, args);

        protected static Task<CliResult> Bicep(InvocationSettings settings, params string?[] args /*null args are ignored*/)
            => Bicep(settings, null, CancellationToken.None, args);

        protected static void AssertNoErrors(string error)
        {
            foreach (var line in error.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries))
            {
                line.Should().NotContain(") : Error ");
            }
        }

        protected static async Task<IEnumerable<string>> GetAllDiagnostics(string bicepFilePath, IContainerRegistryClientFactory clientFactory, ITemplateSpecRepositoryFactory templateSpecRepositoryFactory, IPublicRegistryModuleMetadataClient? moduleMetadataClient = null)
        {
            var compilation = await CreateCompiler(clientFactory, templateSpecRepositoryFactory, moduleMetadataClient).CreateCompilation(PathHelper.FilePathToFileUrl(bicepFilePath));

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
