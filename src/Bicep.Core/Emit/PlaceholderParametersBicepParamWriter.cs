// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Emit.Options;
using Bicep.Core.PrettyPrintV2;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;

namespace Bicep.Core.Emit
{
    public class PlaceholderParametersBicepParamWriter
    {
        private readonly SemanticModel semanticModel;

        public PlaceholderParametersBicepParamWriter(SemanticModel semanticModel, IncludeParamsOption includeParams)
        {
            this.semanticModel = semanticModel;
            this.IncludeParams = includeParams;
        }

        private IncludeParamsOption IncludeParams { get; }

        public void Write(TextWriter writer, string existingContent)
        {
            var bicepFileName = semanticModel.SourceFile.GetFileName();

            var allParameterDeclarations = semanticModel.Root.ParameterDeclarations;

            var filteredParameterDeclarations = this.IncludeParams == IncludeParamsOption.All ? allParameterDeclarations.Select(e => e.DeclaringParameter) : allParameterDeclarations.Where(e => e.DeclaringParameter.Modifier is not ParameterDefaultValueSyntax).Select(e => e.DeclaringParameter);

            var result = filteredParameterDeclarations
                            .OfType<ParameterDeclarationSyntax>()
                            .Select(e => new ParameterAssignmentSyntax([], e.Keyword, e.Name, SyntaxFactory.AssignmentToken, this.GetValueForParameter(e)))
                            .SelectMany(e => new List<SyntaxBase>() { e, SyntaxFactory.NewlineToken });

            var processedSyntaxList = new List<SyntaxBase>()
            {
                new UsingDeclarationSyntax([], SyntaxFactory.UsingKeywordToken, SyntaxFactory.CreateStringLiteral($"./{bicepFileName}"), SyntaxFactory.EmptySkippedTrivia),
                SyntaxFactory.NewlineToken,
                SyntaxFactory.NewlineToken
            }.Concat(result);

            var program = new ProgramSyntax(processedSyntaxList, SyntaxFactory.EndOfFileToken);

            var output = PrettyPrinterV2.PrintValid(program, PrettyPrinterV2Options.Default);

            writer.WriteLine(output);
        }

        private SyntaxBase GetValueForParameter(ParameterDeclarationSyntax syntax)
        {
            ObjectPropertySyntax GetObjectPropertySyntax(IdentifierSyntax? identifier, SyntaxBase type)
            {
                return SyntaxFactory.CreateObjectProperty(identifier?.IdentifierName ?? "", GetSyntaxForType(type, null));
            }

            ExpressionSyntax GetSyntaxForType(SyntaxBase type, SyntaxBase? decoratorSyntax)
            {
                return type switch
                {
                    TypeVariableAccessSyntax typeVariableAccessSyntax => typeVariableAccessSyntax.Name.IdentifierName switch
                    {
                        "int" => SyntaxFactory.CreateIntegerLiteral((decoratorSyntax as IntegerLiteralSyntax)?.Value ?? 0),
                        "bool" => SyntaxFactory.CreateBooleanLiteral(false),
                        "array" => SyntaxFactory.CreateArray([]),
                        "object" => SyntaxFactory.CreateObject([]),
                        _ => SyntaxFactory.CreateStringLiteral((decoratorSyntax as StringSyntax)?.SegmentValues.FirstOrDefault() ?? "")
                    },
                    ObjectTypeSyntax objectTypeSyntax => SyntaxFactory.CreateObject(objectTypeSyntax.Properties.Select(p => GetObjectPropertySyntax(p.Key as IdentifierSyntax, p.Value))),
                    _ => SyntaxFactory.CreateInvalidSyntaxWithComment($"TODO: fix the value assigned to this parameter `{type}`")
                };
            }

            var defaultValue = SyntaxHelper.TryGetDefaultValue(syntax);
            if (defaultValue != null)
            {
                switch (defaultValue)
                {
                    case FunctionCallSyntax defaultValueAsFunctionCall:
                        return CreateCommentSyntaxForFunctionCallSyntax(defaultValueAsFunctionCall.Name.IdentifierName);

                    case ArraySyntax defaultValueAsArray:
                        var items = defaultValueAsArray.Items.Select(item =>
                            item.Value is FunctionCallSyntax valueAsFunctionCall
                                ? SyntaxFactory.CreateArrayItem(CreateCommentSyntaxForFunctionCallSyntax(valueAsFunctionCall.Name.IdentifierName))
                                : item).ToList();
                        return SyntaxFactory.CreateArray(items);

                    case ObjectSyntax defaultValueAsObject:
                        return CheckFunctionCallsInObjectSyntax(defaultValueAsObject);

                    default:
                        return defaultValue;
                }
            }

            return syntax.Type switch
            {
                ObjectTypeSyntax objectType => SyntaxFactory.CreateObject(objectType.Properties.Select(e => GetObjectPropertySyntax(e.Key as IdentifierSyntax, e.Value))),
                TypeVariableAccessSyntax typeVar =>
                    GetSyntaxForType(
                        typeVar,
                        syntax.Decorators
                            .Where(d => d.Expression is FunctionCallSyntax f && f.Name.IdentifierName == "allowed")
                            .SelectMany(d => d.Arguments)
                            .Select(arg => (arg.Expression as ArraySyntax)?.Items.FirstOrDefault()?.Value)
                            .FirstOrDefault()
                    ),
                _ => SyntaxFactory.NewlineToken
            };
        }

        private SyntaxBase CheckFunctionCallsInObjectSyntax(ObjectSyntax objectSyntax)
        {
            var value = objectSyntax.Properties.Select(e =>
            {
                if (e.Value is FunctionCallSyntax valueAsFunctionCallSyntax)
                {
                    return SyntaxFactory.CreateObjectProperty((e.Key as IdentifierSyntax)?.IdentifierName ?? "", CreateCommentSyntaxForFunctionCallSyntax(valueAsFunctionCallSyntax.Name.IdentifierName));
                }
                else if (e.Value is ArraySyntax valueAsFunctionArraySyntax)
                {
                    var value = valueAsFunctionArraySyntax.Items.Select(f =>
                    {
                        if (f.Value is FunctionCallSyntax valueAsFunctionCallSyntax)
                        {
                            return SyntaxFactory.CreateArrayItem(CreateCommentSyntaxForFunctionCallSyntax(valueAsFunctionCallSyntax.Name.IdentifierName));
                        }

                        return f;
                    }).ToList();

                    return SyntaxFactory.CreateObjectProperty((e.Key as IdentifierSyntax)?.IdentifierName ?? "", SyntaxFactory.CreateArray(value));
                }
                else if (e.Value is ObjectSyntax valueAsObjectSyntax)
                {
                    var syntax = CheckFunctionCallsInObjectSyntax(valueAsObjectSyntax);
                    var item = SyntaxFactory.CreateObjectProperty((e.Key as IdentifierSyntax)?.IdentifierName ?? "", syntax);
                    return item;
                }

                return e;
            }).ToList();

            return SyntaxFactory.CreateObject(value);
        }

        private SyntaxBase CreateCommentSyntaxForFunctionCallSyntax(string functionName)
        {
            return SyntaxFactory.CreateInvalidSyntaxWithComment($" TODO : please fix the value assigned to this parameter `{functionName}()` ");
        }
    }
}
