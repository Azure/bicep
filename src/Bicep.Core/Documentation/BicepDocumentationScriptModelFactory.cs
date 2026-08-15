// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Scriban.Runtime;

namespace Bicep.Core.Documentation;

internal static class BicepDocumentationScriptModelFactory
{
    public static ScriptObject Create(BicepDocumentationModel model)
    {
        var custom = CreateCustom(model.Custom);

        return new ScriptObject
        {
            { "custom", custom },
            { "module", CreateModule(model, custom) },
        };
    }

    private static ScriptObject CreateModule(BicepDocumentationModel model, ScriptObject custom) => new()
    {
        { "name", model.Name },
        { "description", model.Description },
        { "path", model.Path },
        { "targetScope", model.TargetScope },
        { "custom", custom },
        { "resourceTypes", CreateArray(model.ResourceTypes, CreateResourceType) },
        { "parameters", CreateParameters(model.Parameters) },
        { "outputs", CreateArray(model.Outputs, CreateOutput) },
        { "exportedFunctions", CreateArray(model.ExportedFunctions, CreateFunction) },
        { "references", CreateArray(model.References, CreateReference) },
        { "usageExamples", CreateArray(model.UsageExamples, CreateUsageExample) },
    };

    private static ScriptObject CreateCustom(ImmutableSortedDictionary<string, string> custom)
    {
        var scriptObject = new ScriptObject();
        foreach (var (key, value) in custom)
        {
            scriptObject.Add(key, value);
        }

        return scriptObject;
    }

    private static ScriptObject CreateResourceType(BicepDocumentationResourceType resourceType) => new()
    {
        { "type", resourceType.Type },
        { "existing", resourceType.IsExisting },
    };

    private static ScriptObject CreateParameter(BicepDocumentationParameter parameter) => new()
    {
        { "name", parameter.Name },
        { "type", parameter.TypeName },
        { "required", parameter.IsRequired },
        { "secure", parameter.IsSecure },
        { "description", parameter.Description },
        { "defaultValue", parameter.DefaultValue },
        { "defaultValueFence", parameter.DefaultValue is { } defaultValue ? GetCodeFence(defaultValue) : null },
        { "allowedValues", CreateStrings(parameter.AllowedValues) },
        { "minValue", parameter.MinValue },
        { "maxValue", parameter.MaxValue },
        { "minLength", parameter.MinLength },
        { "maxLength", parameter.MaxLength },
        { "pattern", parameter.Pattern },
        { "truncated", parameter.IsTruncated },
        { "properties", CreateParameters(parameter.NestedProperties) },
        { "discriminator", parameter.Discriminator is { } discriminator ? CreateDiscriminator(discriminator) : null },
    };

    private static ScriptObject CreateDiscriminator(BicepDocumentationDiscriminator discriminator) => new()
    {
        { "propertyName", discriminator.PropertyName },
        { "cases", CreateDiscriminatorCases(discriminator.Cases) },
    };

    private static ScriptObject CreateDiscriminatorCase(BicepDocumentationDiscriminatorCase discriminatorCase) => new()
    {
        { "value", discriminatorCase.Value },
        { "properties", CreateParameters(discriminatorCase.Properties) },
    };

    private static ScriptObject CreateOutput(BicepDocumentationOutput output) => new()
    {
        { "name", output.Name },
        { "type", output.TypeName },
        { "secure", output.IsSecure },
        { "description", output.Description },
    };

    private static ScriptObject CreateFunction(BicepDocumentationFunction function) => new()
    {
        { "name", function.Name },
        { "parameters", CreateArray(function.Parameters, CreateFunctionParameter) },
        { "returnType", function.ReturnTypeName },
        { "description", function.Description },
    };

    private static ScriptObject CreateFunctionParameter(BicepDocumentationFunctionParameter parameter) => new()
    {
        { "name", parameter.Name },
        { "type", parameter.TypeName },
        { "description", parameter.Description },
    };

    private static ScriptObject CreateReference(BicepDocumentationReference reference) => new()
    {
        { "symbolicName", reference.SymbolicName },
        { "path", reference.Path },
        { "description", reference.Description },
    };

    private static ScriptObject CreateUsageExample(BicepDocumentationUsageExample example) => new()
    {
        { "name", example.Name },
        { "path", example.RelativePath },
        { "description", example.Description },
        { "contents", example.Contents },
        { "fence", GetCodeFence(example.Contents) },
    };

    private static ScriptArray CreateParameters(ImmutableArray<BicepDocumentationParameter> parameters)
    {
        var array = new ScriptArray();
        foreach (var parameter in parameters)
        {
            array.Add(CreateParameter(parameter));
        }

        return array;
    }

    private static ScriptArray CreateDiscriminatorCases(ImmutableArray<BicepDocumentationDiscriminatorCase> cases)
    {
        var array = new ScriptArray();
        foreach (var discriminatorCase in cases)
        {
            array.Add(CreateDiscriminatorCase(discriminatorCase));
        }

        return array;
    }

    private static ScriptArray CreateStrings(ImmutableArray<string> values)
    {
        var array = new ScriptArray();
        foreach (var value in values)
        {
            array.Add(value);
        }

        return array;
    }

    private static ScriptArray CreateArray<T>(ImmutableArray<T> items, Func<T, object?> project)
    {
        var array = new ScriptArray();
        foreach (var item in items)
        {
            array.Add(project(item));
        }

        return array;
    }

    private static string GetCodeFence(string contents)
    {
        var longestRun = 0;
        var currentRun = 0;
        foreach (var character in contents)
        {
            currentRun = character == '`' ? currentRun + 1 : 0;
            longestRun = Math.Max(longestRun, currentRun);
        }

        return new string('`', Math.Max(3, longestRun + 1));
    }
}
