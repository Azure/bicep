// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Registry;

/// <summary>
/// The syntactic form of an artifact reference string. Parsing is purely textual: it does not consult configuration,
/// registries or the file system, and it never fails. Validation happens when the address is resolved.
/// </summary>
/// <remarks>
/// "Qualified" names what precedes the body (a scheme, an alias, or both), not whether the body is complete:
/// an alias-qualified body is only complete after the alias is expanded.
/// </remarks>
public abstract record ArtifactAddress(string Text)
{
    /// <summary>
    /// A path without a scheme or alias, e.g. <c>./modules/storage.bicep</c>.
    /// </summary>
    public sealed record Path(string Text) : ArtifactAddress(Text);

    /// <summary>
    /// A body qualified by a scheme, e.g. <c>br:contoso.azurecr.io/storage:1.0.0</c>.
    /// </summary>
    public sealed record SchemeQualified(string Text, string Scheme, string Body) : ArtifactAddress(Text);

    /// <summary>
    /// A body qualified by a scheme and an alias, which is resolved through <c>moduleAliases</c>, e.g. <c>br/contoso:storage:1.0.0</c>.
    /// </summary>
    public sealed record SchemeAndAliasQualified(string Text, string Scheme, string Alias, string Body) : ArtifactAddress(Text);

    public static ArtifactAddress Parse(string text)
    {
        // The first ':' separates the scheme (optionally followed by "/<alias>") from the body.
        var colonIndex = text.IndexOf(':');
        if (colonIndex < 0)
        {
            return new Path(text);
        }

        var prefix = text[..colonIndex];
        var body = text[(colonIndex + 1)..];

        var slashIndex = prefix.IndexOf('/');
        if (slashIndex < 0)
        {
            return new SchemeQualified(text, prefix, body);
        }

        return new SchemeAndAliasQualified(text, prefix[..slashIndex], prefix[(slashIndex + 1)..], body);
    }
}
