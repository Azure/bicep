// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry;
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
    public void Artifact_services_can_be_overridden_independently()
    {
        var resolver = StrictMock.Of<IArtifactReferenceResolver>().Object;
        var restorer = StrictMock.Of<IArtifactRestorer>().Object;
        var publisher = StrictMock.Of<IArtifactPublisher>().Object;

        using var resolverServices = new ServiceCollection().AddSingleton(resolver).AddBicepCore().BuildServiceProvider();
        resolverServices.GetRequiredService<IArtifactReferenceResolver>().Should().BeSameAs(resolver);
        resolverServices.GetRequiredService<IArtifactRestorer>().Should().BeOfType<ArtifactRestorer>();
        resolverServices.GetRequiredService<IArtifactPublisher>().Should().BeOfType<ArtifactPublisher>();

        using var restorerServices = new ServiceCollection().AddSingleton(restorer).AddBicepCore().BuildServiceProvider();
        restorerServices.GetRequiredService<IArtifactRestorer>().Should().BeSameAs(restorer);
        restorerServices.GetRequiredService<IArtifactReferenceResolver>().Should().BeOfType<ArtifactReferenceResolver>();
        restorerServices.GetRequiredService<IArtifactPublisher>().Should().BeOfType<ArtifactPublisher>();

        using var publisherServices = new ServiceCollection().AddSingleton(publisher).AddBicepCore().BuildServiceProvider();
        publisherServices.GetRequiredService<IArtifactPublisher>().Should().BeSameAs(publisher);
        publisherServices.GetRequiredService<IArtifactReferenceResolver>().Should().BeOfType<ArtifactReferenceResolver>();
        publisherServices.GetRequiredService<IArtifactRestorer>().Should().BeOfType<ArtifactRestorer>();
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
