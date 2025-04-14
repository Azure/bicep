// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Text;

namespace Bicep.Core.SourceLink;

/// <summary>
/// Represents a link from a range inside a source file that points to another source file
/// </summary>
/// <typeparam name="TTarget"></typeparam>
/// <param name="Range">Span of the origin of this link in the source file (e.g. the module path of a module declaration syntax line)</param>
/// <param name="Target">The target file for this link (e.g. the path of the source file pointed to by the module path inside the source.tgz file)</param>
public record SourceCodeDocumentLink<TTarget>(
    TextRange Range,
    TTarget Target
);

// Refers to the target via its original file URI
public record SourceCodeDocumentUriLink : SourceCodeDocumentLink<Uri>
{
    public SourceCodeDocumentUriLink(
        TextRange Range,
        Uri Target)
    : base(Range, Target)
    { }
}

// Refers to the target via the relative path used inside a source.tgz file
public record SourceCodeDocumentPathLink : SourceCodeDocumentLink<string>
{
    public SourceCodeDocumentPathLink(
        TextRange Range,
        string Target)
    : base(Range, Target)
    { }
}
