// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Deployments.Core.Definitions.Identifiers;
using Azure.Deployments.Expression.Intermediate;
using Azure.Deployments.Expression.Intermediate.Extensions;
using Newtonsoft.Json.Linq;

namespace Bicep.Testing.Framework;

/// <summary>
/// A snapshot value that may be resolved or represented by an ARM expression.
/// </summary>
[JsonConverter(typeof(SnapshotValueJsonConverterFactory))]
public sealed class SnapshotValue<T>
{
    private readonly T? value;

    private SnapshotValue()
    {
    }

    private SnapshotValue(T value)
    {
        this.value = value;
        HasResolvedValue = true;
    }

    private SnapshotValue(FunctionExpression expression)
    {
        Expression = expression;
    }

    /// <summary>
    /// The resolved value. Throws if the value is missing or remains an expression.
    /// </summary>
    public T Value => HasResolvedValue && value is T resolvedValue
        ? resolvedValue
        : throw new InvalidOperationException(IsMissing
            ? "The snapshot value is missing."
            : "The snapshot value is an unresolved ARM expression.");

    /// <summary>
    /// The unresolved ARM expression, or <see langword="null"/> when the value is resolved.
    /// </summary>
    public FunctionExpression? Expression { get; }

    /// <summary>
    /// Indicates whether the value is resolved.
    /// </summary>
    public bool HasValue() => HasResolvedValue;

    /// <summary>
    /// Indicates whether the value is absent from the snapshot.
    /// </summary>
    public bool IsMissing => !HasResolvedValue && Expression is null;

    private bool HasResolvedValue { get; }

    /// <summary>
    /// A snapshot value that is absent.
    /// </summary>
    public static SnapshotValue<T> Missing { get; } = new();

    public static SnapshotValue<T> FromValue(T value) => new(value);

    public static SnapshotValue<T> FromExpression(FunctionExpression expression) => new(expression);
}

/// <summary>
/// A parsed Azure resource ID, with convenient access to scope-specific values.
/// </summary>
public sealed record SnapshotResourceIdentifier(ResourceId ResourceId)
{
    /// <summary>
    /// The subscription ID for subscription- or resource-group-scoped IDs.
    /// </summary>
    public string? SubscriptionId => ResourceId switch
    {
        ResourceGroupLevelResourceId resourceGroupId => resourceGroupId.SubscriptionId,
        SubscriptionLevelResourceId subscriptionId => subscriptionId.SubscriptionId,
        _ => null,
    };

    /// <summary>
    /// The resource-group name for resource-group-scoped IDs.
    /// </summary>
    public string? ResourceGroup => (ResourceId as ResourceGroupLevelResourceId)?.ResourceGroup;

    /// <summary>
    /// The fully qualified resource ID.
    /// </summary>
    public string FullyQualifiedId => ResourceId.FullyQualifiedId;
}

/// <summary>
/// Creates converters for snapshot values.
/// </summary>
public sealed class SnapshotValueJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(SnapshotValue<>);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        => (JsonConverter)(Activator.CreateInstance(typeof(SnapshotValueJsonConverter<>).MakeGenericType(typeToConvert.GetGenericArguments()))
            ?? throw new InvalidOperationException($"Could not create a JSON converter for {typeToConvert}."));
}

/// <summary>
/// Reads resolved values and ARM expressions from snapshot JSON.
/// </summary>
public sealed class SnapshotValueJsonConverter<T> : JsonConverter<SnapshotValue<T>>
{
    public override SnapshotValue<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var text = reader.GetString() ?? throw new JsonException("Snapshot values must be JSON strings.");
        if (text.Length >= 2 && text[0] == '[' && text[^1] == ']')
        {
            var expression = ExpressionParser.ParseLanguageExpression(JToken.FromObject(text), string.Empty);
            return expression is FunctionExpression function
                ? SnapshotValue<T>.FromExpression(function)
                : throw new JsonException("The ARM expression did not parse as a function expression.");
        }

        if (typeof(T) == typeof(string))
        {
            return SnapshotValue<T>.FromValue((T)(object)text);
        }

        if (typeof(T) == typeof(SnapshotResourceIdentifier) &&
            ResourceId.TryParse(text, out var resourceId) &&
            resourceId is not null)
        {
            return SnapshotValue<T>.FromValue((T)(object)new SnapshotResourceIdentifier(resourceId));
        }

        throw new JsonException($"The snapshot value '{text}' could not be deserialized as {typeof(T)}.");
    }

    public override void Write(Utf8JsonWriter writer, SnapshotValue<T> value, JsonSerializerOptions options)
    {
        if (value.IsMissing)
        {
            writer.WriteNullValue();
        }
        else if (value.HasValue())
        {
            if (value.Value is SnapshotResourceIdentifier resourceIdentifier)
            {
                writer.WriteStringValue(resourceIdentifier.FullyQualifiedId);
            }
            else
            {
                JsonSerializer.Serialize(writer, value.Value, options);
            }
        }
        else
        {
            writer.WriteStringValue(value.Expression is { } expression
                ? expression.SerializeToString()
                : throw new InvalidOperationException("The snapshot value has neither a resolved value nor an expression."));
        }
    }
}
