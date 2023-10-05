// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Bicep.Core.Intermediate;

namespace Bicep.Core.IntegrationTests.Semantics;

public class ExpressionCollectorVisitor : ExpressionVisitor
{
    public record ExpressionItem(Expression Expression, ExpressionItem? Parent, int Depth)
    {
        public IEnumerable<ExpressionCollectorVisitor.ExpressionItem> GetAncestors()
        {
            var data = this;
            while (data.Parent is { } parent)
            {
                yield return parent;
                data = parent;
            }
        }
    }

    private readonly IList<ExpressionItem> expressionList = new List<ExpressionItem>();
    private ExpressionItem? parent = null;
    private int depth = 0;

    private ExpressionCollectorVisitor()
    {
    }

    public static ImmutableArray<ExpressionItem> Build(Expression expression)
    {
        var visitor = new ExpressionCollectorVisitor();
        visitor.Visit(expression);

        return visitor.expressionList.ToImmutableArray();
    }

    public static string GetExpressionLoggingString(
        ILookup<ExpressionItem?, ExpressionItem> expressionByParent,
        ExpressionItem item)
    {
        // Build a visual graph with lines to help understand the syntax hierarchy
        var graphPrefix = new StringBuilder();

        foreach (var ancestor in item.GetAncestors().Reverse().Skip(1))
        {
            var isLast = (ancestor.Depth > 0 && ancestor == expressionByParent[ancestor.Parent].Last());
            graphPrefix.Append(isLast switch
            {
                true => "  ",
                _ => "| ",
            });
        }

        if (item.Depth > 0)
        {
            var isLast = item == expressionByParent[item.Parent].Last();
            graphPrefix.Append(isLast switch
            {
                true => "└─",
                _ => "├─",
            });
        }

        var unparentedSuffix = item.Expression.SourceSyntax is null ? " [UNPARENTED]" : "";

        return $"{graphPrefix}{item.Expression.GetDebuggerDisplay()}{unparentedSuffix}";
    }

    protected override void VisitInternal(Expression expression)
    {
        var expressionItem = new ExpressionItem(Expression: expression, Parent: parent, Depth: depth);
        expressionList.Add(expressionItem);

        var prevParent = parent;
        parent = expressionItem;
        depth++;
        base.VisitInternal(expression);
        depth--;
        parent = prevParent;
    }
}
