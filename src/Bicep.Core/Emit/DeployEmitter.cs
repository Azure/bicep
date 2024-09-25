// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Azure.Deployments.Core.EventSources;
using Azure.ResourceManager.Resources.Models;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Models;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Emit
{
    public class DeployEmitter
    {
        private record DeploymentScope(string? ManagementGroupId, string? SubscriptionId, string? ResourceGroupName);

        private readonly Compilation compilation;

        public DeployEmitter(Compilation compilation)
        {
            this.compilation = compilation;
        }

        public DeployResult Emit()
        {
            DeployResult CreateFailedResult() => new(false, this.compilation.GetAllDiagnosticsByBicepFile(), null);

            var semanticModel = this.compilation.GetEntrypointSemanticModel();

            if (semanticModel.SourceFileKind != BicepSourceFileKind.DeployFile)
            {
                throw new InvalidOperationException($"Entry-point {semanticModel.Root.FileUri} is not a bicep file");
            }

            if (semanticModel.GetAllDiagnostics().Any(x => x.IsError()) ||
                semanticModel.Root.DeployDeclaration is not { } deployDeclaration ||
                !deployDeclaration.TryGetReferencingSemanticModel().IsSuccess(out var model) ||
                model is not SemanticModel referencingSemanticModel)
            {
                return CreateFailedResult();
            }

            var templateResult = new CompilationEmitter(this.compilation).Template(referencingSemanticModel);

            if (!templateResult.Success)
            {
                throw new InvalidOperationException("The referenced Bicep file is invalid.");
            }

            var deployProperties = deployDeclaration.DeclaringDeploySyntax.BodyObject.ToNamedPropertyDictionary();

            // Emit name.
            var name = Path.GetFileNameWithoutExtension(referencingSemanticModel.SourceFile.FileUri.LocalPath);

            if (deployDeclaration.DeclaringDeploySyntax.BodyObject.TryGetPropertyByName("name")?.Value is StringSyntax nameSyntax &&
                nameSyntax.TryGetLiteralValue() is { } explicitName)
            {
                name = explicitName;
            }

            // Emit scope.
            var scopePropertyValue = deployProperties["scope"].Value;

            if (scopePropertyValue is not FunctionCallSyntax scopeFunctionCall)
            {
                return CreateFailedResult();
            }

            var scopeArguments = scopeFunctionCall.Arguments
                .Select(x => x.Expression as StringSyntax ?? throw new InvalidOperationException("Expect a string literal."))
                .Select(x => x.TryGetLiteralValue() ?? throw new InvalidOperationException("Expect a literal value."))
                .ToArray();

            DeploymentScope scope = scopeFunctionCall.Name.IdentifierName switch
            {
                "tenant" => new(null, null, null),
                "managementGroup" => new(scopeArguments[0], null, null),
                "subscription" => new(null, scopeArguments.Length > 0 ? scopeArguments[0] : Guid.Empty.ToString(), null),
                "resourceGroup" => scopeArguments.Length > 1
                    ? new(null, scopeArguments[0], scopeArguments[1])
                    : new(null, Guid.Empty.ToString(), scopeArguments[0]),
                _ => throw new InvalidOperationException("Invalid scope function call."),
            };

            var parametersSyntax = deployDeclaration.DeclaringDeploySyntax.BodyObject.TryGetPropertyByName("params")?.Value;

            if (parametersSyntax is not ObjectSyntax parametersObject)
            {
                return new DeployResult(true, compilation.GetAllDiagnosticsByBicepFile(), new ArmDeploymentDefinition(
                    scope.ManagementGroupId,
                    scope.SubscriptionId,
                    scope.ResourceGroupName,
                    name,
                    new ArmDeploymentProperties(ArmDeploymentMode.Incremental)
                    {
                        Template = BinaryData.FromString(templateResult.Template ?? throw new InvalidOperationException("Invalid template.")),
                    }));
            }

            var parameters = parametersObject.ToNamedPropertyDictionary();
            var paramsNode = new JsonObject();

            foreach (var (parameterName, parameterProperty) in parameters)
            {
                var parameterValue = (parameterProperty.Value as StringSyntax)?.TryGetLiteralValue() ?? throw new InvalidOperationException("Invalid string parameter");
                var parameterValueObject = new JsonObject()
                {
                    ["value"] = parameterValue,
                };

                paramsNode.Add(parameterName, parameterValueObject);
            }

            var parametersBinaryData = BinaryData.FromObjectAsJson(paramsNode);

            return new DeployResult(true, compilation.GetAllDiagnosticsByBicepFile(), new ArmDeploymentDefinition(
                scope.ManagementGroupId,
                scope.SubscriptionId,
                scope.ResourceGroupName,
                name,
                new ArmDeploymentProperties(ArmDeploymentMode.Incremental)
                {
                    Template = BinaryData.FromString(templateResult.Template ?? throw new InvalidOperationException("Invalid template.")),
                    Parameters = parametersBinaryData,
                }));
        }
    }
}
