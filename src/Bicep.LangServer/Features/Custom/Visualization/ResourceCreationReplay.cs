// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.Text;
using Bicep.LanguageServer.Compilation;
using Bicep.LanguageServer.Extensions;

namespace Bicep.LanguageServer.Features.Custom.Visualization
{
    /// <summary>
    /// Prepares the edits that undo and redo a resource creation made by the visual designer.
    /// <para>
    /// A replay is offered only when it exactly reverses or restores the creation, whatever else changed in the file:
    /// undo requires the declaration to be byte-for-byte what the designer inserted and unreferenced, so removing it
    /// cannot discard user edits or break other declarations; redo requires its symbolic name to still be free.
    /// </para>
    /// </summary>
    internal static class ResourceCreationReplay
    {
        public static PrepareVisualResourceReplayResult Prepare(CompilationContext context, PrepareVisualResourceReplayParams request)
        {
            if (context.SourceFileKind != BicepSourceFileKind.BicepFile)
            {
                return new([.. request.Replays.Select(replay => new VisualResourceReplayEdit(replay.OperationId, null))]);
            }

            var model = context.Compilation.GetEntrypointSemanticModel();
            var sourceText = model.SourceFile.Text;

            return new([.. request.Replays.Select(replay => new VisualResourceReplayEdit(replay.OperationId, replay.Direction switch
            {
                VisualResourceReplayDirection.Undo => PrepareUndo(context, model, sourceText, replay),
                VisualResourceReplayDirection.Redo => PrepareRedo(context, model, replay),
                _ => null,
            }))]);
        }

        private static VisualResourceReplayTextEdit? PrepareUndo(CompilationContext context, SemanticModel model, string sourceText, VisualResourceReplayQuery replay)
        {
            var (leading, declarationText, trailing) = SplitInsertedText(replay.InsertedText);
            var declarations = model.Root.Declarations
                .Where(declaration => LanguageConstants.IdentifierComparer.Equals(declaration.Name, replay.NodeId))
                .ToArray();

            if (declarationText.Length == 0 || declarations is not [ResourceSymbol resource])
            {
                return null;
            }

            var span = resource.DeclaringResource.Span;
            if (!string.Equals(sourceText.Substring(span.Position, span.Length), declarationText, StringComparison.Ordinal))
            {
                return null;
            }

            // The declaration itself is always one of the results.
            if (model.FindReferences(resource).Any(reference => !ReferenceEquals(reference, resource.DeclaringSyntax)))
            {
                return null;
            }

            // Also remove the blank lines inserted with the declaration, where they are still next to it.
            var start = span.Position;
            var end = span.GetEndPosition();
            if (HasTextAt(sourceText, start - leading.Length, leading))
            {
                start -= leading.Length;
            }
            if (HasTextAt(sourceText, end, trailing))
            {
                end += trailing.Length;
            }

            return new(new TextSpan(start, end - start).ToRange(context.LineStarts), string.Empty);
        }

        private static VisualResourceReplayTextEdit? PrepareRedo(CompilationContext context, SemanticModel model, VisualResourceReplayQuery replay)
        {
            var (_, declarationText, _) = SplitInsertedText(replay.InsertedText);

            // Case-insensitive, like the collision check that picked the name when the resource was created.
            if (declarationText.Length == 0 ||
                model.Root.Declarations.Any(declaration => string.Equals(declaration.Name, replay.NodeId, StringComparison.OrdinalIgnoreCase)))
            {
                return null;
            }

            var insertion = GeneratedResourceDeclaration.CreateInsertionEdit(context, declarationText);

            return new(insertion.Range, insertion.NewText);
        }

        private static (string Leading, string Declaration, string Trailing) SplitInsertedText(string insertedText)
        {
            var declaration = insertedText.Trim('\r', '\n');
            var leadingLength = insertedText.Length - insertedText.TrimStart('\r', '\n').Length;

            return (
                insertedText[..leadingLength],
                declaration,
                insertedText[(leadingLength + declaration.Length)..]);
        }

        private static bool HasTextAt(string sourceText, int offset, string text) =>
            offset >= 0 &&
            offset + text.Length <= sourceText.Length &&
            string.CompareOrdinal(sourceText, offset, text, 0, text.Length) == 0;
    }
}
