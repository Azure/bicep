// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Globalization;
using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Rewriters;
using Newtonsoft.Json.Linq;

namespace Bicep.Cli.Helpers;

public static class ParamsFileHelper
{
    private static readonly ImmutableHashSet<JTokenType> SupportedJsonTokenTypes = [JTokenType.Object, JTokenType.Array, JTokenType.String, JTokenType.Integer, JTokenType.Float, JTokenType.Boolean, JTokenType.Null];

    private static SyntaxBase ConvertJsonToBicepSyntax(JToken token) =>
    token switch
    {
        JObject @object => SyntaxFactory.CreateObject(@object.Properties().Where(x => SupportedJsonTokenTypes.Contains(x.Value.Type)).Select(x => SyntaxFactory.CreateObjectProperty(x.Name, ConvertJsonToBicepSyntax(x.Value)))),
        JArray @array => SyntaxFactory.CreateArray(@array.Where(x => SupportedJsonTokenTypes.Contains(x.Type)).Select(ConvertJsonToBicepSyntax)),
        JValue value => value.Type switch
        {
            JTokenType.String => SyntaxFactory.CreateStringLiteral(value.ToString(CultureInfo.InvariantCulture)),
            JTokenType.Integer => SyntaxFactory.CreatePositiveOrNegativeInteger(value.Value<long>()),
            // Floats are currently not supported in Bicep, so fall back to the default behavior of "any"
            JTokenType.Float => SyntaxFactory.CreateFunctionCall("json", SyntaxFactory.CreateStringLiteral(value.ToObject<double>().ToString(CultureInfo.InvariantCulture))),
            JTokenType.Boolean => SyntaxFactory.CreateBooleanLiteral(value.ToObject<bool>()),
            JTokenType.Null => SyntaxFactory.CreateNullLiteral(),
            _ => throw new InvalidOperationException($"Cannot parse JSON object. Unsupported value token type: {value.Type}"),
        },
        _ => throw new InvalidOperationException($"Cannot parse JSON object. Unsupported token: {token.Type}")
    };

    private class InlineReplacementRewriter : SyntaxRewriteVisitor
    {
        private readonly Dictionary<string, JToken> parameters;
        private readonly HashSet<string> replacedParameters;
        private string? currentParameterName;
        private string? currentPropertyName;

        public InlineReplacementRewriter(Dictionary<string, JToken> parameters, HashSet<string> replacedParameters)
        {
            this.parameters = parameters;
            this.replacedParameters = replacedParameters;
        }

        protected override SyntaxBase VisitParameterAssignmentSyntax(ParameterAssignmentSyntax syntax)
        {
            var previousParameterName = currentParameterName;
            currentParameterName = syntax.Name.IdentifierName;

            var result = base.VisitParameterAssignmentSyntax(syntax);

            currentParameterName = previousParameterName;
            return result;
        }

        protected override SyntaxBase ReplaceObjectPropertySyntax(ObjectPropertySyntax syntax)
        {
            var previousPropertyName = currentPropertyName;

            currentPropertyName = syntax.Key switch
            {
                StringSyntax stringSyntax when stringSyntax.TryGetLiteralValue() is string literal => literal,
                IdentifierSyntax identifier => identifier.IdentifierName,
                _ => null
            };

            var result = base.ReplaceObjectPropertySyntax(syntax);

            currentPropertyName = previousPropertyName;
            return result;
        }

        protected override SyntaxBase ReplaceFunctionCallSyntax(FunctionCallSyntax syntax)
        {
            if (syntax.Name.IdentifierName == LanguageConstants.InlineKeyword && syntax.Arguments.Length == 0)
            {
                string? overrideKey = DetermineOverrideKey();

                if (overrideKey != null && parameters.TryGetValue(overrideKey, out var overrideValue))
                {
                    replacedParameters.Add(overrideKey);
                    return ConvertJsonToBicepSyntax(overrideValue);
                }
            }

            return base.ReplaceFunctionCallSyntax(syntax);
        }

        private string? DetermineOverrideKey()
        {
            return currentPropertyName ?? currentParameterName;
        }
    }

    public static BicepParamFile ApplyParameterOverrides(ISourceFileFactory sourceFileFactory, BicepParamFile sourceFile, Dictionary<string, JToken> parameters)
    {
        var replacedParameters = new HashSet<string>();

        var inlineRewriter = new InlineReplacementRewriter(parameters, replacedParameters);
        var newProgramSyntax = inlineRewriter.Rewrite(sourceFile.ProgramSyntax);

        newProgramSyntax = CallbackRewriter.Rewrite(newProgramSyntax, syntax =>
        {
            if (syntax is not ParameterAssignmentSyntax paramSyntax)
            {
                return syntax;
            }

            if (parameters.TryGetValue(paramSyntax.Name.IdentifierName, out var overrideValue))
            {
                replacedParameters.Add(paramSyntax.Name.IdentifierName);
                var replacementValue = ConvertJsonToBicepSyntax(overrideValue);

                return new ParameterAssignmentSyntax(
                    paramSyntax.LeadingNodes,
                    paramSyntax.Keyword,
                    paramSyntax.Name,
                    paramSyntax.Assignment,
                    replacementValue
                );
            }

            return syntax;
        });

        // parameters that aren't explicitly in the .bicepparam file (e.g. parameters with default values)
        var additionalParams = parameters.Keys.Where(x => !replacedParameters.Contains(x)).ToArray();
        if (additionalParams.Length != 0)
        {
            var children = newProgramSyntax.Children.ToList();
            foreach (var paramName in additionalParams)
            {
                var overrideValue = parameters[paramName];
                var replacementValue = ConvertJsonToBicepSyntax(overrideValue);

                children.Add(SyntaxFactory.DoubleNewlineToken);
                children.Add(SyntaxFactory.CreateParameterAssignmentSyntax(paramName, replacementValue));
                replacedParameters.Add(paramName);
            }

            newProgramSyntax = new ProgramSyntax(
                children,
                newProgramSyntax.EndOfFile);
        }

        if (sourceFile.ProgramSyntax == newProgramSyntax)
        {
            // no changes were made
            return sourceFile;
        }

        return sourceFileFactory.CreateBicepParamFile(sourceFile.FileHandle.Uri, newProgramSyntax.ToString());
    }
}
