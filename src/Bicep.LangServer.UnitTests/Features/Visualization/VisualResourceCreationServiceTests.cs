// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer.Compilation;
using Bicep.LanguageServer.Features.Custom.Visualization;
using Bicep.LanguageServer.Utils;
using Bicep.Testing.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using CompilationHelper = Bicep.Core.UnitTests.Utils.CompilationHelper;

namespace Bicep.LangServer.UnitTests.Features.Visualization;

[TestClass]
public class VisualResourceCreationServiceTests
{
    private static readonly ImmutableArray<ResourceTypeComponents> CatalogFixture =
    [
        TestTypeHelper.CreateCustomResourceType("Test.Rp/alpha", "2020-01-01", TypeSymbolValidationFlags.Default),
        TestTypeHelper.CreateCustomResourceType("Test.Rp/alpha", "2021-01-01-preview", TypeSymbolValidationFlags.Default),
        TestTypeHelper.CreateCustomResourceType("Test.Rp/beta", "2020-06-01", TypeSymbolValidationFlags.Default),
        TestTypeHelper.CreateCustomResourceType("Test.Rp/gamma", "2019-01-01", TypeSymbolValidationFlags.Default),
    ];

    #region DeriveBaseSymbolicName

    [DataTestMethod]
    [DataRow("Microsoft.Storage/storageAccounts", "storageAccount")]
    [DataRow("Microsoft.Compute/virtualMachines", "virtualMachine")]
    [DataRow("Microsoft.Network/loadBalancers", "loadBalancer")]
    [DataRow("Test.Rp/basicTests", "basicTest")]
    [DataRow("Test.Rp/9-invalid", "invalid")]
    [DataRow("Test.Rp/123456", "resource")]
    [DataRow("Test.Rp/categories", "category")]
    public void DeriveBaseSymbolicName_ReturnsDeterministicValidIdentifier(string fullyQualifiedType, string expected)
    {
        var typeReference = new ResourceTypeReference(fullyQualifiedType, "2020-01-01");

        VisualResourceCreationService.DeriveBaseSymbolicName(typeReference).Should().Be(expected);
    }

    #endregion

    #region GenerateSymbolicName

    [TestMethod]
    public void GenerateSymbolicName_NoCollision_ReturnsBaseName()
    {
        var model = CreateModel(BuiltInTestTypes.Types, string.Empty);

        var symbolicName = VisualResourceCreationService.GenerateSymbolicName(
            new ResourceTypeReference("Microsoft.Storage/storageAccounts", "2020-01-01"), model);

        symbolicName.Should().Be("storageAccount");
    }

    [TestMethod]
    public void GenerateSymbolicName_WithExistingNumberedCollisions_UsesNextAvailableSuffix()
    {
        var content = """
            resource storageAccount 'Test.Rp/basicTests@2020-01-01' = {
              name: 'sa0'
            }
            resource storageAccount1 'Test.Rp/basicTests@2020-01-01' = {
              name: 'sa1'
            }
            """;

        var model = CreateModel(BuiltInTestTypes.Types, content, expectNoDiagnostics: true);

        var symbolicName = VisualResourceCreationService.GenerateSymbolicName(
            new ResourceTypeReference("Microsoft.Storage/storageAccounts", "2020-01-01"), model);

        symbolicName.Should().Be("storageAccount2");
    }

    [TestMethod]
    public void GenerateSymbolicName_CollisionDiffersOnlyByCase_IsStillTreatedAsACollision()
    {
        var content = """
            resource StorageAccount 'Test.Rp/basicTests@2020-01-01' = {
              name: 'sa'
            }
            """;

        var model = CreateModel(BuiltInTestTypes.Types, content, expectNoDiagnostics: true);

        var symbolicName = VisualResourceCreationService.GenerateSymbolicName(
            new ResourceTypeReference("Microsoft.Storage/storageAccounts", "2020-01-01"), model);

        symbolicName.Should().Be("storageAccount1");
    }

    #endregion

    #region GenerateBody

    [TestMethod]
    public void GenerateBody_RequiredStringLiteralProperty_IsIncludedAsALiteral()
    {
        var body = TestTypeHelper.CreateObjectType(
            "Body",
            ("location", TypeFactory.CreateStringLiteralType("global"), TypePropertyFlags.Required));

        var (properties, unresolved) = VisualResourceCreationService.GenerateBody(CreateResourceType(body));

        unresolved.Should().BeEmpty();
        var property = properties.Should().ContainSingle().Subject;
        property.TryGetKeyText().Should().Be("location");
        ((StringSyntax)property.Value).TryGetLiteralValue().Should().Be("global");
    }

    [TestMethod]
    public void GenerateBody_RequiredNonLiteralProperty_IsReportedAsUnresolved()
    {
        var body = TestTypeHelper.CreateObjectType(
            "Body",
            ("name", LanguageConstants.String, TypePropertyFlags.Required));

        var (properties, unresolved) = VisualResourceCreationService.GenerateBody(CreateResourceType(body));

        properties.Should().BeEmpty();
        unresolved.Should().Equal("name");
    }

    [TestMethod]
    public void GenerateBody_OptionalProperty_IsExcludedFromBodyAndUnresolvedList()
    {
        var body = TestTypeHelper.CreateObjectType(
            "Body",
            ("description", LanguageConstants.String, TypePropertyFlags.None));

        var (properties, unresolved) = VisualResourceCreationService.GenerateBody(CreateResourceType(body));

        properties.Should().BeEmpty();
        unresolved.Should().BeEmpty();
    }

    [TestMethod]
    public void GenerateBody_RequiredButNullableProperty_IsExcludedFromBodyAndUnresolvedList()
    {
        var body = TestTypeHelper.CreateObjectType(
            "Body",
            ("name", TypeHelper.MakeNullable(LanguageConstants.String), TypePropertyFlags.Required));

        var (properties, unresolved) = VisualResourceCreationService.GenerateBody(CreateResourceType(body));

        properties.Should().BeEmpty();
        unresolved.Should().BeEmpty();
    }

    [TestMethod]
    public void GenerateBody_MixedProperties_OnlyConsidersRequiredNonNullableProperties()
    {
        var body = TestTypeHelper.CreateObjectType(
            "Body",
            ("location", TypeFactory.CreateStringLiteralType("global"), TypePropertyFlags.Required),
            ("name", LanguageConstants.String, TypePropertyFlags.Required),
            ("description", LanguageConstants.String, TypePropertyFlags.None));

        var (properties, unresolved) = VisualResourceCreationService.GenerateBody(CreateResourceType(body));

        var property = properties.Should().ContainSingle().Subject;
        property.TryGetKeyText().Should().Be("location");
        unresolved.Should().Equal("name");
    }

    [TestMethod]
    public void GenerateBody_DiscriminatedObjectType_OnlyDiscriminatorKeyIsUnresolved()
    {
        var memberA = TestTypeHelper.CreateObjectType(
            "MemberA",
            ("kind", TypeFactory.CreateStringLiteralType("a"), TypePropertyFlags.Required),
            ("settingA", LanguageConstants.String, TypePropertyFlags.Required));
        var memberB = TestTypeHelper.CreateObjectType(
            "MemberB",
            ("kind", TypeFactory.CreateStringLiteralType("b"), TypePropertyFlags.Required),
            ("settingB", LanguageConstants.String, TypePropertyFlags.Required));

        var body = TestTypeHelper.CreateDiscriminatedObjectType("Body", "kind", memberA, memberB);

        var (properties, unresolved) = VisualResourceCreationService.GenerateBody(CreateResourceType(body));

        properties.Should().BeEmpty();
        unresolved.Should().Equal("kind");
    }

    #endregion

    #region GetResourceTypes

    [TestMethod]
    public void GetResourceTypes_OrdersByTypeAscendingThenApiVersionDescending()
    {
        var model = CreateModel(CatalogFixture, string.Empty);
        var service = new VisualResourceCreationService();

        var result = service.GetResourceTypes(model, query: null, includePreview: true, pageSize: 50, continuationToken: null);

        result.Items.Select(entry => (entry.FullyQualifiedType, entry.ApiVersion)).Should().Equal(
            ("Test.Rp/alpha", "2021-01-01-preview"),
            ("Test.Rp/alpha", "2020-01-01"),
            ("Test.Rp/beta", "2020-06-01"),
            ("Test.Rp/gamma", "2019-01-01"));
        result.Items.Single(entry => entry.ApiVersion == "2021-01-01-preview").IsPreview.Should().BeTrue();
        result.Items.Where(entry => entry.ApiVersion != "2021-01-01-preview").Should().OnlyContain(entry => !entry.IsPreview);
        result.ContinuationToken.Should().BeNull();
    }

    [TestMethod]
    public void GetResourceTypes_IncludePreviewFalse_ExcludesPreviewApiVersions()
    {
        var model = CreateModel(CatalogFixture, string.Empty);
        var service = new VisualResourceCreationService();

        var result = service.GetResourceTypes(model, query: null, includePreview: false, pageSize: 50, continuationToken: null);

        result.Items.Should().HaveCount(3);
        result.Items.Should().NotContain(entry => entry.IsPreview);
    }

    [TestMethod]
    public void GetResourceTypes_QueryFilter_MatchesSubstringCaseInsensitively()
    {
        var model = CreateModel(CatalogFixture, string.Empty);
        var service = new VisualResourceCreationService();

        var result = service.GetResourceTypes(model, query: "ALPHA", includePreview: true, pageSize: 50, continuationToken: null);

        result.Items.Should().HaveCount(2);
        result.Items.Should().OnlyContain(entry => entry.FullyQualifiedType == "Test.Rp/alpha");
    }

    [TestMethod]
    public void GetResourceTypes_PagesResults_UntilContinuationTokenIsExhausted()
    {
        var model = CreateModel(CatalogFixture, string.Empty);
        var service = new VisualResourceCreationService();

        var firstPage = service.GetResourceTypes(model, query: null, includePreview: true, pageSize: 2, continuationToken: null);
        firstPage.Items.Should().HaveCount(2);
        firstPage.ContinuationToken.Should().Be("2");

        var secondPage = service.GetResourceTypes(model, query: null, includePreview: true, pageSize: 2, continuationToken: firstPage.ContinuationToken);
        secondPage.Items.Should().HaveCount(2);
        secondPage.ContinuationToken.Should().BeNull();

        firstPage.Items.Concat(secondPage.Items)
            .Select(entry => (entry.FullyQualifiedType, entry.ApiVersion))
            .Should().Equal(
                ("Test.Rp/alpha", "2021-01-01-preview"),
                ("Test.Rp/alpha", "2020-01-01"),
                ("Test.Rp/beta", "2020-06-01"),
                ("Test.Rp/gamma", "2019-01-01"));
    }

    [TestMethod]
    public void GetResourceTypes_NonPositivePageSize_FallsBackToDefaultPageSize()
    {
        var model = CreateModel(CatalogFixture, string.Empty);
        var service = new VisualResourceCreationService();

        var result = service.GetResourceTypes(model, query: null, includePreview: true, pageSize: 0, continuationToken: null);

        result.Items.Should().HaveCount(4);
        result.ContinuationToken.Should().BeNull();
    }

    [TestMethod]
    public void GetResourceTypes_PageSizeAboveMax_IsClampedRatherThanRejected()
    {
        var model = CreateModel(CatalogFixture, string.Empty);
        var service = new VisualResourceCreationService();

        var result = service.GetResourceTypes(model, query: null, includePreview: true, pageSize: 10_000, continuationToken: null);

        result.Items.Should().HaveCount(4);
        result.ContinuationToken.Should().BeNull();
    }

    #endregion

    #region PrepareResource

    [TestMethod]
    public void PrepareResource_HappyPath_GeneratesValidTopLevelDeclarationAndVersionedEdit()
    {
        var (compiler, compilationResult) = CompileWithResourceTypes(BuiltInTestTypes.Types, string.Empty);
        var context = new CompilationContext(compilationResult.Compilation);
        var service = new VisualResourceCreationService();

        var request = new PrepareVisualResourceParams(
            new VersionedTextDocumentIdentifier { Uri = DocumentUri.From("main.bicep"), Version = 7 },
            "operation-1",
            new VisualResourceTypeIdentifier("Test.Rp/basicTests", "2020-01-01"));

        var response = service.PrepareResource(compiler, context, request);

        response.OperationId.Should().Be("operation-1");
        response.SymbolicName.Should().Be("basicTest");
        response.ExpectedNodeId.Should().Be("basicTest");
        response.UnresolvedRequiredProperties.Should().Equal("name");

        var textDocumentEdit = response.Edit.DocumentChanges.Should().ContainSingle().Subject.TextDocumentEdit;
        textDocumentEdit.Should().NotBeNull();
        textDocumentEdit!.TextDocument.Uri.Should().Be(request.TextDocument.Uri);
        textDocumentEdit.TextDocument.Version.Should().Be(request.TextDocument.Version);

        var updatedContent = ApplyEdit(string.Empty, context.LineStarts, response.Edit);
        updatedContent.Should().Contain("resource basicTest 'Test.Rp/basicTests@2020-01-01' = {");

        // The generated declaration is syntactically valid Bicep, but - per design - is not required to be
        // semantically complete: "name" is required and non-literal, so it is reported as unresolved rather
        // than invented, and the applied edit is expected to leave the normal, authoritative compiler
        // diagnostic for that missing property in place.
        var (_, updatedResult) = CompileWithResourceTypes(BuiltInTestTypes.Types, updatedContent);
        updatedResult.Should().OnlyContainDiagnostic(
            "BCP035",
            DiagnosticLevel.Error,
            "The specified \"resource\" declaration is missing the following required properties: \"name\".");
    }

    [TestMethod]
    public void PrepareResource_SymbolicNameCollision_GeneratesUniqueSuffixedName()
    {
        var content = """
            resource basicTest 'Test.Rp/basicTests@2020-01-01' = {
              name: 'existing'
            }
            """;

        var (compiler, compilationResult) = CompileWithResourceTypes(BuiltInTestTypes.Types, content);
        compilationResult.Should().NotHaveAnyDiagnostics();
        var context = new CompilationContext(compilationResult.Compilation);
        var service = new VisualResourceCreationService();

        var request = new PrepareVisualResourceParams(
            new VersionedTextDocumentIdentifier { Uri = DocumentUri.From("main.bicep"), Version = 1 },
            "operation-2",
            new VisualResourceTypeIdentifier("Test.Rp/basicTests", "2020-01-01"));

        var response = service.PrepareResource(compiler, context, request);

        response.SymbolicName.Should().Be("basicTest1");

        var updatedContent = ApplyEdit(content, context.LineStarts, response.Edit);
        var (_, updatedResult) = CompileWithResourceTypes(BuiltInTestTypes.Types, updatedContent);
        updatedResult.Should().OnlyContainDiagnostic(
            "BCP035",
            DiagnosticLevel.Error,
            "The specified \"resource\" declaration is missing the following required properties: \"name\".");
    }

    [TestMethod]
    public void PrepareResource_WithExistingResources_InsertsAfterLastResourceAndBeforeOutput()
    {
        var content = """
            resource first 'Test.Rp/basicTests@2020-01-01' = {
              name: 'first'
            }

            resource second 'Test.Rp/basicTests@2020-01-01' = {
              name: 'second'
            }

            output result string = second.name
            """;

        var updatedContent = PrepareAndApply(content);

        updatedContent.IndexOf("resource second", StringComparison.Ordinal)
            .Should().BeLessThan(updatedContent.IndexOf("resource basicTest", StringComparison.Ordinal));
        updatedContent.IndexOf("resource basicTest", StringComparison.Ordinal)
            .Should().BeLessThan(updatedContent.IndexOf("output result", StringComparison.Ordinal));
        updatedContent.ReplaceLineEndings("\n").Should().Contain("}\n\nresource basicTest");
    }

    [TestMethod]
    public void PrepareResource_WithoutExistingResource_InsertsAfterParametersAndVariablesAndBeforeOutputs()
    {
        var content = """
            param prefix string
            var resourceName = '${prefix}-resource'

            output result string = resourceName
            """;

        var updatedContent = PrepareAndApply(content);

        updatedContent.IndexOf("var resourceName", StringComparison.Ordinal)
            .Should().BeLessThan(updatedContent.IndexOf("resource basicTest", StringComparison.Ordinal));
        updatedContent.IndexOf("resource basicTest", StringComparison.Ordinal)
            .Should().BeLessThan(updatedContent.IndexOf("output result", StringComparison.Ordinal));
    }

    [TestMethod]
    public void PrepareResource_WithOnlyOutputs_InsertsBeforeFirstOutput()
    {
        var content = "output result string = 'value'";

        var updatedContent = PrepareAndApply(content);

        updatedContent.Should().StartWith("resource basicTest");
        updatedContent.IndexOf("resource basicTest", StringComparison.Ordinal)
            .Should().BeLessThan(updatedContent.IndexOf("output result", StringComparison.Ordinal));
    }

    [TestMethod]
    public void PrepareResource_ReadWriteType_ReportsUnresolvedRequiredProperties()
    {
        var (compiler, compilationResult) = CompileWithResourceTypes(BuiltInTestTypes.Types, string.Empty);
        var context = new CompilationContext(compilationResult.Compilation);
        var service = new VisualResourceCreationService();

        var request = new PrepareVisualResourceParams(
            new VersionedTextDocumentIdentifier { Uri = DocumentUri.From("main.bicep"), Version = 1 },
            "operation-3",
            new VisualResourceTypeIdentifier("Test.Rp/readWriteTests", "2020-01-01"));

        var response = service.PrepareResource(compiler, context, request);

        response.UnresolvedRequiredProperties.Should().Equal("name", "properties");
    }

    [TestMethod]
    public void PrepareResource_DiscriminatedType_ReportsDiscriminatorKeyAsUnresolved()
    {
        var (compiler, compilationResult) = CompileWithResourceTypes(BuiltInTestTypes.Types, string.Empty);
        var context = new CompilationContext(compilationResult.Compilation);
        var service = new VisualResourceCreationService();

        var request = new PrepareVisualResourceParams(
            new VersionedTextDocumentIdentifier { Uri = DocumentUri.From("main.bicep"), Version = 1 },
            "operation-4",
            new VisualResourceTypeIdentifier("Test.Rp/discriminatorTests", "2020-01-01"));

        var response = service.PrepareResource(compiler, context, request);

        response.UnresolvedRequiredProperties.Should().Equal("kind");
    }

    [TestMethod]
    public void PrepareResource_UnknownResourceType_ThrowsVisualResourceCreationException()
    {
        var (compiler, compilationResult) = CompileWithResourceTypes(BuiltInTestTypes.Types, string.Empty);
        var context = new CompilationContext(compilationResult.Compilation);
        var service = new VisualResourceCreationService();

        var request = new PrepareVisualResourceParams(
            new VersionedTextDocumentIdentifier { Uri = DocumentUri.From("main.bicep"), Version = 1 },
            "operation-5",
            new VisualResourceTypeIdentifier("Test.Rp/doesNotExist", "2020-01-01"));

        Action act = () => service.PrepareResource(compiler, context, request);

        act.Should().Throw<VisualResourceCreationException>()
            .WithMessage("Resource type \"Test.Rp/doesNotExist@2020-01-01\" was not found.");
    }

    #endregion

    private static ResourceType CreateResourceType(ITypeReference body) =>
        new(
            TestTypeHelper.GetBuiltInNamespaceType("az"),
            new ResourceTypeReference("Test.Rp/widgets", "2020-01-01"),
            ResourceScope.ResourceGroup,
            ResourceScope.None,
            ResourceFlags.None,
            body,
            []);

    private static SemanticModel CreateModel(IEnumerable<ResourceTypeComponents> resourceTypes, string content, bool expectNoDiagnostics = false)
    {
        var services = new ServiceBuilder().WithAzResources(resourceTypes);
        var result = CompilationHelper.Compile(services, content);

        if (expectNoDiagnostics)
        {
            result.Should().NotHaveAnyDiagnostics();
        }

        return result.Compilation.GetEntrypointSemanticModel();
    }

    // Mirrors CompilationHelper.Compile's internal implementation, but also returns the BicepCompiler used to
    // build the compilation. PrepareResource needs a compiler built from the exact same registrations as the
    // supplied CompilationContext so its internal self-validation recompile of the generated resource
    // declaration succeeds.
    private static (BicepCompiler Compiler, CompilationHelper.CompilationResult Result) CompileWithResourceTypes(
        IEnumerable<ResourceTypeComponents> resourceTypes, string content)
    {
        var fileSet = new MockFileSystemTestFileSet();
        fileSet.AddFile("main.bicep", content);

        var compiler = new ServiceBuilder()
            .WithAzResources(resourceTypes)
            .WithFileExplorer(fileSet.FileExplorer)
            .WithFileSystem(fileSet.FileSystem)
            .Build()
            .GetCompiler();

        var compilation = compiler.CreateCompilationWithoutRestore(fileSet.GetUri("main.bicep"));
        return (compiler, CompilationHelper.GetCompilationResult(compilation));
    }

    private static string PrepareAndApply(string content)
    {
        var (compiler, compilationResult) = CompileWithResourceTypes(BuiltInTestTypes.Types, content);
        var context = new CompilationContext(compilationResult.Compilation);
        var service = new VisualResourceCreationService();
        var request = new PrepareVisualResourceParams(
            new VersionedTextDocumentIdentifier { Uri = DocumentUri.From("main.bicep"), Version = 1 },
            "operation",
            new VisualResourceTypeIdentifier("Test.Rp/basicTests", "2020-01-01"));

        var response = service.PrepareResource(compiler, context, request);
        return ApplyEdit(content, context.LineStarts, response.Edit);
    }

    // The generated code replacement is always a zero-length insertion, so applying it is a plain string insertion.
    private static string ApplyEdit(string content, ImmutableArray<int> lineStarts, WorkspaceEdit edit)
    {
        var textDocumentEdit = edit.DocumentChanges!.Single().TextDocumentEdit!;
        var textEdit = textDocumentEdit.Edits.Single();
        var offset = PositionHelper.GetOffset(lineStarts, textEdit.Range.Start);

        return content.Insert(offset, textEdit.NewText);
    }
}
