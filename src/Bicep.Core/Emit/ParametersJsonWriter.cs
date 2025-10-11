// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using Bicep.Core.Intermediate;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Microsoft.Identity.Client;
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
                        emitter.EmitProperty("expression", () => emitter.EmitLanguageExpression(expression));
                    }
                    else
                    {
                        throw new InvalidOperationException($"The '{assignment.Name}' parameter assignment defined neither a concrete value nor a key vault reference");
                    }
                });
            }
        });

        if (this.Context.ExternalInputReferences.ExternalInputIndexMap.Count > 0)
        {
            if (this.Context.ExternalInputReferences.ParametersReferences.Count > 0)
            {
                WriteExternalInputDefinitions(emitter, this.Context.ExternalInputReferences.ExternalInputIndexMap);
            }
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
                    emitter.EmitProperty("expression", () => emitter.EmitLanguageExpression(expression));
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

    private void WriteExternalInputDefinitions(ExpressionEmitter emitter, IDictionary<FunctionCallSyntaxBase, string> externalInputIndexMap)
    {
        emitter.EmitObjectProperty("externalInputDefinitions", () =>
        {
            var parametersWithExternalInputs = this.Context.ExternalInputReferences.ParametersReferences;
            var variablesWithExternalInputs = this.Context.ExternalInputReferences.VariablesReferences;

            var syntaxNodesToInclude = new HashSet<SyntaxBase>();

            foreach (var param in parametersWithExternalInputs)
            {
                // Add the parameter's declaring syntax
                syntaxNodesToInclude.Add(param.DeclaringParameterAssignment);

                // Add all variables referenced by this parameter
                var closure = this.Context.SemanticModel.Binder.GetReferencedSymbolClosureFor(param);
                foreach (var symbol in closure.OfType<VariableSymbol>())
                {
                    if (variablesWithExternalInputs.Contains(symbol))
                    {
                        syntaxNodesToInclude.Add(symbol.DeclaringVariable);
                    }
                }
            }

            // Filter the external input index map to only include function calls that are within the syntax nodes we want to include
            var filteredReferences = externalInputIndexMap
                .Where(kvp => syntaxNodesToInclude.Any(node => this.Context.SemanticModel.Binder.IsDescendant(kvp.Key, node)))
                .OrderBy(x => x.Value);

            // Sort the external input references by name for deterministic ordering
            foreach (var reference in filteredReferences)
            {
                var expression = (FunctionCallExpression)ExpressionBuilder.Convert(reference.Key);
                var hasInline = this.Context.ExternalInputReferences.InlineFunctions.TryGetValue(reference.Key, out var inlineArgs);

                emitter.EmitObjectProperty(reference.Value, () =>
                {
                    if (hasInline && inlineArgs is { } sa)
                    {
                        emitter.EmitProperty("kind", sa.Kind);
                        if (!string.IsNullOrEmpty(sa.Config))
                        {
                            emitter.EmitProperty("config", sa.Config);
                        }
                    }
                    else
                    {
                        emitter.EmitProperty("kind", expression.Parameters[0]);
                        if (expression.Parameters.Length > 1)
                        {
                            emitter.EmitProperty("config", expression.Parameters[1]);
                        }
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
}
