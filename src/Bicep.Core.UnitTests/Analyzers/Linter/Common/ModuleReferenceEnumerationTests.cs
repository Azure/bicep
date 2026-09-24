// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions.TestingHelpers;
using Bicep.Core.Analyzers.Linter.Common;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Analyzers.Linter.Common;

[TestClass]
public class ModuleReferenceEnumerationTests
{
    [TestMethod]
    public void EnumerateAllLocalModuleModelsTransitively_SimpleChain_IncludesEveryFileInTheChain()
    {
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                module a 'a.bicep' = {
                  name: 'a'
                }
                """),
            ("a.bicep", """
                module b 'b.bicep' = {
                  name: 'b'
                }
                """),
            ("b.bicep", """
                output value string = 'leaf'
                """));

        var entryPoint = result.Compilation.GetEntrypointSemanticModel();
        var models = entryPoint.EnumerateAllLocalModuleModelsTransitively().ToList();

        models.Should().HaveCount(2);
        models.Select(m => m.SourceFile.FileHandle.Uri.ToString()).Should().Contain(uri => uri.EndsWith("a.bicep"));
        models.Select(m => m.SourceFile.FileHandle.Uri.ToString()).Should().Contain(uri => uri.EndsWith("b.bicep"));
    }

    [TestMethod]
    public void EnumerateAllLocalModuleModelsTransitively_DiamondDependency_VisitsSharedModuleOnlyOnce()
    {
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                module a 'a.bicep' = {
                  name: 'a'
                }
                module b 'b.bicep' = {
                  name: 'b'
                }
                """),
            ("a.bicep", """
                module shared 'shared.bicep' = {
                  name: 'shared'
                }
                """),
            ("b.bicep", """
                module shared 'shared.bicep' = {
                  name: 'shared'
                }
                """),
            ("shared.bicep", """
                output value string = 'leaf'
                """));

        var entryPoint = result.Compilation.GetEntrypointSemanticModel();
        var models = entryPoint.EnumerateAllLocalModuleModelsTransitively().ToList();

        // a + b + shared - shared must be visited exactly once, not twice.
        models.Should().HaveCount(3);
    }

    [TestMethod]
    public void EnumerateLocalModuleModels_SameModuleReferencedTwiceFromSameFile_YieldsItOnlyOnceWhenDeduped()
    {
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                module a1 'child.bicep' = {
                  name: 'a1'
                }
                module a2 'child.bicep' = {
                  name: 'a2'
                }
                """),
            ("child.bicep", """
                output value string = 'leaf'
                """));

        var entryPoint = result.Compilation.GetEntrypointSemanticModel();

        var models = entryPoint.EnumerateAllLocalModuleModelsTransitively().ToList();
        models.Should().HaveCount(1);
    }

    [TestMethod]
    public void EnumerateAllLocalModuleModelsTransitively_BrokenModuleReference_SkipsGracefullyWithoutThrowing()
    {
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                module missing 'does-not-exist.bicep' = {
                  name: 'missing'
                }
                """));

        var entryPoint = result.Compilation.GetEntrypointSemanticModel();
        var models = entryPoint.EnumerateAllLocalModuleModelsTransitively().ToList();

        // The broken reference contributes no model.
        models.Should().BeEmpty();
    }

    [TestMethod]
    public void EnumerateAllLocalModuleModelsTransitively_BrokenModuleReferenceAlongsideValidOne_KeepsValidModelAndSkipsBrokenOne()
    {
        // main references one broken module and one valid module - the broken one shouldn't
        // affect the valid sibling.
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                module missing 'does-not-exist.bicep' = {
                  name: 'missing'
                }
                module valid 'valid.bicep' = {
                  name: 'valid'
                }
                """),
            ("valid.bicep", """
                output value string = 'leaf'
                """));

        var entryPoint = result.Compilation.GetEntrypointSemanticModel();
        var models = entryPoint.EnumerateAllLocalModuleModelsTransitively().ToList();

        models.Should().HaveCount(1);
        models.Single().SourceFile.FileHandle.Uri.ToString().Should().EndWith("valid.bicep");
    }

    [TestMethod]
    public void EnumerateAllLocalModuleModelsTransitively_BrokenModuleReferenceDeepInTheChain_DoesNotAffectAncestorsOrThrow()
    {
        // main -> a (valid) -> broken. The break happens two levels down, not at the entrypoint.
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                module a 'a.bicep' = {
                  name: 'a'
                }
                """),
            ("a.bicep", """
                module missing 'does-not-exist.bicep' = {
                  name: 'missing'
                }
                """));

        var entryPoint = result.Compilation.GetEntrypointSemanticModel();
        var models = entryPoint.EnumerateAllLocalModuleModelsTransitively().ToList();

        // Only "a" - the broken grandchild contributes no model and doesn't affect "a" itself.
        models.Should().HaveCount(1);
        models.Single().SourceFile.FileHandle.Uri.ToString().Should().EndWith("a.bicep");
    }

    [TestMethod]
    public void EnumerateAllLocalModuleModelsTransitively_UnreferencedFileInCompilation_IsNotIncluded()
    {
        // "orphan.bicep" is in the compilation but unreferenced - only files reached via `module`
        // are included, not every file in the compilation.
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                module a 'a.bicep' = {
                  name: 'a'
                }
                """),
            ("a.bicep", """
                output value string = 'leaf'
                """),
            ("orphan.bicep", """
                output value string = 'unreferenced'
                """));

        var entryPoint = result.Compilation.GetEntrypointSemanticModel();
        var models = entryPoint.EnumerateAllLocalModuleModelsTransitively().ToList();

        models.Should().HaveCount(1);
        models.Select(m => m.SourceFile.FileHandle.Uri.ToString()).Should().NotContain(uri => uri.EndsWith("orphan.bicep"));
    }

    [TestMethod]
    public void EnumerateAllLocalModuleModelsTransitively_EntrypointWithNoModules_YieldsNothing()
    {
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                output value string = 'leaf'
                """));

        var entryPoint = result.Compilation.GetEntrypointSemanticModel();
        var models = entryPoint.EnumerateAllLocalModuleModelsTransitively().ToList();

        models.Should().BeEmpty();
    }

    [TestMethod]
    public void EnumerateAllLocalModuleModelsTransitively_EachYieldedModelResolvesItsOwnBicepVersionConstraint()
    {
        // main.bicep and a.bicep have different bicepconfig.json constraints (>=0.30.0 vs
        // >=0.40.0). Each model's Configuration should reflect its own file, not the entrypoint's.
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                module a 'a/a.bicep' = {
                  name: 'a'
                }
                """),
            ("bicepconfig.json", """{ "bicep": { "version": ">=0.30.0" } }"""),
            ("a/a.bicep", """
                output value string = 'leaf'
                """),
            ("a/bicepconfig.json", """{ "bicep": { "version": ">=0.40.0" } }"""));

        var entryPoint = result.Compilation.GetEntrypointSemanticModel();
        var aModel = entryPoint.EnumerateAllLocalModuleModelsTransitively().Single();

        entryPoint.Configuration.Compiler.Version!.ToString().Should().Be(">=0.30.0");
        aModel.Configuration.Compiler.Version!.ToString().Should().Be(">=0.40.0");
    }

    [TestMethod]
    public async Task EnumerateAllLocalModuleModelsTransitively_RegistryModuleReference_IsExcluded()
    {
        var fileSystem = new MockFileSystem();
        var clientFactory = await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync(
            fileSystem,
            new RegistryHelper.ModuleToPublish("br:mockregistry.io/test/published:v1", "output value string = 'leaf'"));

        var services = new ServiceBuilder().WithContainerRegistryClientFactory(clientFactory);
        var result = await CompilationHelper.RestoreAndCompile(
            services,
            ("main.bicep", """
                module fromRegistry 'br:mockregistry.io/test/published:v1' = {
                  name: 'fromRegistry'
                }
                """));

        var entryPoint = result.Compilation.GetEntrypointSemanticModel();
        var models = entryPoint.EnumerateAllLocalModuleModelsTransitively().ToList();

        models.Should().BeEmpty();
    }
}
