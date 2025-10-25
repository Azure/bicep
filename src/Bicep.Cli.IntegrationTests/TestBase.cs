// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.UnitTests;
using Bicep.Core;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Catalog.Implementation;
using Bicep.Core.Registry.Catalog.Implementation.PublicRegistries;
using Bicep.Core.Samples;
using Bicep.Core.Text;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Utils;
using Bicep.TextFixtures.Utils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Spectre.Console;
using TestEnvironment = Bicep.Core.UnitTests.Utils.TestEnvironment;

namespace Bicep.Cli.IntegrationTests
{
    public abstract class TestBase : Bicep.Core.UnitTests.TestBase
    {
        private static BicepCompiler CreateCompiler(IContainerRegistryClientFactory clientFactory, ITemplateSpecRepositoryFactory templateSpecRepositoryFactory, IPublicModuleIndexHttpClient? moduleMetadataClient)
            => ServiceBuilder.Create(
                services =>
                {
                    services
                        .AddSingleton(clientFactory)
                        .AddSingleton(templateSpecRepositoryFactory);

                    IServiceCollectionExtensions.AddMockHttpClientIfNotNull(services, moduleMetadataClient);
                }
                ).GetCompiler();

        protected const string BuildSummaryFailedRegex = @"Build failed: (\d*) Warning\(s\), ([1-9][0-9]*) Error\(s\)";
        protected const string BuildSummarySucceededRegex = @"Build succeeded: (\d*) Warning\(s\), 0 Error\(s\)";

        protected static readonly MockRepository Repository = new(MockBehavior.Strict);

        protected record InvocationSettings
        {
            public static readonly InvocationSettings Default = new();

            public FeatureProviderOverrides? FeatureOverrides { get; init; }
            public IContainerRegistryClientFactory ClientFactory { get; init; }
            public ITemplateSpecRepositoryFactory TemplateSpecRepositoryFactory { get; init; }
            public IEnvironment? Environment { get; init; }
            public IPublicModuleIndexHttpClient ModuleMetadataClient { get; init; }

            public InvocationSettings(
                FeatureProviderOverrides? FeatureOverrides = null,
                IContainerRegistryClientFactory? ClientFactory = null,
                ITemplateSpecRepositoryFactory? TemplateSpecRepositoryFactory = null,
                IEnvironment? Environment = null,
                IPublicModuleIndexHttpClient? ModuleMetadataClient = null)
            {
                this.FeatureOverrides = FeatureOverrides;
                this.ClientFactory = ClientFactory ?? Repository.Create<IContainerRegistryClientFactory>().Object;
                this.TemplateSpecRepositoryFactory = TemplateSpecRepositoryFactory ?? Repository.Create<ITemplateSpecRepositoryFactory>().Object;
                this.Environment = Environment;

                this.ModuleMetadataClient = ModuleMetadataClient ?? new MockPublicModuleIndexHttpClient(new());
            }

            public InvocationSettings WithArtifactManager(TestExternalArtifactManager artifactManager, TestContext testContext) =>
                this with
                {
                    FeatureOverrides = (this.FeatureOverrides ?? new(testContext)) with { RegistryEnabled = true },
                    ClientFactory = artifactManager.ContainerRegistryClientFactory,
                    TemplateSpecRepositoryFactory = artifactManager.TemplateSpecRepositoryFactory
                };
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

                        IServiceCollectionExtensions.AddMockHttpClientIfNotNull(services, settings.ModuleMetadataClient);

                        services
                            .AddSingletonIfNotNull(settings.Environment ?? BicepTestConstants.EmptyEnvironment)
                            .AddSingletonIfNotNull(settings.ClientFactory)
                            .AddSingletonIfNotNull(settings.TemplateSpecRepositoryFactory)
                            .AddSingleton(AnsiConsole.Create(new()
                            {
                                Ansi = AnsiSupport.Yes,
                                ColorSystem = ColorSystemSupport.Standard,
                                Interactive = InteractionSupport.No,
                                Out = new AnsiConsoleOutput(@out),
                            }));

                        registerAction?.Invoke(services);
                    }
                   )
                   .RunAsync(args.ToArrayExcludingNull(), cancellationToken));

        protected static Task<CliResult> Bicep(params string[] args) => Bicep(InvocationSettings.Default, args);

        protected static Task<CliResult> Bicep(Action<IServiceCollection> registerAction, params string[] args)
            => Bicep(InvocationSettings.Default, registerAction, CancellationToken.None, args);

        protected static Task<CliResult> Bicep(Action<IServiceCollection> registerAction, CancellationToken cancellationToken, params string[] args)
            => Bicep(InvocationSettings.Default, registerAction, cancellationToken, args);

        protected static Task<CliResult> Bicep(InvocationSettings settings, params string?[] args /*null args are ignored*/)
            => Bicep(settings, null, CancellationToken.None, args);

        protected static void AssertNoErrors(string error)
        {
            foreach (var line in error.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries))
            {
                line.Should().NotContain(") : Error ");
            }
        }

        protected static async Task<IEnumerable<string>> GetAllDiagnostics(string bicepFilePath, IContainerRegistryClientFactory clientFactory, ITemplateSpecRepositoryFactory templateSpecRepositoryFactory, IPublicModuleIndexHttpClient? moduleMetadataClient = null)
        {
            var compilation = await CreateCompiler(clientFactory, templateSpecRepositoryFactory, moduleMetadataClient).CreateCompilation(PathHelper.FilePathToFileUrl(bicepFilePath).ToIOUri());

            var output = new List<string>();
            foreach (var (bicepFile, diagnostics) in compilation.GetAllDiagnosticsByBicepFile())
            {
                foreach (var diagnostic in diagnostics)
                {
                    var (line, character) = TextCoordinateConverter.GetPosition(bicepFile.LineStarts, diagnostic.Span.Position);
                    var codeDescription = diagnostic.Uri == null ? string.Empty : $" [{diagnostic.Uri.AbsoluteUri}]";
                    output.Add($"{bicepFile.FileHandle.Uri}({line + 1},{character + 1}) : {diagnostic.Level} {diagnostic.Code}: {diagnostic.Message}{codeDescription}");
                }
            }

            return output;
        }

        protected static async Task<IEnumerable<string>> GetAllParamDiagnostics(ServiceBuilder serviceBuilder, string paramFilePath)
        {
            var compiler = serviceBuilder.Build().GetCompiler();

            var compilation = await compiler.CreateCompilation(PathHelper.FilePathToFileUrl(paramFilePath).ToIOUri());

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

        protected async Task<InvocationSettings> CreateDefaultSettingsWithDefaultMockRegistry()
            => CreateDefaultSettings().WithArtifactManager(await CreateDefaultExternalArtifactManager(), TestContext);

        protected InvocationSettings CreateDefaultSettings(Func<FeatureProviderOverrides, FeatureProviderOverrides>? featureOverrides = null) =>
            new()
            {
                FeatureOverrides = featureOverrides?.Invoke(CreateDefaultFeatureProviderOverrides()) ?? CreateDefaultFeatureProviderOverrides(),
                Environment = CreateDefaultEnvironment()
            };

        protected FeatureProviderOverrides CreateDefaultFeatureProviderOverrides() => new(TestContext);

        protected async Task<TestExternalArtifactManager> CreateDefaultExternalArtifactManager()
        {
            FileHelper.GetCacheRootDirectory(TestContext).EnsureExists();

            return await MockRegistry.CreateDefaultExternalArtifactManager(TestContext);
        }

        protected static IEnvironment CreateDefaultEnvironment() => TestEnvironment.Default.WithVariables(
            ("stringEnvVariableName", "test"),
            ("intEnvVariableName", "100"),
            ("boolEnvironmentVariable", "true")
        );
    }
}
