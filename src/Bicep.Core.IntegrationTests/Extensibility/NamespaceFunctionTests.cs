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
        var extensionTgz = await GetExtensionTgz((factory, types) =>
        {
            var configFunctionType = factory.Create(() => new NamespaceFunctionType(
                "config",
                "Requests configuration from external tooling",
                "[externalInput('sys.expression', concat('$config(', parameters('configKey'), ')'))]",
                [new NamespaceFunctionParameter("configKey", factory.GetReference(types.StringType), "The configuration key parameter", NamespaceFunctionParameterFlags.Required | NamespaceFunctionParameterFlags.CompileTimeConstant)],
                factory.GetReference(types.AnyType),
                BicepSourceFileKind.ParamsFile));

            return [configFunctionType];
        });
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
        var extensionTgz = await GetExtensionTgz((factory, types) =>
        {
            var simpleFunctionType = factory.Create(() => new NamespaceFunctionType(
                "foo",
                null,
                null,
                [new NamespaceFunctionParameter("myParam", factory.GetReference(types.StringType), null, NamespaceFunctionParameterFlags.Required)],
                factory.GetReference(types.StringType),
                null));

            return [simpleFunctionType];
        });
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
        var extensionTgz = await GetExtensionTgz((factory, types) =>
        {
            var sasUriConfigType = factory.Create(() => new ObjectType("myConfig", new Dictionary<string, ObjectTypeProperty>
            {
                ["path"] = new(factory.GetReference(types.StringType), ObjectTypePropertyFlags.Required, "Path to artifact"),
                ["isDirectory"] = new(factory.GetReference(types.BoolType), ObjectTypePropertyFlags.None, "Is a directory"),
                ["enableReplacements"] = new(factory.GetReference(types.BoolType), ObjectTypePropertyFlags.None, "Enable replacements")
            }, null));

            var sasUriFunctionType = factory.Create(() => new NamespaceFunctionType(
                "sasUri",
                "Requests SAS URI from external tooling",
                "[externalInput('sys.sasUriForPath', parameters('sasUriConfig'))]",
                [new NamespaceFunctionParameter("sasUriConfig", factory.GetReference(sasUriConfigType), "The SAS URI configuration properties", NamespaceFunctionParameterFlags.Required | NamespaceFunctionParameterFlags.CompileTimeConstant)],
                factory.GetReference(types.AnyType),
                BicepSourceFileKind.ParamsFile));

            return [sasUriFunctionType];
        });
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
    public async Task Function_evaluation_failure_when_collecting_external_inputs_returns_diagnostic()
    {
        var extensionTgz = await GetExtensionTgz((factory, types) =>
        {
            var fooFunctionType = factory.Create(() => new NamespaceFunctionType(
                "foo",
                null,
                "[externalInput('foo', unknown())]",
                [],
                factory.GetReference(types.StringType),
                BicepSourceFileKind.ParamsFile));

            return [fooFunctionType];
        });
        var services = new ServiceBuilder();
        var result = await CompilationHelper.RestoreAndCompileParams(services,
            ("parameters.bicepparam", new("""
                using none
                extension '../extension.tgz' as ext
                param myParam = ext.foo()
                """)),
            ("../extension.tgz", extensionTgz));

        result.Should().ContainDiagnostic("BCP338", DiagnosticLevel.Error, "Failed to evaluate function \"ext.foo()\": The template function 'unknown' is not valid. Please see https://aka.ms/arm-functions for usage details.");
    }

    [TestMethod]
    public async Task Function_with_single_external_input_request_compiles()
    {
        var extensionTgz = await GetExtensionTgz((factory, types) =>
        {
            var configFunctionType = factory.Create(() => new NamespaceFunctionType(
                "config",
                "Requests configuration from external tooling",
                "[externalInput('sys.expression', concat('$config(', parameters('configKey'), ')'))]",
                [new NamespaceFunctionParameter("configKey", factory.GetReference(types.StringType), "The configuration key parameter", NamespaceFunctionParameterFlags.Required | NamespaceFunctionParameterFlags.CompileTimeConstant)],
                factory.GetReference(types.AnyType),
                BicepSourceFileKind.ParamsFile));

            var sasUriConfigType = factory.Create(() => new ObjectType("myConfig", new Dictionary<string, ObjectTypeProperty>
            {
                ["path"] = new(factory.GetReference(types.StringType), ObjectTypePropertyFlags.Required, "Path to artifact"),
                ["isDirectory"] = new(factory.GetReference(types.BoolType), ObjectTypePropertyFlags.None, "Is a directory"),
                ["enableReplacements"] = new(factory.GetReference(types.BoolType), ObjectTypePropertyFlags.None, "Enable replacements")
            }, null));

            var sasUriFunctionType = factory.Create(() => new NamespaceFunctionType(
                "sasUri",
                "Requests SAS URI from external tooling",
                "[externalInput('sys.sasUriForPath', parameters('sasUriConfig'))]",
                [new NamespaceFunctionParameter("sasUriConfig", factory.GetReference(sasUriConfigType), "The SAS URI configuration properties", NamespaceFunctionParameterFlags.Required | NamespaceFunctionParameterFlags.CompileTimeConstant)],
                factory.GetReference(types.AnyType),
                BicepSourceFileKind.ParamsFile));

            return [configFunctionType, sasUriFunctionType];
        });
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
        parameters["bar"].Expression.Should().DeepEqual("[externalInputs('sys_expression_0')]");
        parameters["servicePackageLink"].Expression.Should().DeepEqual("[externalInputs('sys_sasUriForPath_1')]");

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

    [TestMethod]
    public async Task Function_with_multiple_external_input_request_compiles()
    {
        var extensionTgz = await GetExtensionTgz((factory, types) =>
        {
            var objectType = factory.Create(() => new ObjectType("myConfig", new Dictionary<string, ObjectTypeProperty>
            {
                ["path"] = new(factory.GetReference(types.StringType), ObjectTypePropertyFlags.Required, "Path to artifact"),
            }, null));

            var concatConfigFunctionType = factory.Create(() => new NamespaceFunctionType(
                "concatConfig",
                "Requests configuration from external tooling",
                "[concat(externalInput('sys.expression', parameters('config1')), '-combined-with-', externalInput('sys.expression', parameters('config2')), '-and-with-', externalInput('sys.expression', parameters('config3')))]",
                [
                    new NamespaceFunctionParameter("config1", factory.GetReference(types.StringType), "The first configuration key parameter", NamespaceFunctionParameterFlags.Required),
                    new NamespaceFunctionParameter("config2", factory.GetReference(types.IntType), "The second configuration key parameter", NamespaceFunctionParameterFlags.Required),
                    new NamespaceFunctionParameter("config3", factory.GetReference(objectType), "The third configuration key parameter", NamespaceFunctionParameterFlags.Required),
                ],
                factory.GetReference(types.AnyType),
                BicepSourceFileKind.ParamsFile));

            return [concatConfigFunctionType];
        });
        var services = new ServiceBuilder();
        var result = await CompilationHelper.RestoreAndCompileParams(services,
            ("parameters.bicepparam", new("""
                using none

                extension '../extension.tgz' as ext
                var lifeMeaning = 42
                param baz = ext.concatConfig(substring('hello world', 0, 5), lifeMeaning, {
                    path: '/path/to/something'
                })
                """)),
            ("../extension.tgz", extensionTgz));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        var parameters = TemplateHelper.ConvertAndAssertParameters(result.Parameters);
        parameters["baz"].Expression.Should().DeepEqual("[concat(externalInputs('sys_expression_0'), '-combined-with-', externalInputs('sys_expression_1'), '-and-with-', externalInputs('sys_expression_2'))]");
        var externalInputs = TemplateHelper.ConvertAndAssertExternalInputs(result.Parameters);
        externalInputs["sys_expression_0"].Should().DeepEqual(new JObject
        {
            ["kind"] = "sys.expression",
            ["config"] = "hello"
        });
        externalInputs["sys_expression_1"].Should().DeepEqual(new JObject
        {
            ["kind"] = "sys.expression",
            ["config"] = 42
        });
        externalInputs["sys_expression_2"].Should().DeepEqual(new JObject
        {
            ["kind"] = "sys.expression",
            ["config"] = new JObject
            {
                ["path"] = "/path/to/something"
            }
        });
    }

    [TestMethod]
    public async Task Function_with_external_input_in_property_access_compiles()
    {
        var extensionTgz = await GetExtensionTgz((factory, types) =>
        {
            var endpointConfigType = factory.Create(() => new ObjectType("endpointConfig", new Dictionary<string, ObjectTypeProperty>(), factory.GetReference(types.AnyType)));

            var getEndpointFunctionType = factory.Create(() => new NamespaceFunctionType(
                "getEndpoint",
                null,
                "[parameters('config')[externalInput('endpointId')]]",
                [new NamespaceFunctionParameter("config", factory.GetReference(endpointConfigType), null, NamespaceFunctionParameterFlags.None)],
                factory.GetReference(types.AnyType),
                BicepSourceFileKind.ParamsFile));

            return [getEndpointFunctionType];
        });
        var services = new ServiceBuilder();
        var result = await CompilationHelper.RestoreAndCompileParams(services,
            ("parameters.bicepparam", new("""
                using none
                extension '../extension.tgz' as ext
                param endpoint = ext.getEndpoint({
                    endpointId: 'endpointId'
                    endpointName: 'endpointName'
                })
                """)),
            ("../extension.tgz", extensionTgz));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        var parameters = TemplateHelper.ConvertAndAssertParameters(result.Parameters);
        parameters["endpoint"].Expression.Should().DeepEqual("[createObject('endpointId', 'endpointId', 'endpointName', 'endpointName')[externalInputs('endpointId_0')]]");

        var externalInputs = TemplateHelper.ConvertAndAssertExternalInputs(result.Parameters);
        externalInputs["endpointId_0"].Should().DeepEqual(new JObject
        {
            ["kind"] = "endpointId",
        });
    }

    [TestMethod]
    public async Task Function_output_type_is_validated()
    {
        var extensionTgz = await GetExtensionTgz((factory, types) =>
        {
            var simpleFunctionType = factory.Create(() => new NamespaceFunctionType(
                "foo",
                null,
                null,
                [new NamespaceFunctionParameter("myParam", factory.GetReference(types.StringType), null, NamespaceFunctionParameterFlags.Required)],
                factory.GetReference(types.StringType),
                null));

            return [simpleFunctionType];
        });
        var services = new ServiceBuilder();
        var result = await CompilationHelper.RestoreAndCompile(services,
            ("main.bicep", new("""
                extension '../extension.tgz' as ext
                var result = ext.foo('myValue')
                var numberValue = result + 42
                """)),
            ("../extension.tgz", extensionTgz));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP045", DiagnosticLevel.Error, "Cannot apply operator \"+\" to operands of type \"string\" and \"42\"."),
        });
    }

    [TestMethod]
    public async Task Nested_namespace_function_calls_compiles()
    {
        var extensionTgz = await GetExtensionTgz((factory, types) =>
        {
            var simpleFunctionType = factory.Create(() => new NamespaceFunctionType(
                "foo",
                null,
                null,
                [new NamespaceFunctionParameter("myParam", factory.GetReference(types.StringType), null, NamespaceFunctionParameterFlags.Required)],
                factory.GetReference(types.StringType),
                null));

            var bazType = factory.Create(() => new NamespaceFunctionType(
                "baz",
                null,
                "[parameters('configKey')]",
                [new NamespaceFunctionParameter("configKey", factory.GetReference(types.StringType), null, NamespaceFunctionParameterFlags.Required)],
                factory.GetReference(types.StringType),
                null));

            return [simpleFunctionType, bazType];
        });
        var services = new ServiceBuilder();
        var result = await CompilationHelper.RestoreAndCompile(services,
            ("main.bicep", new("""
                extension '../extension.tgz' as ext
                var nested = ext.baz(ext.foo('innerValue'))
                """)),
            ("../extension.tgz", extensionTgz));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
    }

    private static async Task<BinaryData> GetExtensionTgz(Func<TypeFactory, CommonPrimitives, TypeBase[]> configureFunctions)
    {
        var factory = new TypeFactory([]);
        var commonTypes = new CommonPrimitives(
            IntType: factory.Create(() => new IntegerType()),
            StringType: factory.Create(() => new StringType()),
            BoolType: factory.Create(() => new BooleanType()),
            AnyType: factory.Create(() => new AnyType()));

        // Let the caller configure the functions
        var functions = configureFunctions(factory, commonTypes);

        var settings = new TypeSettings(name: "ThirdPartyExtension", version: "1.0.0", isSingleton: false, configurationType: null!);

        var functionReferences = functions.Select(f => new CrossFileTypeReference("types.json", factory.GetIndex(f))).ToList();

        var index = new TypeIndex(
            new Dictionary<string, CrossFileTypeReference>(),
            new Dictionary<string, IReadOnlyDictionary<string, IReadOnlyList<CrossFileTypeReference>>>(),
            functionReferences,
            settings,
            null);

        var indexTgz = ExtensionResourceTypeHelper.GetTypesTgzBytesFromFiles(
            ("index.json", StreamHelper.GetString(stream => TypeSerializer.SerializeIndex(stream, index))),
            ("types.json", StreamHelper.GetString(stream => TypeSerializer.Serialize(stream, factory.GetTypes()))));

        return await ExtensionV1Archive.Build(new(indexTgz, false, []));
    }

    private record CommonPrimitives(TypeBase IntType, TypeBase StringType, TypeBase BoolType, TypeBase AnyType);
}
