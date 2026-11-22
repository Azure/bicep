// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.Json;
using Bicep.Core.Artifacts;
using Bicep.Core.Artifacts.References;
using Bicep.Core.Diagnostics;
using Bicep.Core.SourceGraph;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Configuration;

/// <summary>
/// An entry of <c>artifacts.aliases</c>, referenced as <c>&lt;alias&gt;::&lt;artifact&gt;</c>.
/// </summary>
public abstract record ArtifactAlias(string Name);

/// <param name="RepositoryPrefix">Prepended to the artifact path. If it's the complete repository, a reference can name only the tag or digest, as in <c>storage:::1.0.0</c>.</param>
public sealed record OciArtifactAlias(string Name, string Registry, string? RepositoryPrefix) : ArtifactAlias(Name);

public sealed record TemplateSpecArtifactAlias(string Name, string SubscriptionId, string ResourceGroup) : ArtifactAlias(Name);

/// <param name="Path">The alias root, relative to <paramref name="DeclaringConfigUri"/>.</param>
/// <param name="DeclaringConfigUri">The configuration file that declared <paramref name="Path"/>.</param>
public sealed record FileArtifactAlias(string Name, RelativePath Path, IOUri DeclaringConfigUri) : ArtifactAlias(Name);

/// <summary>
/// Binds the <c>artifacts</c> configuration section.
/// </summary>
/// <remarks>
/// Binding never fails: an invalid alias would otherwise make the whole configuration invalid. Each alias is checked when
/// it's used, and its errors name the configuration file that declared it.
/// </remarks>
public sealed class ArtifactsConfiguration : IBicepArtifactsConfiguration, IWritableConfigurationSection
{
    public const string AliasesKey = "aliases";

    public const string OciAliasType = "oci";

    public const string TemplateSpecAliasType = "ts";

    public const string FileAliasType = "file";

    private const string TypeKey = "type";

    /// <summary>
    /// Defined in code rather than in the built-in configuration, so that an override replaces the whole entry instead of
    /// being merged with it. They don't depend on the cloud profile.
    /// </summary>
    public static readonly ImmutableDictionary<string, OciArtifactAlias> PredefinedAliases = new OciArtifactAlias[]
    {
        new("mar", "mcr.microsoft.com", "bicep"),
        new("avm", "mcr.microsoft.com", "bicep/avm"),
    }.ToImmutableDictionary(alias => alias.Name, StringComparer.Ordinal);

    public static readonly ArtifactsConfiguration Empty = new(null, null, null);

    /// <summary>
    /// Scheme names, which can't be alias names: <c>br::x</c> is almost certainly a mistyped <c>br:x</c>. Compared
    /// case-insensitively, so that <c>BR::x</c> isn't accepted either.
    /// </summary>
    public static readonly ImmutableHashSet<string> ReservedAliasNames = ImmutableHashSet.Create(
        StringComparer.OrdinalIgnoreCase,
        ArtifactSchemes.Br,
        ArtifactSchemes.Oci,
        ArtifactSchemes.TemplateSpecs,
        ArtifactSchemes.File,
        OciArtifactReferenceFacts.MockedScheme);


    private static readonly ConfigurationPropertyPath SectionPath = ConfigurationPropertyPath.Create(BicepConfiguration.ArtifactsKey);

    private static readonly ConfigurationPropertyPath AliasesPath = SectionPath.Append(AliasesKey);

    /// <summary>
    /// A problem with the definition of an alias, or with the section that contains it.
    /// </summary>
    /// <param name="Path">The invalid property.</param>
    /// <param name="DeclaringFileUri">The file that declared it.</param>
    private sealed record DefinitionError(ConfigurationPropertyPath Path, IOUri? DeclaringFileUri, string Reason);

    private readonly JsonElement? element;
    private readonly IOUri? configFileUri;
    private readonly ConfigurationProvenance? provenance;

    private ArtifactsConfiguration(JsonElement? element, IOUri? configFileUri, ConfigurationProvenance? provenance)
    {
        this.element = element;
        this.configFileUri = configFileUri;
        this.provenance = provenance;
    }

    public static ArtifactsConfiguration Bind(JsonElement element, IOUri? configFileUri) => new(element, configFileUri, null);

    /// <summary>
    /// Returns a copy that uses <paramref name="provenance"/> to find the file that declared each alias, so that file alias
    /// paths are resolved against it and errors name it. Without it, the leaf configuration file is used.
    /// </summary>
    public ArtifactsConfiguration WithProvenance(ConfigurationProvenance provenance) => new(this.element, this.configFileUri, provenance);

    public bool IsDeclared => this.element is not null;

    public ResultWithDiagnosticBuilder<ArtifactAlias> TryGetAlias(string aliasName)
    {
        // Checked first, so that defining the alias doesn't make it usable.
        if (ReservedAliasNames.Contains(aliasName))
        {
            return new(x => x.ReservedArtifactAliasName(aliasName));
        }

        if (!this.TryGetAliasesElement().IsSuccess(out var aliases, out var sectionError))
        {
            return Fail(sectionError);
        }

        if (aliases is { } definedAliases && definedAliases.TryGetProperty(aliasName, out var alias))
        {
            return this.BindAlias(aliasName, alias).IsSuccess(out var boundAlias, out var aliasError)
                ? new(boundAlias)
                : Fail(aliasError);
        }

        if (PredefinedAliases.TryGetValue(aliasName, out var predefinedAlias))
        {
            return new(predefinedAlias);
        }

        // A missing alias names the leaf configuration file, because that's where it would be added.
        return new(x => x.ArtifactAliasNotFound(aliasName, this.configFileUri));
    }

    public void WriteTo(Utf8JsonWriter writer)
    {
        if (this.element is { } section)
        {
            section.WriteTo(writer);
        }
        else
        {
            writer.WriteStartObject();
            writer.WriteEndObject();
        }
    }

    private static ResultWithDiagnosticBuilder<ArtifactAlias> Fail(DefinitionError error) =>
        new(x => x.InvalidArtifactAlias(error.Path.ToString(), error.DeclaringFileUri, error.Reason));

    /// <summary>
    /// Returns the <c>artifacts.aliases</c> object, or null if it isn't set.
    /// </summary>
    private Result<JsonElement?, DefinitionError> TryGetAliasesElement()
    {
        if (this.element is not { } section)
        {
            return new((JsonElement?)null);
        }

        if (section.ValueKind != JsonValueKind.Object)
        {
            return new(this.Invalid(SectionPath, "The value must be an object."));
        }

        if (!section.TryGetProperty(AliasesKey, out var aliases))
        {
            return new((JsonElement?)null);
        }

        if (aliases.ValueKind != JsonValueKind.Object)
        {
            return new(this.Invalid(AliasesPath, "The value must be an object."));
        }

        return new(aliases);
    }

    private Result<ArtifactAlias, DefinitionError> BindAlias(string aliasName, JsonElement alias)
    {
        var aliasPath = AliasesPath.Append(aliasName);

        if (alias.ValueKind != JsonValueKind.Object)
        {
            return new(this.Invalid(aliasPath, $"The value must be an object with a \"{TypeKey}\" property."));
        }

        if (!alias.TryGetProperty(TypeKey, out var typeElement) ||
            typeElement.ValueKind != JsonValueKind.String ||
            typeElement.GetString() is not { } type ||
            type is not (OciAliasType or TemplateSpecAliasType or FileAliasType))
        {
            return new(this.Invalid(aliasPath, $"The \"{TypeKey}\" property must be \"{OciAliasType}\", \"{TemplateSpecAliasType}\" or \"{FileAliasType}\"."));
        }

        if (PredefinedAliases.ContainsKey(aliasName) && type != OciAliasType)
        {
            return new(this.Invalid(aliasPath, $"\"{aliasName}\" is a predefined alias, so its \"{TypeKey}\" must be \"{OciAliasType}\"."));
        }

        switch (type)
        {
            case OciAliasType:
                if (!this.TryGetRequiredString(alias, aliasPath, "registry").IsSuccess(out var registry, out var error))
                {
                    return new(error);
                }

                if (this.TryGetOptionalString(alias, aliasPath, "repositoryPrefix", out var repositoryPrefix) is { } repositoryPrefixError)
                {
                    return new(repositoryPrefixError);
                }

                return new(new OciArtifactAlias(aliasName, registry, repositoryPrefix));

            case TemplateSpecAliasType:
                if (!this.TryGetRequiredString(alias, aliasPath, "subscriptionId").IsSuccess(out var subscriptionId, out error))
                {
                    return new(error);
                }

                if (!this.TryGetRequiredString(alias, aliasPath, "resourceGroup").IsSuccess(out var resourceGroup, out error))
                {
                    return new(error);
                }

                return new(new TemplateSpecArtifactAlias(aliasName, subscriptionId, resourceGroup));

            default:
                const string PathKey = "path";
                if (!this.TryGetRequiredString(alias, aliasPath, PathKey).IsSuccess(out var pathText, out error))
                {
                    return new(error);
                }

                if (!RelativePath.TryCreate(pathText).IsSuccess(out var path))
                {
                    return new(this.Invalid(aliasPath, $"The \"{PathKey}\" property must be a relative path that uses \"/\" as the separator, such as \"./modules\"."));
                }

                if (this.GetDeclaringFile(aliasPath.Append(PathKey)) is not { } declaringConfigUri)
                {
                    // Only reachable for a configuration that wasn't loaded from a file.
                    return new(this.Invalid(aliasPath, $"A \"{FileAliasType}\" alias must be declared in a Bicep configuration file."));
                }

                return new(new FileArtifactAlias(aliasName, path, declaringConfigUri));
        }
    }

    private Result<string, DefinitionError> TryGetRequiredString(JsonElement alias, ConfigurationPropertyPath aliasPath, string propertyName)
    {
        if (!alias.TryGetProperty(propertyName, out _))
        {
            return new(this.Invalid(aliasPath, $"The \"{propertyName}\" property must be specified."));
        }

        if (this.TryGetOptionalString(alias, aliasPath, propertyName, out var value) is { } error)
        {
            return new(error);
        }

        return new(value ?? throw new UnreachableException());
    }

    /// <returns>An error if the property is set to something other than a non-empty string.</returns>
    private DefinitionError? TryGetOptionalString(JsonElement alias, ConfigurationPropertyPath aliasPath, string propertyName, out string? value)
    {
        value = null;

        if (!alias.TryGetProperty(propertyName, out var element))
        {
            return null;
        }

        if (element.ValueKind != JsonValueKind.String || element.GetString() is not { Length: > 0 } text)
        {
            return this.Invalid(aliasPath.Append(propertyName), "The value must be a non-empty string.");
        }

        value = text;
        return null;
    }

    private IOUri? GetDeclaringFile(ConfigurationPropertyPath path) => this.provenance?.GetDeclaringFile(path) ?? this.configFileUri;

    private DefinitionError Invalid(ConfigurationPropertyPath path, string reason) => new(path, this.GetDeclaringFile(path), reason);
}