// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Artifacts.Registries;
using Bicep.Core.Artifacts.Resolution;
using Bicep.Core.Artifacts.Restore;
using Bicep.Core.UnitTests.Mock;
using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests;

[TestClass]
public class BicepCoreServiceCollectionExtensionsTests : TestBase
{
    [TestMethod]
    public void Registries_are_registered_once_and_share_their_concrete_instances()
    {
        using var services = new ServiceCollection().AddBicepCore().AddBicepCore().BuildServiceProvider();
        var registries = services.GetServices<IArtifactRegistry>().ToArray();

        registries.Should().HaveCount(2);
        registries.Should().ContainSingle(registry => ReferenceEquals(registry, services.GetRequiredService<OciArtifactRegistry>()));
        registries.Should().ContainSingle(registry => ReferenceEquals(registry, services.GetRequiredService<TemplateSpecModuleRegistry>()));
    }

    [TestMethod]
    public void Artifact_services_can_be_overridden_independently()
    {
        var resolver = StrictMock.Of<IArtifactReferenceResolver>().Object;
        var restorer = StrictMock.Of<IArtifactRestorer>().Object;

        using var resolverServices = new ServiceCollection().AddSingleton(resolver).AddBicepCore().BuildServiceProvider();
        resolverServices.GetRequiredService<IArtifactReferenceResolver>().Should().BeSameAs(resolver);
        resolverServices.GetRequiredService<IArtifactRestorer>().Should().BeOfType<ArtifactRestorer>();

        using var restorerServices = new ServiceCollection().AddSingleton(restorer).AddBicepCore().BuildServiceProvider();
        restorerServices.GetRequiredService<IArtifactRestorer>().Should().BeSameAs(restorer);
        restorerServices.GetRequiredService<IArtifactReferenceResolver>().Should().BeOfType<ArtifactReferenceResolver>();
    }

    [TestMethod]
    public async Task BicepCompiler_can_be_constructed()
    {
        var fileExplorer = new InMemoryFileExplorer();
        var bicepUri = IOUri.FromFilePath("/main.bicep");
        fileExplorer.GetFile(bicepUri).WriteAllText("""
        param foo string
        output foo string = foo
        """);

        var services = new ServiceCollection()
            .AddSingleton<IFileExplorer>(fileExplorer)
            .AddBicepCore()
            .BuildServiceProvider();

        var compiler = services.GetRequiredService<BicepCompiler>();
        var result = await compiler.CompileBicepFile(bicepUri);

        result.Template.Should().Contain("[parameters('foo')]");
    }
}
