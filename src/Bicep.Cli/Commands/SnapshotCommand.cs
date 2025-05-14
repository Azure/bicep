// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;
using Azure.Deployments.Core.Configuration;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Core.Entities;
using Azure.Deployments.Expression.Intermediate;
using Azure.Deployments.Expression.Intermediate.Extensions;
using Azure.Deployments.Templates.Engines;
using Azure.Deployments.Templates.ParsedEntities;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Helpers.WhatIf;
using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Json;
using Bicep.Core.TypeSystem;
using Bicep.IO.Abstraction;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Bicep.Cli.Commands;

public class SnapshotCommand(
    IOContext io,
    ILogger logger,
    IFileExplorer fileExplorer,
    BicepCompiler compiler,
    DiagnosticLogger diagnosticLogger) : ICommand
{
    public record Snapshot(
        ImmutableArray<JsonElement> PredictedResources,
        ImmutableArray<string> Diagnostics);

    public async Task<int> RunAsync(SnapshotArguments args, CancellationToken cancellationToken)
    {
        logger.LogWarning($"WARNING: The '{args.CommandName}' CLI command group is an experimental feature. Experimental features should be enabled for testing purposes only, as there are no guarantees about the quality or stability of these features. Do not enable these settings for any production usage, or your production environment may be subject to breaking.");

        var snapshotMode = args.Mode ?? SnapshotArguments.SnapshotMode.Overwrite;

        var inputUri = ArgumentHelper.GetFileUri(args.InputFile);
        ArgumentHelper.ValidateBicepParamFile(inputUri);

        var newSnapshot = await GetSnapshot(args, inputUri, noRestore: false, cancellationToken)
            ?? throw new CommandLineException($"Failed to generate snapshot for file {inputUri.LocalPath}");

        if (newSnapshot.Diagnostics.Length > 0)
        {
            logger.LogWarning("Diagnostics:");
            foreach (var diagnostic in newSnapshot.Diagnostics)
            {
                logger.LogWarning($"  {diagnostic}");
            }
        }

        var outputUri = PathHelper.ChangeExtension(inputUri, ".snapshot.json").ToIOUri();
        switch (snapshotMode)
        {
            case SnapshotArguments.SnapshotMode.Overwrite:
                WriteSnapshot(outputUri, newSnapshot);
                return 0;
            case SnapshotArguments.SnapshotMode.Validate:
                {
                    var oldSnapshot = ReadSnapshot(outputUri);

                    var changes = SnapshotDiffer.CalculateChanges(oldSnapshot, newSnapshot);
                    await io.Output.WriteAsync(WhatIfOperationResultFormatter.Format(changes));
                    return changes.Any() ? 1 : 0;
                }
            default:
                throw new UnreachableException();
        }
    }

    private async Task<Snapshot?> GetSnapshot(SnapshotArguments arguments, Uri inputUri, bool noRestore, CancellationToken cancellationToken)
    {
        var compilation = await compiler.CreateCompilation(inputUri, skipRestore: noRestore);
        CommandHelper.LogExperimentalWarning(logger, compilation);

        var summary = diagnosticLogger.LogDiagnostics(DiagnosticOptions.Default, compilation);

        if (summary.HasErrors ||
            compilation.Emitter.Parameters() is not { } result ||
            result.Template?.Template is not { } templateContent ||
            result.Parameters is not { } parametersContent)
        {
            return null;
        }

        var parameters = parametersContent.FromJson<DeploymentParametersDefinition>();
        var template = TemplateEngine.ParseTemplate(templateContent);
        var scope = compilation.GetEntrypointSemanticModel().TargetScope switch
        {
            ResourceScope.Tenant => TemplateDeploymentScope.Tenant,
            ResourceScope.ManagementGroup => TemplateDeploymentScope.ManagementGroup,
            ResourceScope.Subscription => TemplateDeploymentScope.Subscription,
            ResourceScope.ResourceGroup => TemplateDeploymentScope.ResourceGroup,
            var otherwise => throw new CommandLineException($"Cannot create snapshot of template with a target scope of {otherwise}"),
        };

        var expansionResult = await TemplateEngine.ExpandNestedDeployments(
            EmitConstants.NestedDeploymentResourceApiVersion,
            scope,
            template,
            parameters: ResolveParameters(parameters),
            rootDeploymentMetadata: GetDeploymentMetadata(arguments, scope, template),
            cancellationToken: cancellationToken);

        return new(
            [
                ..expansionResult.preflightResources.Select(x => JsonElementFactory.CreateElement(JsonExtensions.ToJson(DeploymentPreflightResourceWithParsedExpressions.From(x)))),
                ..expansionResult.extensibleResources.Select(x => JsonElementFactory.CreateElement(JsonExtensions.ToJson(x))),
            ],
            [
                ..expansionResult.diagnostics.Select(d => $"{d.Target} {d.Level} {d.Code}: {d.Message}")
            ]);
    }

    private static Dictionary<string, ITemplateLanguageExpression>? ResolveParameters(DeploymentParametersDefinition? deploymentParameters)
        => deploymentParameters?.Parameters?.ToDictionary(kvp => kvp.Key, kvp => ResolveParameter(kvp.Key, kvp.Value));

    private static ITemplateLanguageExpression ResolveParameter(
        string parameterName,
        DeploymentParameterDefinition parameter)
    {
        if (parameter.Value is not null)
        {
            return JTokenConverter.ConvertToLanguageExpression(parameter.Value);
        }

        if (parameter.Reference is not null)
        {
            return new FunctionExpression("parameters", [parameterName.AsExpression()], null, irreducible: true);
        }

        if (parameter.Expression is not null)
        {
            return RewriteVisitor.Instance.Rewrite(ExpressionParser.ParseLanguageExpression(parameter.Expression));
        }

        throw new InvalidOperationException(
            $"Parameters compilation produced an invalid object for parameter '{parameterName}'.");
    }

    private class RewriteVisitor : ExpressionRewriteVisitor
    {
        internal static readonly RewriteVisitor Instance = new();

        private RewriteVisitor() { }

        protected override ITemplateLanguageExpression ReplaceFunctionExpression(FunctionExpression expression)
        {
            if (expression.Name.Equals(LanguageConstants.ExternalInputsArmFunctionName, StringComparison.OrdinalIgnoreCase))
            {
                return expression with
                {
                    Arguments = [.. expression.Arguments.Select(Rewrite)],
                    Properties = [.. expression.Properties.Select(Rewrite)],
                    // we won't know the value of external inputs until the real deployment
                    Irreducible = true,
                };
            }

            return base.ReplaceFunctionExpression(expression);
        }
    }

    private static Dictionary<string, ITemplateLanguageExpression> GetDeploymentMetadata(
        SnapshotArguments arguments,
        TemplateDeploymentScope scope,
        Template template)
    {
        Dictionary<string, ITemplateLanguageExpression> metadata = new(StringComparer.OrdinalIgnoreCase);

        if (arguments.TenantId is not null)
        {
            metadata[DeploymentMetadata.TenantKey] = new ObjectExpression(
                [
                    new("countryCode".AsExpression(), MetadataPlaceholder("tenant", "countryCode")),
                    new("displayName".AsExpression(), MetadataPlaceholder("tenant", "displayName")),
                    new("id".AsExpression(), $"/tenants/{arguments.TenantId}".AsExpression()),
                    new("tenantId".AsExpression(), arguments.TenantId.AsExpression()),
                ],
                position: null);
        }

        if (arguments.SubscriptionId is not null)
        {
            if (scope is not TemplateDeploymentScope.Subscription and not TemplateDeploymentScope.ResourceGroup)
            {
                throw new CommandLineException($"Subscription ID cannot be specified for a template of scope {scope}");
            }

            metadata[DeploymentMetadata.SubscriptionKey] = new ObjectExpression(
                [
                    new("id".AsExpression(), $"/subscriptions/{arguments.SubscriptionId}".AsExpression()),
                    new("subscriptionId".AsExpression(), arguments.SubscriptionId.AsExpression()),
                    new(
                        "tenantId".AsExpression(),
                        arguments.TenantId?.AsExpression() ?? MetadataPlaceholder("tenant", "tenantId")),
                    new("displayName".AsExpression(), MetadataPlaceholder("subscription", "displayName")),
                ],
                position: null);
        }

        if (arguments.ResourceGroup is not null)
        {
            if (scope is not TemplateDeploymentScope.ResourceGroup)
            {
                throw new CommandLineException($"Resource group name cannot be specified for a template of scope {scope}");
            }

            metadata[DeploymentMetadata.ResourceGroupKey] = new ObjectExpression(
                [
                    new("id".AsExpression(), arguments.SubscriptionId is not null
                        ? $"/subscriptions/{arguments.SubscriptionId}/resourceGroups/{arguments.ResourceGroup}".AsExpression()
                        : MetadataPlaceholder("resourceGroup", "id")),
                    new("name".AsExpression(), arguments.ResourceGroup.AsExpression()),
                    new("type".AsExpression(), "Microsoft.Resources/resourceGroups".AsExpression()),
                    new("location".AsExpression(), arguments.Location is not null
                        ? arguments.Location.AsExpression()
                        : MetadataPlaceholder("resourceGroup", "location")),
                    new("tags".AsExpression(), MetadataPlaceholder("resourceGroup", "tags")),
                    new("managedBy".AsExpression(), MetadataPlaceholder("resourceGroup", "managedBy")),
                    new("properties".AsExpression(), MetadataPlaceholder("resourceGroup", "properties")),
                ],
                position: null);
        }

        if (arguments.DeploymentName is not null ||
            (arguments.Location is not null && scope is not TemplateDeploymentScope.ResourceGroup))
        {
            Dictionary<ITemplateLanguageExpression, ITemplateLanguageExpression> properties = new()
            {
                {
                    "name".AsExpression(),
                    arguments.DeploymentName?.AsExpression() ?? MetadataPlaceholder("deployment", "name")
                },
                {
                    "properties".AsExpression(),
                    new ObjectExpression(
                        [
                            new("template".AsExpression(), new ObjectExpression(
                                [new("contentVersion".AsExpression(), template.ContentVersion.Value.AsExpression())],
                                position: null))
                        ],
                        position: null)
                },
            };

            if (scope is not TemplateDeploymentScope.ResourceGroup)
            {
                properties["location".AsExpression()] = arguments.Location?.AsExpression()
                    ?? MetadataPlaceholder("deployment", "location");
            }

            metadata[DeploymentMetadata.DeploymentKey] = new ObjectExpression(properties, position: null);
        }

        return metadata;
    }

    private static ITemplateLanguageExpression MetadataPlaceholder(string name, params string[] properties)
        => new FunctionExpression(name, [], [.. properties.Select(p => p.AsExpression())], null, irreducible: true);

    private Snapshot ReadSnapshot(IOUri uri)
    {
        var file = fileExplorer.GetFile(uri);
        if (!file.TryReadAllText().IsSuccess(out var contents, out var failureBuilder))
        {
            var message = failureBuilder(DiagnosticBuilder.ForDocumentStart()).Message;
            throw new CommandLineException($"Error opening file {uri.GetLocalFilePath()}: {message}.");
        }

#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        return JsonSerializer.Deserialize<Snapshot>(contents, serializerOptions) ?? throw new InvalidOperationException($"Failed to read from {uri.GetLocalFilePath()}");
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
    }

    private void WriteSnapshot(IOUri uri, Snapshot newSnapshot)
    {
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        var contents = JsonSerializer.Serialize(newSnapshot, serializerOptions) ?? throw new InvalidOperationException($"Failed to write to {uri.GetLocalFilePath()}");
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code

        var file = fileExplorer.GetFile(uri);
        file.Write(contents);
    }

    private static readonly JsonSerializerOptions serializerOptions = new(JsonSerializerDefaults.Web)
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };
}
