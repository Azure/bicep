// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Intermediate;
using Bicep.Core.Semantics;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit;

public class ParametersJsonWriter
{
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
                        WriteKeyVaultReference(emitter, keyVaultReference);
                    }
                    else if (parameter.Value is { } value)
                    {
                        emitter.EmitProperty("value", () => value.WriteTo(jsonWriter));
                    }
                    else if (parameter.Expression is { } expression)
                    {
                        emitter.EmitProperty("expression", expression);
                    }
                    else
                    {
                        throw new InvalidOperationException($"The '{assignment.Name}' parameter assignment defined neither a concrete value nor a key vault reference");
                    }
                });
            }
        });
        
        if (this.Context.SemanticModel.Root.ParameterAssignments.Length > 0 &&
            this.Context.ExternalInputReferences.ExternalInputIndexMap.Count > 0)
        {
            emitter.EmitObjectProperty("externalInputDefinitions", () =>
            {
                foreach (var reference in this.Context.ExternalInputReferences.ExternalInputIndexMap)
                {
                    var functionCall = reference.Key;
                    var index = reference.Value;

                    var expression = (FunctionCallExpression)ExpressionBuilder.Convert(functionCall);
                    emitter.EmitObjectProperty(index.ToString(), () =>
                    {
                        emitter.EmitProperty("kind", expression.Parameters[0]);
                        emitter.EmitProperty("options", expression.Parameters[1]);
                    });
                }
            });
        }

        if (this.Context.SemanticModel.Features is { ExtensibilityEnabled: true, ModuleExtensionConfigsEnabled: true })
        {
            WriteExtensionConfigs(emitter, this.Context.SemanticModel.Root.ExtensionConfigAssignments);
        }

        jsonWriter.WriteEndObject();

        var content = jsonWriter.ToString();
        return content.FromJson<JToken>();
    }

    private static void WriteKeyVaultReference(ExpressionEmitter emitter, ParameterKeyVaultReferenceExpression keyVaultReference)
    {
        emitter.EmitObjectProperty("reference", () =>
        {
            emitter.EmitObjectProperty("keyVault", () =>
            {
                emitter.EmitProperty("id", keyVaultReference.KeyVaultId);
            });

            emitter.EmitProperty("secretName", keyVaultReference.SecretName);

            if (keyVaultReference.SecretVersion is { } secretVersion)
            {
                emitter.EmitProperty("secretVersion", secretVersion);
            }
        });
    }

    private static void WriteExtensionConfigs(ExpressionEmitter emitter, IEnumerable<ExtensionConfigAssignmentSymbol> extensionConfigAssignments)
    {
        emitter.EmitObjectProperty("extensionConfigs", () =>
        {
            foreach (var extension in extensionConfigAssignments)
            {
                emitter.EmitObjectProperty(extension.Name, () =>
                {
                    // TODO(kylealbert): write config properties
                });
            }
        });
    }
}