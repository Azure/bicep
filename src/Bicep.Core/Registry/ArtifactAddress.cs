// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Registry;

/// <summary>
/// The syntactic form of an artifact reference string. Parsing is purely textual: it does not consult configuration,
/// registries or the file system, and it never fails. Validation happens when the address is resolved.
/// </summary>
public abstract record ArtifactAddress(string Text)
{
    /// <summary>
    /// A reference without a scheme, e.g. <c>./modules/storage.bicep</c>.
    /// </summary>
    public sealed record Relative(string Text) : ArtifactAddress(Text);

    /// <summary>
    /// A reference with a scheme, e.g. <c>br:contoso.azurecr.io/storage:1.0.0</c>.
    /// </summary>
    public sealed record Qualified(string Text, string Scheme, string Body) : ArtifactAddress(Text);

    /// <summary>
    /// A reference with a scheme-qualified alias that is resolved through <c>moduleAliases</c>, e.g. <c>br/contoso:storage:1.0.0</c>.
    /// </summary>
    public sealed record SchemeQualifiedAlias(string Text, string Scheme, string Alias, string Body) : ArtifactAddress(Text);

    public static ArtifactAddress Parse(string text)
    {
        // The first ':' separates the scheme (optionally qualified with "/<alias>") from the body.
        var colonIndex = text.IndexOf(':');
        if (colonIndex < 0)
        {
            return new Relative(text);
        }

        var prefix = text[..colonIndex];
        var body = text[(colonIndex + 1)..];

        var slashIndex = prefix.IndexOf('/');
        if (slashIndex < 0)
        {
            return new Qualified(text, prefix, body);
        }

        return new SchemeQualifiedAlias(text, prefix[..slashIndex], prefix[(slashIndex + 1)..], body);
    }
}
