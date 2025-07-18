// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Comparers;
using Bicep.Core.TypeSystem;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.Analyzers.Linter.Rules;

public sealed class UseParentPropertyRule : LinterRuleBase
{
    public new const string Code = "use-parent-property";

    public UseParentPropertyRule() : base(
        code: Code,
        description: CoreResources.UseParentPropertyRule_Description,
        LinterRuleCategory.BestPractice)
    { }

    public override string FormatMessage(params object[] values)
        => string.Format(CoreResources.UseParentPropertyRule_MessageFormat, values);

    public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
    {
        var allResources = model.AllResources.OfType<DeclaredResourceMetadata>().Where(x => x.IsAzResource).ToArray();
        var typeLookup = allResources.ToLookup(x => x.TypeReference.FormatType(), StringComparer.OrdinalIgnoreCase);

        foreach (var resource in allResources)
        {
            if (resource.TypeReference.TypeSegments.Length < 2 ||
                resource.Parent is not null ||
                resource.TryGetNameSyntax() is not StringSyntax childName)
            {
                continue;
            }

            if (TryGetReplacementChildName(model, resource, childName) is { } info &&
                TryCreateDiagnostic(model, diagnosticLevel, info.parent, resource, info.name) is { } nameDiagnostic)
            {
                yield return nameDiagnostic;
                continue;
            }

            var parentType = resource.TypeReference.TypeSegments
                .Take(resource.TypeReference.TypeSegments.Length - 1)
                .ConcatStrings("/");

            foreach (var parentResource in typeLookup[parentType])
            {
                if (!parentResource.Symbol.IsCollection &&
                    parentResource.TryGetNameSyntax() is { } parentNameSyntax &&
                    TryGetReplacementChildName(model, parentNameSyntax, childName) is { } replacement &&
                    TryCreateDiagnostic(model, diagnosticLevel, parentResource, resource, replacement) is { } diagnostic)
                {
                    yield return diagnostic;
                    break;
                }
            }
        }
    }

    private static IEnumerable<string> RemoveFirstLeadingSlash(IEnumerable<string> segments)
        => segments.Select((x, i) => i == 0 ? x.Substring(1) : x);

    private SyntaxBase? TryGetReplacementChildName(SemanticModel model, StringSyntax parentName, StringSyntax childName)
    {
        /* Handle cases such as:
            * parent: 'abc', child: 'abc/def'
            * parent: '${abc}', child: '${abc}/${def}'
            * parent: '${abc}', child: '${abc}/def'
            * parent: '${abc}-foo', child: '${abc}-foo/bar'
        */

        if (childName.SegmentValues.Length < parentName.SegmentValues.Length ||
            childName.Expressions.Length < parentName.Expressions.Length)
        {
            // child name must have more or equal string segments and expressions
            return null;
        }

        for (var i = 0; i < parentName.SegmentValues.Length; i++)
        {
            var isFinalCommonSegment = (i == parentName.SegmentValues.Length - 1);
            var parentSegment = parentName.SegmentValues[i];
            var childSegment = childName.SegmentValues[i];

            // All common string segments (apart from the final common string segment) must match the parent string segments EXACTLY
            if (!isFinalCommonSegment &&
                childSegment != parentSegment)
            {
                return null;
            }

            // The final common segment must either match exactly, or be equal to <parent_segment>/<something>
            if (isFinalCommonSegment &&
                (childSegment.Length <= parentSegment.Length ||
                childSegment[parentSegment.Length] != '/' ||
                childSegment.IndexOf(parentSegment) != 0))
            {
                return null;
            }
        }

        // Use everything after the final parent segment and trailing '/' for the first segment of the new string
        var newFirstChildSegment = childName.SegmentValues[parentName.SegmentValues.Length - 1].Substring(parentName.SegmentValues.Last().Length + 1);

        if (!Enumerable.SequenceEqual(
            childName.Expressions.Take(parentName.Expressions.Length),
            parentName.Expressions,
            SyntaxIgnoringTriviaComparer.Instance))
        {
            // the expressions must all match.
            //this check is expensive - only run if everything else looks ok.
            return null;
        }

        var newString = SyntaxFactory.CreateString(
            new[] { newFirstChildSegment }.Concat(childName.SegmentValues.Skip(parentName.SegmentValues.Length)),
            childName.Expressions.Skip(parentName.Expressions.Length));

        if (SimplifyInterpolationRule.TrySimplify(newString) is { } simplfied &&
            model.GetTypeInfo(simplfied) is { } simplfiedType &&
            TypeValidator.AreTypesAssignable(simplfiedType, LanguageConstants.String))
        {
            // Check if we can simplify "name: '${expr}'" to "name: expr"
            return simplfied;
        }

        return newString;
    }

    private SyntaxBase? TryGetReplacementChildName(SemanticModel model, SyntaxBase parentName, StringSyntax childName)
    {
        if (parentName is not StringSyntax parentNameString)
        {
            if (childName.SegmentValues.Length > 1 &&
                childName.SegmentValues.Skip(1).First().StartsWith('/') &&
                childName.Expressions.First() is { } firstExpr &&
                SyntaxIgnoringTriviaComparer.Instance.Equals(firstExpr, parentName))
            {
                return SyntaxFactory.CreateString(
                    RemoveFirstLeadingSlash(childName.SegmentValues.Skip(1)),
                    childName.Expressions.Skip(1));
            }

            return null;
        }

        return TryGetReplacementChildName(model, parentNameString, childName);
    }

    private (SyntaxBase name, DeclaredResourceMetadata parent)? TryGetReplacementChildName(SemanticModel model, DeclaredResourceMetadata child, StringSyntax childName)
    {
        if (childName.SegmentValues.Length > 1 &&
            childName.SegmentValues[1].StartsWith('/') &&
            childName.Expressions[0] is PropertyAccessSyntax nameProp &&
            nameProp.PropertyName.NameEquals("name") &&
            model.ResourceMetadata.TryLookup(nameProp.BaseExpression) is DeclaredResourceMetadata nameResource &&
            nameResource.TypeReference.IsParentOf(child.TypeReference))
        {
            var name = SyntaxFactory.CreateString(
                RemoveFirstLeadingSlash(childName.SegmentValues.Skip(1)),
                childName.Expressions.Skip(1));

            return (name, nameResource);
        }

        return null;
    }

    private IDiagnostic? TryCreateDiagnostic(SemanticModel model, DiagnosticLevel diagnosticLevel, DeclaredResourceMetadata parentResource, DeclaredResourceMetadata childResource, SyntaxBase replacementName)
    {
        if (childResource.Symbol.DeclaringResource.TryGetBody() is not { } body ||
            body.TryGetPropertyByName("name") is not { } nameProp)
        {
            return null;
        }

        var updatedBody = SyntaxModifier.TryAddProperty(
            body,
            SyntaxFactory.CreateObjectProperty(
                "parent",
                SyntaxFactory.CreateIdentifier(parentResource.Symbol.NameIdentifier.IdentifierName)),
            model.ParsingErrorLookup,
            atIndex: body.Properties.IndexOf(x => x == nameProp));
        if (updatedBody is null)
        {
            return null;
        }

        updatedBody = SyntaxModifier.TryUpdatePropertyValue(updatedBody, "name", prop => replacementName);
        if (updatedBody is null)
        {
            return null;
        }

        var codeReplacement = new CodeReplacement(body.Span, updatedBody.ToString());

        return CreateFixableDiagnosticForSpan(
            diagnosticLevel,
            nameProp.Value.Span,
            new CodeFix(CoreResources.UseParentPropertyRule_CodeFix, isPreferred: true, CodeFixKind.QuickFix, codeReplacement),
            childResource.Symbol.NameIdentifier.IdentifierName,
            parentResource.Symbol.NameIdentifier.IdentifierName);
    }
}
