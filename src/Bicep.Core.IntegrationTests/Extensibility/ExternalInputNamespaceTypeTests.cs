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
    [NotNull]
    public TestContext? TestContext { get; set; }

    public static BinaryData GetExternalInputTypesTgz()
    {
        var factory = new TypeFactory([]);

        var stringType = factory.Create(() => new StringType());
        var boolType = factory.Create(() => new BooleanType());
        var sasUriConfig = factory.Create(() => new ObjectType("sasUriConfig", new Dictionary<string, ObjectTypeProperty>
        {
            ["path"] = new(factory.GetReference(stringType), ObjectTypePropertyFlags.Required, "Path to artifact"),
            ["isDirectory"] = new(factory.GetReference(boolType), ObjectTypePropertyFlags.None, "Is a directory"),
            ["enableScopeTagBindings"] = new(factory.GetReference(boolType), ObjectTypePropertyFlags.None, "Enable binding replacements")
        }, null));

        var anyType = factory.Create(() => new AnyType());
        var scopeBindingFunctionType = factory.Create(() => new NamespaceFunctionType(
            "scopeBinding",
            "Reads scope binding from external tooling",
            "[externalInput('ev2.scopeBinding', parameters('bindingKey'))]",
            [new NamespaceFunctionParameter("bindingKey", factory.GetReference(stringType), "The binding key parameter", NamespaceFunctionParameterFlags.Required | NamespaceFunctionParameterFlags.Constant)],
            factory.GetReference(anyType),
            NamespaceFunctionFlags.ExternalInput,
            NamespaceFunctionFileVisibilityRestriction.Bicepparam));

        var configFunctionType = factory.Create(() => new NamespaceFunctionType(
            "config",
            "Reads configuration from external tooling",
            "[externalInput('ev2.expression', concat('$config(', parameters('configKey'), ')'))]",
            [new NamespaceFunctionParameter("configKey", factory.GetReference(stringType), "The configuration key parameter", NamespaceFunctionParameterFlags.Required | NamespaceFunctionParameterFlags.Constant)],
            factory.GetReference(anyType),
            NamespaceFunctionFlags.ExternalInput,
            NamespaceFunctionFileVisibilityRestriction.Bicepparam));

        var sysVariableFunctionType = factory.Create(() => new NamespaceFunctionType(
            "sysVar",
            "Reads configuration from external tooling",
            "[externalInput('ev2.expression', parameters('varExpr'))]",
            [new NamespaceFunctionParameter("varExpr", factory.GetReference(stringType), "The system variable expression", NamespaceFunctionParameterFlags.Required | NamespaceFunctionParameterFlags.Constant)],
            factory.GetReference(anyType),
            NamespaceFunctionFlags.ExternalInput,
            NamespaceFunctionFileVisibilityRestriction.Bicepparam));

        var sasUriFunctionType = factory.Create(() => new NamespaceFunctionType(
            "sasUri",
            "Reads SAS URI from external tooling",
            "[externalInput('ev2.sasUriForPath', parameters('sasUriConfig'))]",
            [new NamespaceFunctionParameter("sasUriConfig", factory.GetReference(sasUriConfig), "The SAS URI configuration properties", NamespaceFunctionParameterFlags.Required | NamespaceFunctionParameterFlags.Constant)],
            factory.GetReference(anyType),
            NamespaceFunctionFlags.ExternalInput,
            NamespaceFunctionFileVisibilityRestriction.Bicepparam));

        var settings = new TypeSettings(name: "ThirdPartyExtension", version: "1.0.0", isSingleton: false, configurationType: null!);

        var index = new TypeIndex(
            new Dictionary<string, CrossFileTypeReference>(),
            new Dictionary<string, IReadOnlyDictionary<string, IReadOnlyList<CrossFileTypeReference>>>(),
            new List<CrossFileTypeReference>() {
                new("types.json", factory.GetIndex(scopeBindingFunctionType)),
                new("types.json", factory.GetIndex(configFunctionType)),
                new("types.json", factory.GetIndex(sasUriFunctionType)),
                new("types.json", factory.GetIndex(sysVariableFunctionType)),
            },
            settings,
            null);

        return ExtensionResourceTypeHelper.GetTypesTgzBytesFromFiles(
            ("index.json", StreamHelper.GetString(stream => TypeSerializer.SerializeIndex(stream, index))),
            ("types.json", StreamHelper.GetString(stream => TypeSerializer.Serialize(stream, factory.GetTypes()))));
    }

    [TestMethod]
    public async Task ExternalInput_alias_functions_works()
    {
        var typesTgz = GetExternalInputTypesTgz();
        var extensionTgz = await ExtensionV1Archive.Build(new(typesTgz, false, []));
        await File.WriteAllBytesAsync("C:\\Users\\muriukilevi\\Repos\\playground\\bicep\\external-inputs\\extension\\extension.tgz", extensionTgz);
        var services = new ServiceBuilder();
        var result = await CompilationHelper.RestoreAndCompileParams(services,
            ("parameters.bicepparam", new("""
using none

extension '../extension.tgz' as ext
param foo = ext.scopeBinding('BINDING')
//param bar = ext.config('redis')
//param buildVer = ext.sysVar('$buildVersion()')
//param servicePackageLink = ext.sasUri({
//    path: 'bin/service.sfpkg'
//})
""")),
            ("../extension.tgz", extensionTgz));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
    }
}
