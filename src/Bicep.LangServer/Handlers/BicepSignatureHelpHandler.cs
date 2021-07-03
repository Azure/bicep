// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Deployments.Core.Extensions;
using Bicep.Core;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Completions;
using Bicep.LanguageServer.Utils;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepSignatureHelpHandler: SignatureHelpHandlerBase
    {
        private const string FunctionArgumentStart = "(";
        private const string FunctionArgumentEnd = ")";

        private readonly ICompilationManager compilationManager;

        public BicepSignatureHelpHandler(ICompilationManager compilationManager)
        {
            this.compilationManager = compilationManager;
        }

        public override Task<SignatureHelp?> Handle(SignatureHelpParams request, CancellationToken cancellationToken)
        {
            // local function
            static Task<SignatureHelp?> NoHelp() => Task.FromResult<SignatureHelp?>(null);

            CompilationContext? context = this.compilationManager.GetCompilation(request.TextDocument.Uri);
            if (context == null)
            {
                return NoHelp();
            }

            int offset = PositionHelper.GetOffset(context.LineStarts, request.Position);
            
            var functionCall = GetActiveFunctionCall(context.ProgramSyntax, offset);
            if (functionCall == null)
            {
                return NoHelp();
            }

            var semanticModel = context.Compilation.GetEntrypointSemanticModel();
            var symbol = semanticModel.GetSymbolInfo(functionCall);
            if (symbol is not FunctionSymbol functionSymbol)
            {
                // no symbol or symbol is not a function
                return NoHelp();
            }

            // suppress ErrorType in arguments because the code is being written
            // this prevents function signature mismatches due to errors
            var normalizedArgumentTypes = NormalizeArgumentTypes(functionCall.Arguments, semanticModel);

            // do not include return type in signatures for decorator functions
            // because the return type on decorators is currently an internal implementation detail
            // which will be confusing to users
            // (can revisit if we add decorator extensibility in the future)
            var includeReturnType = semanticModel.Binder.GetParent(functionCall) is not DecoratorSyntax;

            var signatureHelp = CreateSignatureHelp(functionCall.Arguments, normalizedArgumentTypes, functionSymbol, offset, includeReturnType);
            signatureHelp = TryReuseActiveSignature(request.Context, signatureHelp);

            return Task.FromResult<SignatureHelp?>(signatureHelp);
        }

        private static FunctionCallSyntaxBase? GetActiveFunctionCall(ProgramSyntax syntax, int offset)
        {
            // if the cursor is placed after the closing paren of a function, it needs to count as outside of that function call
            // for purposes of signature help (otherwise we'll show the wrong function when function calls are nested)
            var matchingNodes = SyntaxMatcher.FindNodesMatchingOffsetExclusive(syntax, offset);

            var index = matchingNodes
                .FindLastIndex(
                    matchingNodes.Count - 1,
                    current => current is FunctionCallSyntaxBase functionCall && TextSpan.BetweenExclusive(functionCall.OpenParen.Span, functionCall.CloseParen).ContainsInclusive(offset));

            return index < 0 ? null : (FunctionCallSyntaxBase)matchingNodes[index];
        }

        private static SignatureHelp TryReuseActiveSignature(SignatureHelpContext? context, SignatureHelp signatureHelp)
        {
            if (context?.ActiveSignatureHelp == null ||
                string.Equals(context.TriggerCharacter, FunctionArgumentStart, StringComparison.Ordinal) ||
                string.Equals(context.TriggerCharacter, FunctionArgumentEnd, StringComparison.Ordinal))
            {
                // we don't have a previous active signature or the user typed ( or ), which would indicate a new "session"
                return signatureHelp;
            }

            if (CheckIfSignatureHelpSimilar(context.ActiveSignatureHelp, signatureHelp))
            {
                // the signature help is for the same function so we can reuse the active signature index
                // this prevents resetting of the active signature when multiple overloads are ambiguous and the user selected a specific one manually
                return signatureHelp with
                {
                    ActiveSignature = context.ActiveSignatureHelp.ActiveSignature
                };
            }

            // cannot improve the active signature - return as-is
            return signatureHelp;
        }

        private static bool CheckIfSignatureHelpSimilar(SignatureHelp active, SignatureHelp @new)
        {
            // local function
            static string GetFunctionName(SignatureInformation info)
            {
                var openParenIndex = info.Label.IndexOf(FunctionArgumentStart, StringComparison.Ordinal);
                return openParenIndex <= 0 ? info.Label : info.Label.Substring(0, openParenIndex - 1);
            }

            var newSignatureCount = @new.Signatures.Count();
            if (active.ActiveSignature > newSignatureCount || active.Signatures.Count() != newSignatureCount)
            {
                return false;
            }

            return active.Signatures
                .Zip(@new.Signatures)
                .All(tuple => string.Equals(GetFunctionName(tuple.First), GetFunctionName(tuple.Second), StringComparison.Ordinal) &&
                              string.Equals(tuple.First.Documentation?.MarkupContent?.Value, tuple.Second.Documentation?.MarkupContent?.Value, StringComparison.Ordinal));
        }

        private static List<TypeSymbol> NormalizeArgumentTypes(ImmutableArray<FunctionArgumentSyntax> arguments, SemanticModel semanticModel)
        {
            return arguments
                .Select(arg =>
                {
                    var argumentType = semanticModel.GetTypeInfo(arg);
                    return argumentType is ErrorType ? LanguageConstants.Any : argumentType;
                })
                .ToList();
        }

        private static SignatureHelp CreateSignatureHelp(ImmutableArray<FunctionArgumentSyntax> arguments, List<TypeSymbol> normalizedArgumentTypes, FunctionSymbol symbol, int offset, bool includeReturnType)
        {
            // exclude overloads where the specified arguments have exceeded the maximum
            // allow count mismatches because the user may not have started typing the arguments yet
            var matchingOverloads = symbol.Overloads
                .Where(fo => !fo.MaximumArgumentCount.HasValue || normalizedArgumentTypes.Count <= fo.MaximumArgumentCount.Value)
                .Select(overload => (overload, result: overload.Match(normalizedArgumentTypes, out _, out _)))
                .ToList();

            int activeSignatureIndex = matchingOverloads.IndexOf(tuple => tuple.result == FunctionMatchResult.Match);
            if (activeSignatureIndex < 0)
            {
                // no best match - try potential match
                activeSignatureIndex = matchingOverloads.IndexOf(tuple => tuple.result == FunctionMatchResult.PotentialMatch);
            }

            return new SignatureHelp
            {
                Signatures = new Container<SignatureInformation>(matchingOverloads.Select(tuple => CreateSignature(tuple.overload, arguments, includeReturnType))),
                ActiveSignature = activeSignatureIndex < 0 ? (int?) null : activeSignatureIndex,
                ActiveParameter = GetActiveParameterIndex(arguments, offset)
            };
        }

        private static int? GetActiveParameterIndex(ImmutableArray<FunctionArgumentSyntax> arguments, int offset)
        {
            for (int i = 0; i < arguments.Length; i++)
            {
                // the comma token is included in the argument node, so we need to check the span of the expression
                if (arguments[i].Expression.Span.ContainsInclusive(offset))
                {
                    return i;
                }
            }

            return null;
        }

        private static SignatureInformation CreateSignature(FunctionOverload overload, ImmutableArray<FunctionArgumentSyntax> arguments, bool includeReturnType)
        {
            const string delimiter = ", ";

            var typeSignature = new StringBuilder();
            var parameters = new List<ParameterInformation>();

            typeSignature.Append(overload.Name);
            typeSignature.Append(FunctionArgumentStart);

            foreach (var fixedParameter in overload.FixedParameters)
            {
                AppendParameter(typeSignature, parameters, fixedParameter.Signature, fixedParameter.Description);
                typeSignature.Append(delimiter);
            }

            if (overload.VariableParameter != null)
            {
                // the function supports varargs
                int index = 0;

                // include minimum number of variable parameters in the signature and dynamically generate the additional ones
                while (index < overload.VariableParameter.MinimumCount || arguments.Length > parameters.Count)
                {
                    // we have a parameter that isn't accounted for in the signature
                    AppendParameter(typeSignature, parameters, overload.VariableParameter.GetNamedSignature(index), overload.VariableParameter.Description);
                    ++index;

                    typeSignature.Append(delimiter);
                }

                // on functions with varargs, we don't know if the user finished typing yet or not
                // as a result, we need to offer a hint that more arguments can be added
                // (otherwise you end up with signature help that prints something like concat() which is not helpful)
                AppendParameter(typeSignature, parameters, overload.VariableParameter.GenericSignature, overload.VariableParameter.Description);
                typeSignature.Append(delimiter);
            }

            if (parameters.Any())
            {
                // some parameters were appended, which left a trailing delimiter
                // remove the delimiter
                typeSignature.Remove(typeSignature.Length - delimiter.Length, delimiter.Length);
            }

            typeSignature.Append(FunctionArgumentEnd);

            if (includeReturnType)
            {
                typeSignature.Append(": ");
                typeSignature.Append(overload.TypeSignatureSymbol);
            }

            return new SignatureInformation
            {
                Label = typeSignature.ToString(),
                Documentation = new MarkupContent {Kind = MarkupKind.Markdown, Value = overload.Description},
                Parameters = new Container<ParameterInformation>(parameters)
            };
        }

        private static void AppendParameter(StringBuilder typeSignature, List<ParameterInformation> parameterInfos, string parameterSignature, string documentation)
        {
            int start = typeSignature.Length;
            typeSignature.Append(parameterSignature);
            int end = typeSignature.Length;

            parameterInfos.Add(new ParameterInformation
            {
                Label = new ParameterInformationLabel((start, end)),
                Documentation = new MarkupContent {Kind = MarkupKind.Markdown, Value = documentation}
            });
        }

        protected override SignatureHelpRegistrationOptions CreateRegistrationOptions(SignatureHelpCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            DocumentSelector = DocumentSelectorFactory.Create(),
            /*
             * ( - triggers sig. help when starting function arguments
             * , - separates function arguments
             * ) - triggers sig. help for the outer function (or nothing)
             */
            TriggerCharacters = new Container<string>(FunctionArgumentStart, ",", FunctionArgumentEnd),
            RetriggerCharacters = new Container<string>()
        };
    }
}
