// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Modules;
using Bicep.Core.Navigation;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.SourceGraph;

[TestClass]
public class IArtifactFileLookupExtensionsTests
{
    [TestMethod]
    public void Symbols_return_the_artifact_reference_resolved_by_the_source_file_grouping()
    {
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                import { foo } from 'types.bicep'
                import * as types from 'types.bicep'

                module mod 'mod.bicep' = {}

                output fooOutput foo = 'a'
                output typesOutput types.foo = 'b'
                """),
            ("types.bicep", """
                @export()
                type foo = string
                """),
            ("mod.bicep", ""));

        result.Should().NotHaveAnyDiagnostics();

        var grouping = result.Compilation.SourceFileGrouping;
        var fileSymbol = result.Compilation.GetEntrypointSemanticModel().Root;

        var importedSymbol = fileSymbol.ImportedSymbols.Should().ContainSingle().Subject;
        importedSymbol.TryGetArtifactReference().Should().BeOfType<LocalModuleReference>()
            .And.BeSameAs(grouping.ArtifactLookup[importedSymbol.EnclosingDeclaration].Reference);

        var wildcardImport = fileSymbol.WildcardImports.Should().ContainSingle().Subject;
        wildcardImport.TryGetArtifactReference().Should().BeOfType<LocalModuleReference>()
            .And.BeSameAs(grouping.ArtifactLookup[wildcardImport.EnclosingDeclaration].Reference);

        var module = fileSymbol.ModuleDeclarations.Should().ContainSingle().Subject;
        grouping.TryGetArtifactReference(module.DeclaringModule).Should().BeOfType<LocalModuleReference>()
            .And.BeSameAs(grouping.ArtifactLookup[module.DeclaringModule].Reference);
    }

    [TestMethod]
    public void TryGetArtifactReference_returns_null_for_an_invalid_reference()
    {
        var result = CompilationHelper.Compile("""
            module mod 'br:' = {}
            """);

        var grouping = result.Compilation.SourceFileGrouping;
        var module = result.Compilation.GetEntrypointSemanticModel().Root.ModuleDeclarations.Should().ContainSingle().Subject;

        grouping.ArtifactLookup.Should().ContainKey(module.DeclaringModule);
        grouping.TryGetArtifactReference(module.DeclaringModule).Should().BeNull();
    }

    [TestMethod]
    public void TryGetArtifactReference_returns_null_for_syntax_that_was_not_resolved()
    {
        var result = CompilationHelper.Compile("");
        var otherResult = CompilationHelper.Compile(("main.bicep", "module mod 'mod.bicep' = {}"), ("mod.bicep", ""));
        var otherModule = otherResult.BicepFile.ProgramSyntax.Declarations.OfType<ModuleDeclarationSyntax>().Single();

        result.Compilation.SourceFileGrouping.TryGetArtifactReference(otherModule).Should().BeNull();
    }
}
