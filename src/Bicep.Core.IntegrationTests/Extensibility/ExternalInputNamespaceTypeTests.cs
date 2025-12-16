// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json.Serialization;
using Azure.Bicep.Types;
using Azure.Bicep.Types.Concrete;
using Azure.Bicep.Types.Index;
using Azure.Bicep.Types.Serialization;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bicep.Core.UnitTests.Assertions;
using static Bicep.Core.UnitTests.Utils.RegistryHelper;
using Bicep.Core.Registry.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Bicep.Core.IntegrationTests.Extensibility;

[TestClass]
public class ExternalInputNamespaceTypeTests
{
    //private static ServiceBuilder GetServiceBuilder(IFileSystem fileSystem, string registryHost, string repositoryPath)
    //{
    //    var clientFactory = RegistryHelper.CreateMockRegistryClient(new RepoDescriptor(registryHost, repositoryPath, ["tag"]));

    //    return new ServiceBuilder()
    //        .WithFileSystem(fileSystem)
    //        .WithContainerRegistryClientFactory(clientFactory);
    //}

    [NotNull]
    public TestContext? TestContext { get; set; }

    //private async Task<ServiceBuilder> GetServicesWithPrepublishedTypes()
    //{
    //    var typesTgz = GetExternalInputTypesTgz();
    //    var extensionTgz = await ExtensionV1Archive.Build(new(typesTgz, false, []));

    //    var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext));

    //    var tgzData = GetExternalInputTypesTgz();
    //    await RegistryHelper.PublishExtensionToRegistryAsync(services.Build(), $"br:{registry}/{repository}:1.0.0", tgzData);

    //    return services;
    //}

    public static BinaryData GetExternalInputTypesTgz()
    {
        var factory = new TypeFactory([]);

        var stringType = factory.Create(() => new StringType());
        var anyType = factory.Create(() => new AnyType());
        var scopeBindingFunctionType = factory.Create(() => new NamespaceFunctionType(
            "scopeBinding",
            "Reads scope binding from external tooling",
            "[externalInputs('binding', parameters('bindingKey'))]",
            [new FunctionParameter("kind", factory.GetReference(stringType), "The kind parameter")],
            factory.GetReference(anyType),
            NamespaceFunctionTypeFlags.ExternalInput,
            NamespaceFunctionTypeFileVisibilityFlags.Bicepparam));

        var settings = new TypeSettings(name: "ThirdPartyExtension", version: "1.0.0", isSingleton: false, configurationType: null!);

        var index = new TypeIndex(
            new Dictionary<string, CrossFileTypeReference>(),
            new Dictionary<string, IReadOnlyDictionary<string, IReadOnlyList<CrossFileTypeReference>>>(),
            new List<CrossFileTypeReference>() { new("types.json", factory.GetIndex(scopeBindingFunctionType)) },
            settings,
            null);

        return ExtensionResourceTypeHelper.GetTypesTgzBytesFromFiles(
            ("index.json", StreamHelper.GetString(stream => TypeSerializer.SerializeIndex(stream, index))),
            ("types.json", StreamHelper.GetString(stream => TypeSerializer.Serialize(stream, factory.GetTypes()))));
    }

    [TestMethod]
    public async Task ExternalInput_function_works()
    {
        var typesTgz = GetExternalInputTypesTgz();
        var extensionTgz = await ExtensionV1Archive.Build(new(typesTgz, false, []));

        var services = new ServiceBuilder();
        var result = await CompilationHelper.RestoreAndCompile(services,
            ("parameters.bicepparam", new("""
using none

extension '../extension.tgz' as ext
param foo = ext.scopeBinding('BINDING')
""")),
            ("../extension.tgz", extensionTgz));

        var model = result.Compilation.GetEntrypointSemanticModel();
        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
    }
}

//internal class NamespaceFunctionType : TypeBase
//{
//    [JsonConstructor]
//    public NamespaceFunctionType(string name, ITypeReference type, string? description) => (Name, Type, Description) = (name, type, description);

//    public string Name { get; }
//    public ITypeReference Type { get; }
//    public string? Description { get; }
//}
