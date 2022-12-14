// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Bicep.Core.SourceGenerators;

[Generator]
public class ExpressionTreeGenerator : ISourceGenerator
{
    public class ExpressionsEntryField
    {
        [JsonConstructor]
        public ExpressionsEntryField(string name, string type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }
        public string Type { get; }
    }

    public class ExpressionsEntry
    {
        [JsonConstructor]
        public ExpressionsEntry(string name, ImmutableArray<ExpressionsEntryField> fields)
        {
            Name = name;
            Fields = fields;
        }

        public string Name { get; }
        public ImmutableArray<ExpressionsEntryField> Fields { get; }
    }

    public class ExpressionsFile
    {
        [JsonConstructor]
        public ExpressionsFile(ImmutableArray<ExpressionsEntry> entries)
        {
            Entries = entries;
        }

        public ImmutableArray<ExpressionsEntry> Entries { get; }
    }

    public void Initialize(GeneratorInitializationContext context) {}

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.AdditionalFiles.FirstOrDefault(x => Path.GetFileName(x.Path) == "expressions.json") is not {} inputFile ||
            inputFile.GetText() is not {} inputText)
        {
            return;
        }

        var file = JsonSerializer.Deserialize<ExpressionsFile>(inputText.ToString(), new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReadCommentHandling = JsonCommentHandling.Skip,
        }) ?? throw new InvalidOperationException($"Failed to deserialize {inputFile.Path}");

        context.AddSource("Expressions.cs", GenerateExpressions(file));
        context.AddSource("IExpressionVisitor.cs", GenerateIExpressionVisitor(file));
        context.AddSource("ExpressionVisitor.cs", GenerateExpressionVisitor(file));
        context.AddSource("ExpressionRewriteVisitor.cs", GenerateExpressionRewriteVisitor(file));
    }

    private static SourceText GenerateExpressions(ExpressionsFile file)
    {
        var sb = new StringBuilder();
        sb.Append($@"
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;

namespace Bicep.Core.Intermediate;
");
        foreach (var entry in file.Entries.OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase))
        {
            sb.Append($@"
public record {entry.Name}Expression(");
            for (var i = 0; i < entry.Fields.Length; i++)
            {
                var field = entry.Fields[i];
                sb.Append($@"
    {field.Type} {field.Name}");
                
                if (i < entry.Fields.Length - 1)
                {
                    sb.Append(",");
                }
            }

            sb.Append($@"
) : Expression
{{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.Visit{entry.Name}Expression(this);
}}
");
        }

        return SourceText.From(sb.ToString(), Encoding.UTF8);
    }

    private static SourceText GenerateIExpressionVisitor(ExpressionsFile file)
    {
        var sb = new StringBuilder();
        sb.Append($@"
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Bicep.Core.Intermediate;

public partial interface IExpressionVisitor
{{
");
        foreach (var entry in file.Entries.OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase))
        {
            sb.Append($@"
    void Visit{entry.Name}Expression({entry.Name}Expression expression);
");
        }

        sb.Append($@"
}}
");

        return SourceText.From(sb.ToString(), Encoding.UTF8);
    }

    private static SourceText GenerateExpressionVisitor(ExpressionsFile file)
    {
        var sb = new StringBuilder();
        sb.Append($@"
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Bicep.Core.Intermediate;

public abstract partial class ExpressionVisitor : IExpressionVisitor
{{
");
        foreach (var entry in file.Entries.OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase))
        {
            sb.Append($@"
    void IExpressionVisitor.Visit{entry.Name}Expression({entry.Name}Expression expression)
    {{
");
            foreach (var field in entry.Fields.Where(ShouldVisit))
            {
                    sb.Append($@"
        Visit(expression.{field.Name});");
            }
            sb.Append($@"
    }}
");
        }
            sb.Append($@"
}}
");

        return SourceText.From(sb.ToString(), Encoding.UTF8);
    }

    private static SourceText GenerateExpressionRewriteVisitor(ExpressionsFile file)
    {
        var sb = new StringBuilder();
        sb.Append($@"
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Bicep.Core.Intermediate;

public abstract partial class ExpressionRewriteVisitor : IExpressionVisitor
{{
");
        foreach (var entry in file.Entries.OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase))
        {
            sb.Append($@"
    void IExpressionVisitor.Visit{entry.Name}Expression({entry.Name}Expression expression) => ReplaceCurrent(expression, Replace{entry.Name}Expression);            
    public virtual Expression Replace{entry.Name}Expression({entry.Name}Expression expression)
    {{
");
            var fieldsToVisit = entry.Fields.Where(ShouldVisit).ToImmutableArray();

            if (!fieldsToVisit.Any())
            {
                sb.Append($@"
        return expression;
");
            }
            else
            {
                sb.Append($@"
        var hasChanges = false;");

                for (var i = 0; i < fieldsToVisit.Length; i++)
                {
                    var field = fieldsToVisit[i];
                    sb.Append($@"
        hasChanges |= TryRewrite(expression.{field.Name}, out var rw_{field.Name});");
                }

                var newFields = entry.Fields.Select(x => ShouldVisit(x) ? $"rw_{x.Name}" : $"expression.{x.Name}");

                sb.Append($@"
        return hasChanges ? new {entry.Name}Expression({string.Join(", ", newFields)}) : expression;");
            }

            sb.Append($@"
    }}
");
        }
            sb.Append($@"
}}
");

        return SourceText.From(sb.ToString(), Encoding.UTF8);
    }

    private static bool ShouldVisit(ExpressionsEntryField field)
        => field.Type == "Expression" || field.Type == "ImmutableArray<Expression>";
}