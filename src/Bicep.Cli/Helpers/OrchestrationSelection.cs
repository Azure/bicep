// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json;
using Azure.Deployments.Expression.Configuration;
using Azure.Deployments.Expression.Engines;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.TypeSystem;
using Bicep.Core.Utils.Snapshots;
using Newtonsoft.Json.Linq;

namespace Bicep.Cli.Helpers;

public record OrchestrationSelection(
    string Template,
    ImmutableArray<JsonElement> Resources,
    ImmutableDictionary<string, ImmutableHashSet<string>> Dependencies,
    ImmutableHashSet<string> Selected)
{
    private const string DependenciesProperty = "__bicepOrchestrationDependencies";
    private const string LabelsLengthProperty = "__bicepOrchestrationLabelsLength";

    public static ImmutableDictionary<string, ImmutableHashSet<string>> ParseLabels(IEnumerable<string> selectors)
    {
        var labels = new Dictionary<string, ImmutableHashSet<string>.Builder>(StringComparer.Ordinal);
        foreach (var selector in selectors)
        {
            var equals = selector.IndexOf('=');
            if (equals <= 0 || equals == selector.Length - 1)
            {
                throw new CommandLineException($"Invalid label selector '{selector}'. Expected key=value1,value2.");
            }

            var key = selector[..equals];
            var values = selector[(equals + 1)..].Split(',');
            if (string.IsNullOrWhiteSpace(key) || values.Any(string.IsNullOrWhiteSpace))
            {
                throw new CommandLineException($"Invalid label selector '{selector}'. Label keys and values must not be empty.");
            }

            if (!labels.TryGetValue(key, out var acceptedValues))
            {
                labels[key] = acceptedValues = ImmutableHashSet.CreateBuilder<string>(StringComparer.Ordinal);
            }
            acceptedValues.UnionWith(values);
        }

        return labels.ToImmutableDictionary(entry => entry.Key, entry => entry.Value.ToImmutable(), StringComparer.Ordinal);
    }

    public static async Task<OrchestrationSelection> Create(
        string templateContent,
        string parametersContent,
        ImmutableDictionary<string, ImmutableHashSet<string>> labels,
        bool teardown,
        CancellationToken cancellationToken)
    {
        var template = JObject.Parse(templateContent);
        if (template["resources"] is not JObject resources)
        {
            throw new CommandLineException("Label selection requires a symbolic orchestration template.");
        }

        // Evaluate dependency expressions in the same loop/parameter context as each stack's labels.
        var planningTemplate = (JObject)template.DeepClone();
        foreach (var resource in planningTemplate.GetValue("resources") is JObject planningResources
            ? planningResources.Properties()
            : throw new CommandLineException("Unable to read orchestration resources."))
        {
            if (resource.Value["properties"] is not JObject properties)
            {
                throw new CommandLineException($"Unable to read properties for stack '{resource.Name}'.");
            }
            properties[DependenciesProperty] = resource.Value["dependsOn"]?.DeepClone() ?? new JArray();
            var stackLabels = GetLabelsExpression(properties["body"]?["labels"] ?? new JObject());
            // A numeric result distinguishes resolved strings (including literal brackets) from unevaluated expressions.
            properties[LabelsLengthProperty] = ExpressionsEngine.SerializeExpression(
                new FunctionExpression("length", [new FunctionExpression("string", [stackLabels], [])], []),
                new ExpressionSerializerSettings { IncludeOuterSquareBrackets = true });
        }

        var snapshot = await SnapshotHelper.GetSnapshot(
            ResourceScope.Orchestrator, planningTemplate.ToString(), parametersContent,
            tenantId: null, managementGroupId: null, subscriptionId: null, resourceGroup: null,
            deploymentName: null, location: null, externalInputs: [], includeSymbolicNames: true, cancellationToken);
        if (snapshot.Diagnostics.Length > 0)
        {
            throw new CommandLineException($"Unable to resolve stacks for label selection: {string.Join(Environment.NewLine, snapshot.Diagnostics)}");
        }

        var stacks = snapshot.PredictedResources.ToImmutableDictionary(
            resource => resource.GetProperty("symbolicName").GetString()
                ?? throw new CommandLineException("Unable to resolve a stack's symbolic name."),
            StringComparer.Ordinal);
        var dependencies = stacks.ToImmutableDictionary(
            entry => entry.Key,
            entry => ResolveDependencies(entry.Value, stacks.Keys),
            StringComparer.Ordinal);
        var selected = stacks.Where(entry => MatchesLabels(entry.Key, entry.Value, labels))
            .Select(entry => entry.Key).ToImmutableHashSet(StringComparer.Ordinal);
        if (selected.Count == 0)
        {
            throw new CommandLineException("No stacks match the supplied label selectors.");
        }

        if (teardown)
        {
            foreach (var surviving in dependencies.Where(entry => !selected.Contains(entry.Key)))
            {
                var removed = surviving.Value.Intersect(selected).Order(StringComparer.Ordinal).ToArray();
                if (removed.Length > 0)
                {
                    throw new CommandLineException($"Cannot tear down the selected stacks: unselected stack '{surviving.Key}' requires {string.Join(", ", removed)}.");
                }
            }
        }
        else
        {
            var pending = new Queue<string>(selected);
            while (pending.TryDequeue(out var name))
            {
                foreach (var dependency in dependencies[name])
                {
                    if (!selected.Contains(dependency))
                    {
                        selected = selected.Add(dependency);
                        pending.Enqueue(dependency);
                    }
                }
            }
        }

        foreach (var resource in resources.Properties())
        {
            var names = selected.Where(name => name == resource.Name || name.StartsWith($"{resource.Name}[", StringComparison.Ordinal))
                .Order(StringComparer.Ordinal).ToArray();
            var symbolExpression = resource.Value["copy"] is JObject
                ? $"format('{resource.Name}[{{0}}]', copyIndex())"
                : $"'{resource.Name}'";
            var selectionExpression = names.Length == 0 ? "false()" :
                $"contains(createArray({string.Join(", ", names.Select(name => $"'{name}'"))}), {symbolExpression})";
            if (resource.Value["condition"] is { } condition)
            {
                var original = condition.Type == JTokenType.Boolean
                    ? condition.Value<bool>() ? "true()" : "false()"
                    : condition.Value<string>() is { } expression && expression.StartsWith('[') && expression.EndsWith(']')
                        ? expression[1..^1]
                        : throw new CommandLineException($"Unable to resolve the condition for stack '{resource.Name}'.");
                selectionExpression = $"and({original}, {selectionExpression})";
            }
            resource.Value["condition"] = $"[{selectionExpression}]";
        }

        return new(template.ToString(),
            [.. snapshot.PredictedResources.Where(resource => selected.Contains(resource.GetProperty("symbolicName").GetString() ?? ""))],
            dependencies, selected);
    }

    private static bool MatchesLabels(
        string name,
        JsonElement resource,
        ImmutableDictionary<string, ImmutableHashSet<string>> selectors)
    {
        var body = resource.GetProperty("properties").GetProperty("body");
        if (resource.GetProperty("properties").GetProperty(LabelsLengthProperty).ValueKind != JsonValueKind.Number)
        {
            throw new CommandLineException($"Labels for stack '{name}' must be resolvable before deployment.");
        }
        if (!body.TryGetProperty("labels", out var labels))
        {
            return selectors.Count == 0;
        }
        if (labels.ValueKind != JsonValueKind.Object || labels.EnumerateObject().Any(label => label.Value.ValueKind != JsonValueKind.String))
        {
            throw new CommandLineException($"Labels for stack '{name}' must resolve to an object of string values before deployment.");
        }

        return selectors.All(selector =>
        {
            if (!labels.TryGetProperty(selector.Key, out var value) || value.GetString() is not { } text)
            {
                return false;
            }
            // Snapshot strings retain ARM's escape prefix for literal opening brackets.
            return selector.Value.Contains(text.StartsWith("[[", StringComparison.Ordinal) ? text[1..] : text);
        });
    }

    private static LanguageExpression GetLabelsExpression(JToken labels) => labels switch
    {
        JObject obj => new FunctionExpression("createObject",
            [.. obj.Properties().SelectMany(property => new LanguageExpression[]
            {
                GetLabelsExpression(new JValue(property.Name)),
                GetLabelsExpression(property.Value),
            })], []),
        JValue { Type: JTokenType.String } value when value.Value<string>() is { } text =>
            ExpressionsEngine.IsLanguageExpression(text) ? ExpressionsEngine.ParseLanguageExpression(text) : new JTokenExpression(text),
        _ => new FunctionExpression("json", [new JTokenExpression(labels.ToString(Newtonsoft.Json.Formatting.None))], []),
    };

    private static ImmutableHashSet<string> ResolveDependencies(JsonElement resource, IEnumerable<string> names)
    {
        var dependencies = ImmutableHashSet.CreateBuilder<string>(StringComparer.Ordinal);
        foreach (var dependency in resource.GetProperty("properties").GetProperty(DependenciesProperty).EnumerateArray())
        {
            if (dependency.ValueKind != JsonValueKind.String || dependency.GetString() is not { } name || name.StartsWith('['))
            {
                throw new CommandLineException("Stack dependencies must be resolvable before label selection.");
            }
            // A collection dependency refers to every active instance, including zero instances.
            dependencies.UnionWith(names.Where(candidate => candidate == name || candidate.StartsWith($"{name}[", StringComparison.Ordinal)));
        }
        return dependencies.ToImmutable();
    }

    public ImmutableArray<ImmutableArray<string>> GetTeardownBatches()
    {
        var remaining = Selected;
        var batches = ImmutableArray.CreateBuilder<ImmutableArray<string>>();
        while (remaining.Count > 0)
        {
            var batch = remaining.Where(name => !remaining.Any(other => Dependencies[other].Contains(name)))
                .Order(StringComparer.Ordinal).ToImmutableArray();
            if (batch.Length == 0)
            {
                throw new CommandLineException("Cannot determine teardown order because stack dependencies contain a cycle.");
            }
            batches.Add(batch);
            remaining = remaining.Except(batch);
        }
        return batches.ToImmutable();
    }
}
