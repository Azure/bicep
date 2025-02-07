// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Emit.Options;
using Bicep.Core.PrettyPrintV2;
using Bicep.Core.Semantics;
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
            var bicepFileName = Path.GetFileName(semanticModel.SourceFile.Uri.LocalPath);

            var allParameterDeclarations = semanticModel.Root.ParameterDeclarations;

            var filteredParameterDeclarations = this.IncludeParams == IncludeParamsOption.All ? allParameterDeclarations.Select(e => e.DeclaringParameter) : allParameterDeclarations.Where(e => e.DeclaringParameter.Modifier is not ParameterDefaultValueSyntax).Select(e => e.DeclaringParameter);

            var result = filteredParameterDeclarations
                            .OfType<ParameterDeclarationSyntax>()
                            .Select(e => new ParameterAssignmentSyntax(e.Keyword, e.Name, SyntaxFactory.AssignmentToken, GetValueForParameter(e)))
                            .SelectMany(e => new List<SyntaxBase>() { e, SyntaxFactory.NewlineToken });

            var processedSyntaxList = new List<SyntaxBase>()
            {
                new UsingDeclarationSyntax(SyntaxFactory.UsingKeywordToken, SyntaxFactory.CreateStringLiteral($"./{bicepFileName}")),
                SyntaxFactory.NewlineToken,
                SyntaxFactory.NewlineToken
            }.Concat(result);

            var program = new ProgramSyntax(processedSyntaxList, SyntaxFactory.EndOfFileToken);

            var output = PrettyPrinterV2.PrintValid(program, PrettyPrinterV2Options.Default);

            writer.WriteLine(output);
        }

        private static SyntaxBase CheckFunctionCallsInObjectSyntax(ObjectSyntax objectSyntax)
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

        private static SyntaxBase CreateCommentSyntaxForFunctionCallSyntax(string functionName, bool isJsonParamWriter = false)
        {
            if (isJsonParamWriter)
            {
                return SyntaxFactory.CreateStringLiteral("");
            }
            else
            {
                return SyntaxFactory.CreateInvalidSyntaxWithComment($" TODO : please fix the value assigned to this parameter `{functionName}()` ");
            }
        }

        public static SyntaxBase GetValueForParameter(ParameterDeclarationSyntax syntax, bool isJsonParamWriter = false)
        {
            var defaultValue = SyntaxHelper.TryGetDefaultValue(syntax);
            if (defaultValue != null)
            {
                if (defaultValue is FunctionCallSyntax defaultValueAsFunctionCall)
                {
                    return CreateCommentSyntaxForFunctionCallSyntax(defaultValueAsFunctionCall.Name.IdentifierName, isJsonParamWriter);
                }
                else if (defaultValue is ArraySyntax defaultValueAsArray)
                {
                    var value = defaultValueAsArray.Items.Select(e =>
                    {
                        if (e.Value is FunctionCallSyntax valueAsFunctionCall)
                        {
                            return SyntaxFactory.CreateArrayItem(CreateCommentSyntaxForFunctionCallSyntax(valueAsFunctionCall.Name.IdentifierName, isJsonParamWriter)).Value;
                        }

                        return e.Value;
                    }).ToList();

                    return SyntaxFactory.CreateArray(value);
                }
                else if (defaultValue is ObjectSyntax defaultValueAsObject)
                {
                    return CheckFunctionCallsInObjectSyntax(defaultValueAsObject);
                }

                return defaultValue;
            }

            if (syntax.Type is TypeVariableAccessSyntax variableAccessSyntax)
            {
                var allowedDecorator = syntax.Decorators.Where(e => e.Expression is FunctionCallSyntax functionCallSyntax && functionCallSyntax.Name.IdentifierName == "allowed").Select(e => e.Arguments).FirstOrDefault();

                var allowedDecoratorFirstItem = (allowedDecorator?.First().Expression as ArraySyntax)?.Items.First().Value;

                switch (variableAccessSyntax.Name.IdentifierName)
                {
                    case "int":
                        return SyntaxFactory.CreateIntegerLiteral((allowedDecoratorFirstItem as IntegerLiteralSyntax)?.Value ?? 0);
                    case "bool":
                        return SyntaxFactory.CreateBooleanLiteral(false);
                    case "array":
                        return SyntaxFactory.CreateArray([]);
                    case "object":
                        return SyntaxFactory.CreateObject([]);
                    case "string":
                    default:
                        return SyntaxFactory.CreateStringLiteral((allowedDecoratorFirstItem as StringSyntax)?.SegmentValues.First() ?? "");
                }
            }

            return SyntaxFactory.NewlineToken;
        }
    }
}
