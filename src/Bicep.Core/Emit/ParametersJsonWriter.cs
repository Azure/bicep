// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using Azure.Deployments.Expression.Engines;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.ArmHelpers;
using Bicep.Core.Extensions;
using Bicep.Core.Intermediate;
using Bicep.Core.Semantics;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit;

public class ParametersJsonWriter
{
    public const string UsingConfigPropertyName = "usingConfig";

    private ExpressionBuilder ExpressionBuilder { get; }

    private EmitterContext Context => ExpressionBuilder.Context;

    public ParametersJsonWriter(SemanticModel semanticModel)
    {
        ExpressionBuilder = new ExpressionBuilder(new EmitterContext(semanticModel));
    }

    public void Write(SourceAwareJsonTextWriter writer)
    {
        var parametersJToken = GenerateParametersJToken(writer.TrackingJsonWriter);
        parametersJToken.WriteTo(writer);
    }

    private JToken GenerateParametersJToken(PositionTrackingJsonTextWriter jsonWriter)
    {
        var emitter = new ExpressionEmitter(jsonWriter, this.Context);

        jsonWriter.WriteStartObject();
        emitter.EmitProperty("$schema", "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#");
        emitter.EmitProperty("contentVersion", "1.0.0.0");
        emitter.EmitObjectProperty("parameters", () =>
        {
            foreach (var assignment in this.Context.SemanticModel.Root.ParameterAssignments)
            {
                emitter.EmitObjectProperty(assignment.Name, () =>
                {
                    var parameter = this.Context.SemanticModel.EmitLimitationInfo.ParameterAssignments[assignment];

                    if (parameter.KeyVaultReferenceExpression is { } keyVaultReference)
                    {
                        WriteKeyVaultReference(emitter, keyVaultReference, "reference");
                    }
                    else if (parameter.Value is { } value)
                    {
                        emitter.EmitProperty("value", () => value.WriteTo(jsonWriter));
                    }
                    else if (parameter.Expression is { } expression)
                    {
                        // The backend is always expecting an expression string, so we must always ensure we emit
                        // a top-level expression, even if we could simplify by emitting a top-level object.
                        emitter.EmitPropertyWithTransform("expression", expression, RewriteExternalInputReferences);
                    }
                    else
                    {
                        throw new InvalidOperationException($"The '{assignment.Name}' parameter assignment defined neither a concrete value nor a key vault reference");
                    }
                });
            }
        });

        if (this.Context.SemanticModel.EmitLimitationInfo.ExternalInputDefinitions is { } externalInputDefinitions)
        {
            WriteExternalInputDefinitions(emitter, jsonWriter, externalInputDefinitions);
        }

        if (this.Context.SemanticModel.Features.ModuleExtensionConfigsEnabled)
        {
            WriteExtensionConfigs(emitter, jsonWriter);
        }

        if (this.Context.SemanticModel.EmitLimitationInfo.UsingConfig is { } usingConfig)
        {
            emitter.EmitObjectProperty(UsingConfigPropertyName, () =>
            {
                if (usingConfig.KeyVaultReferenceExpression is { } keyVaultReference)
                {
                    WriteKeyVaultReference(emitter, keyVaultReference, "reference");
                }
                else if (usingConfig.Value is { } value)
                {
                    emitter.EmitProperty("value", () => value.WriteTo(jsonWriter));
                }
                else if (usingConfig.Expression is { } expression)
                {
                    // The backend is always expecting an expression string, so we must always ensure we emit
                    // a top-level expression, even if we could simplify by emitting a top-level object.
                    emitter.EmitPropertyWithTransform("expression", expression, RewriteExternalInputReferences);
                }
                else
                {
                    throw new UnreachableException();
                }
            });
        }

        jsonWriter.WriteEndObject();

        var content = jsonWriter.ToString();
        return content.FromJson<JToken>();
    }

    private void WriteExternalInputDefinitions(
        ExpressionEmitter emitter,
        PositionTrackingJsonTextWriter writer,
        IEnumerable<ExternalInputDefinition> externalInputDefinitions)
    {
        emitter.EmitObjectProperty("externalInputDefinitions", () =>
        {
            foreach (var externalInputDefinition in externalInputDefinitions)
            {
                emitter.EmitObjectProperty(externalInputDefinition.Key, () =>
                {
                    emitter.EmitProperty("kind", externalInputDefinition.Kind);
                    if (externalInputDefinition.Config is { } config)
                    {
                        emitter.EmitProperty("config", () => config.WriteTo(writer));
                    }
                });
            }
        });
    }

    private void WriteExtensionConfigs(ExpressionEmitter emitter, PositionTrackingJsonTextWriter jsonWriter)
    {
        emitter.EmitObjectProperty(
            "extensionConfigs", () =>
            {
                foreach (var extension in this.Context.SemanticModel.Root.ExtensionConfigAssignments.OrderBy(a => a.Name))
                {
                    emitter.EmitObjectProperty(
                        extension.Name, () =>
                        {
                            var configProperties = this.Context.SemanticModel.EmitLimitationInfo.ExtensionConfigAssignments[extension];

                            foreach (var configProperty in configProperties.OrderBy(p => p.Key))
                            {
                                emitter.EmitObjectProperty(
                                    configProperty.Key, () =>
                                    {
                                        if (configProperty.Value.KeyVaultReferenceExpression is { } keyVaultReference)
                                        {
                                            WriteKeyVaultReference(emitter, keyVaultReference, "keyVaultReference");
                                        }
                                        else if (configProperty.Value.Value is { } value)
                                        {
                                            emitter.EmitProperty("value", () => value.WriteTo(jsonWriter));
                                        }
                                        else
                                        {
                                            throw new InvalidOperationException($"The '{configProperty.Key}' property of the '{extension.Name}' extension config assignment defined neither a concrete value nor a key vault reference");
                                        }
                                    });
                            }
                        });
                }
            });
    }

    private static void WriteKeyVaultReference(ExpressionEmitter emitter, ParameterKeyVaultReferenceExpression keyVaultReference, string referencePropertyName)
    {
        emitter.EmitObjectProperty(
            referencePropertyName, () =>
            {
                emitter.EmitObjectProperty("keyVault", () => emitter.EmitProperty("id", keyVaultReference.KeyVaultId));

                emitter.EmitProperty("secretName", keyVaultReference.SecretName);

                if (keyVaultReference.SecretVersion is { } secretVersion)
                {
                    emitter.EmitProperty("secretVersion", secretVersion);
                }
            });
    }

    private LanguageExpression RewriteExternalInputReferences(LanguageExpression expression) =>
        LanguageExpressionRewriter.Rewrite(expression, exp =>
        {
            if (exp is not FunctionExpression function || !function.NameEquals(LanguageConstants.ExternalInputBicepFunctionName))
            {
                return exp;
            }

            var serialized = ExpressionsEngine.SerializeExpression(function);
            return !this.Context.SemanticModel.ExternalInputReferences.InfoBySerializedExpression.TryGetValue(serialized, out var info)
                ? exp
                : new FunctionExpression(LanguageConstants.ExternalInputsArmFunctionName, [new JTokenExpression(info.DefinitionKey)], []);
        });
}
