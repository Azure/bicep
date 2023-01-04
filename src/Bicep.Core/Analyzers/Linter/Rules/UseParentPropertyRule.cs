// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Comparers;
using Bicep.Core.TypeSystem;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class UseParentPropertyRule : LinterRuleBase
    {
        public new const string Code = "use-parent-property";

        public UseParentPropertyRule() : base(
            code: Code,
            description: CoreResources.UseParentPropertyRuleDescription,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"),
            diagnosticLevel: DiagnosticLevel.Warning)
        { }

        public override string FormatMessage(params object[] values)
            => string.Format(CoreResources.UseParentPropertyRule_MessageFormat, values);

        override public IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
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

                var parentType = resource.TypeReference.TypeSegments
                    .Take(resource.TypeReference.TypeSegments.Length - 1)
                    .ConcatStrings("/");

                foreach (var parentResource in typeLookup[parentType])
                {
                    if (!parentResource.Symbol.IsCollection &&
                        parentResource.TryGetNameSyntax() is {} parentNameSyntax &&
                        TryGetParentName(model, childName, parentNameSyntax) is {} replacement &&
                        TryCreateDiagnostic(diagnosticLevel, parentResource, resource, replacement) is {} diagnostic)
                    {
                        yield return diagnostic;
                    }
                }
            }
        }

        private static IEnumerable<string> RemoveFirstLeadingSlash(IEnumerable<string> segments)
            => segments.Select((x, i) => i == 0 ? x.Substring(1) : x);

        private SyntaxBase? TryGetParentName(SemanticModel model, StringSyntax parentName, StringSyntax childName)
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
                new [] { newFirstChildSegment }.Concat(childName.SegmentValues.Skip(parentName.SegmentValues.Length)),
                childName.Expressions.Skip(parentName.Expressions.Length));

            if (SimplifyInterpolationRule.TrySimplify(newString) is {} simplfied &&
                model.GetTypeInfo(simplfied) is {} simplfiedType &&
                TypeValidator.AreTypesAssignable(simplfiedType, LanguageConstants.String))
            {
                // Check if we can simplify "name: '${expr}'" to "name: expr"
                return simplfied;
            }

            return newString;
        }

        private SyntaxBase? TryGetParentName(SemanticModel model, StringSyntax childName, SyntaxBase parentName)
        {
            if (parentName is not StringSyntax parentNameString)
            {
                if (childName.SegmentValues.Length > 1 &&
                    childName.SegmentValues.Skip(1).First().StartsWith('/') &&
                    childName.Expressions.First() is {} firstExpr &&
                    SyntaxIgnoringTriviaComparer.Instance.Equals(firstExpr, parentName))
                {
                    return SyntaxFactory.CreateString(
                        RemoveFirstLeadingSlash(childName.SegmentValues.Skip(1)),
                        childName.Expressions.Skip(1));
                }

                return null;
            }

            return TryGetParentName(model, parentNameString, childName);
        }

        private IDiagnostic? TryCreateDiagnostic(DiagnosticLevel diagnosticLevel, DeclaredResourceMetadata parentResource, DeclaredResourceMetadata childResource, SyntaxBase replacementName)
        {
            if (childResource.Symbol.DeclaringResource.TryGetBody() is not {} body ||
                body.TryGetPropertyByName("name") is not {} nameProp)
            {
                return null;
            }

            var updatedBody = SyntaxModifier.TryAddProperty(
                body,
                SyntaxFactory.CreateObjectProperty(
                    "parent",
                    SyntaxFactory.CreateIdentifier(parentResource.Symbol.NameIdentifier.IdentifierName)),
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

            var codeReplacement = new CodeReplacement(body.Span, updatedBody.ToTextPreserveFormatting());

            return CreateFixableDiagnosticForSpan(
                diagnosticLevel,
                nameProp.Value.Span,
                new CodeFix(CoreResources.UseParentPropertyRuleCodeFix, isPreferred: true, CodeFixKind.QuickFix, codeReplacement),
                childResource.Symbol.NameIdentifier.IdentifierName,
                parentResource.Symbol.NameIdentifier.IdentifierName);
        }
    }
}
