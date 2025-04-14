// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Extensions;
using Bicep.Core.Navigation;
using Bicep.Core.SourceGraph;
using Bicep.Core.Text;
using Bicep.Core.Utils;
using Uri = System.Uri;


namespace Bicep.Core.SourceLink;

public static class SourceCodeDocumentLinkHelper
{
    /// <summary>
    /// Retrieves all document links for module declaration syntax lines
    /// </summary>
    /// <param name="sourceFileGrouping"></param>
    /// <returns></returns>
    /// <example>
    ///   ```bicep
    ///   module mod1 'subfolder/module1.bicep' = {
    ///   ```bicep
    ///
    ///   will cause a DocumentLink from the source location 'subfolder/module1.bicep' to the source file
    ///   URI resolved from "subfolder/module1.bicep"
    /// </example>
    public static IImmutableDictionary<Uri, SourceCodeDocumentUriLink[]> GetAllModuleDocumentLinks(SourceFileGrouping sourceFileGrouping)
    {
        var dictionary = new Dictionary<Uri, SourceCodeDocumentUriLink[]>();

        foreach (var grouping in sourceFileGrouping.ArtifactLookup.Values.GroupBy(x => x.Origin))
        {
            var referencingFile = grouping.Key;
            var referencingFileLineStarts = referencingFile.LineStarts;
            var linksForReferencingFile = new List<SourceCodeDocumentUriLink>();

            foreach (var artifact in grouping)
            {
                if (artifact.Syntax is { } syntax &&
                    syntax.Path is { } &&
                    artifact.Result.IsSuccess(out var uri))
                {
                    var start = new TextPosition(TextCoordinateConverter.GetPosition(referencingFileLineStarts, syntax.Path.Span.Position));
                    var end = new TextPosition(TextCoordinateConverter.GetPosition(referencingFileLineStarts, syntax.Path.Span.Position + syntax.Path.Span.Length));

                    linksForReferencingFile.Add(new SourceCodeDocumentUriLink(new TextRange(start, end), uri));
                }
            }

            dictionary.Add(referencingFile.Uri, [.. linksForReferencingFile]);
        }

        return dictionary.ToImmutableDictionary();
    }
}
