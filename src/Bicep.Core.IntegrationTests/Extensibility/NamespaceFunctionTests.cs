// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Bicep.Types;
using Azure.Bicep.Types.Concrete;
using Azure.Bicep.Types.Index;
using Azure.Bicep.Types.Serialization;
using Bicep.Core.Diagnostics;
using Bicep.Core.Registry.Extensions;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests.Extensibility;

[TestClass]
public class NamespaceFunctionTests
{
    [TestMethod]
    public async Task Function_visiblity_is_validated()
    {
        var typesTgz = GetTypesTgz();
        var extensionTgz = await ExtensionV1Archive.Build(new(typesTgz, false, []));
        var services = new ServiceBuilder();

        var result = await CompilationHelper.RestoreAndCompile(services,
            ("main.bicep", new("""
extension '../extension.tgz' as ext
var foo = ext.config('redis')
""")),
            ("../extension.tgz", extensionTgz));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP107", DiagnosticLevel.Error, "The function \"config\" does not exist in namespace \"ext\"."),
        });
    }

    [TestMethod]
    public async Task Function_parameter_flags_are_validated()
    {
        var typesTgz = GetTypesTgz();
        var extensionTgz = await ExtensionV1Archive.Build(new(typesTgz, false, []));
        var services = new ServiceBuilder();
        var result = await CompilationHelper.RestoreAndCompile(services,
            ("main.bicep", new("""
extension '../extension.tgz' as ext
var foo = ext.foo()
""")),
            ("../extension.tgz", extensionTgz));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP071", DiagnosticLevel.Error, "Expected 1 argument, but got 0."),
        });
    }

    [TestMethod]
    public async Task Function_parameter_types_are_validated()
    {
        var typesTgz = GetTypesTgz();
        var extensionTgz = await ExtensionV1Archive.Build(new(typesTgz, false, []));
        var services = new ServiceBuilder();
        var result = await CompilationHelper.RestoreAndCompileParams(services,
            ("parameters.bicepparam", new("""
using none

extension '../extension.tgz' as ext
param servicePackageLink = ext.sasUri({
    pth: 'bin/service.sfpkg' // typo here
    isDirectory: 'false'
})
""")),
            ("../extension.tgz", extensionTgz));

        result.ExcludingLinterDiagnostics().Should().ContainDiagnostic("BCP089", DiagnosticLevel.Error, "The property \"pth\" is not allowed on objects of type \"myConfig\". Did you mean \"path\"?");
        result.ExcludingLinterDiagnostics().Should().ContainDiagnostic("BCP036", DiagnosticLevel.Error, "The property \"isDirectory\" expected a value of type \"bool | null\" but the provided value is of type \"'false'\".");
    }

    [TestMethod]
    public async Task Function_with_specified_evaluated_language_expression_compiles()
    {
        var typesTgz = GetTypesTgz();
        var extensionTgz = await ExtensionV1Archive.Build(new(typesTgz, false, []));
        var services = new ServiceBuilder();

        var result = await CompilationHelper.RestoreAndCompileParams(services,
            ("parameters.bicepparam", new("""
using none

extension '../extension.tgz' as ext
param bar = ext.config('redis')
param servicePackageLink = ext.sasUri({
    path: 'bin/service.sfpkg'
    isDirectory: false
    enableReplacements: true
})
""")),
            ("../extension.tgz", extensionTgz));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        var parameters = TemplateHelper.ConvertAndAssertParameters(result.Parameters);
        parameters["bar"].Expression.Should().DeepEqual("""[externalInputs('sys_expression_0')]""");
        parameters["servicePackageLink"].Expression.Should().DeepEqual("""[externalInputs('sys_sasUriForPath_1')]""");

        var externalInputs = TemplateHelper.ConvertAndAssertExternalInputs(result.Parameters);
        externalInputs["sys_expression_0"].Should().DeepEqual(new JObject
        {
            ["kind"] = "sys.expression",
            ["config"] = "$config(redis)"
        });
        externalInputs["sys_sasUriForPath_1"].Should().DeepEqual(new JObject
        {
            ["kind"] = "sys.sasUriForPath",
            ["config"] = new JObject
            {
                ["path"] = "bin/service.sfpkg",
                ["isDirectory"] = false,
                ["enableReplacements"] = true
            }
        });
    }

    private static BinaryData GetTypesTgz()
    {
        var factory = new TypeFactory([]);

        var stringType = factory.Create(() => new StringType());
        var boolType = factory.Create(() => new BooleanType());

        var anyType = factory.Create(() => new AnyType());

        var simpleFunctionType = factory.Create(() => new NamespaceFunctionType(
            "foo",
            null,
            null,
            [new NamespaceFunctionParameter("myParam", factory.GetReference(stringType), null, NamespaceFunctionParameterFlags.Required)],
            factory.GetReference(anyType),
            BicepSourceFileKind.BicepFile));

        var configFunctionType = factory.Create(() => new NamespaceFunctionType(
            "config",
            "Requests configuration from external tooling",
            "[externalInput('sys.expression', concat('$config(', parameters('configKey'), ')'))]",
            [new NamespaceFunctionParameter("configKey", factory.GetReference(stringType), "The configuration key parameter", NamespaceFunctionParameterFlags.Required | NamespaceFunctionParameterFlags.CompileTimeConstant)],
            factory.GetReference(anyType),
            BicepSourceFileKind.ParamsFile));

        var sasUriConfig = factory.Create(() => new ObjectType("myConfig", new Dictionary<string, ObjectTypeProperty>
        {
            ["path"] = new(factory.GetReference(stringType), ObjectTypePropertyFlags.Required, "Path to artifact"),
            ["isDirectory"] = new(factory.GetReference(boolType), ObjectTypePropertyFlags.None, "Is a directory"),
            ["enableReplacements"] = new(factory.GetReference(boolType), ObjectTypePropertyFlags.None, "Enable replacements")
        }, null));
        var sasUriFunctionType = factory.Create(() => new NamespaceFunctionType(
            "sasUri",
            "Requests SAS URI from external tooling",
            "[externalInput('sys.sasUriForPath', parameters('sasUriConfig'))]",
            [new NamespaceFunctionParameter("sasUriConfig", factory.GetReference(sasUriConfig), "The SAS URI configuration properties", NamespaceFunctionParameterFlags.Required | NamespaceFunctionParameterFlags.CompileTimeConstant)],
            factory.GetReference(anyType),
            BicepSourceFileKind.ParamsFile));

        var settings = new TypeSettings(name: "ThirdPartyExtension", version: "1.0.0", isSingleton: false, configurationType: null!);

        var index = new TypeIndex(
            new Dictionary<string, CrossFileTypeReference>(),
            new Dictionary<string, IReadOnlyDictionary<string, IReadOnlyList<CrossFileTypeReference>>>(),
            new List<CrossFileTypeReference>() {
                new("types.json", factory.GetIndex(simpleFunctionType)),
                new("types.json", factory.GetIndex(configFunctionType)),
                new("types.json", factory.GetIndex(sasUriFunctionType)),
            },
            settings,
            null);

        return ExtensionResourceTypeHelper.GetTypesTgzBytesFromFiles(
            ("index.json", StreamHelper.GetString(stream => TypeSerializer.SerializeIndex(stream, index))),
            ("types.json", StreamHelper.GetString(stream => TypeSerializer.Serialize(stream, factory.GetTypes()))));
    }
}
