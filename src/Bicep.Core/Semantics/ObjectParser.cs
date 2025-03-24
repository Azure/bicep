// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Semantics
{
    public abstract class ObjectParser : IObjectParser
    {
        public bool TryExtractFromObject(string fileContent, string? tokenSelectorPath, IPositionable[] positionable, [NotNullWhen(false)] out IDiagnostic? errorDiagnostic, [NotNullWhen(true)] out JToken? newToken)
        {
            errorDiagnostic = null;
            newToken = this.ExtractTokenFromObject(fileContent);
            if (newToken is not { })
            {
                // Instead of catching and returning the parsing exception, we simply return a generic error.
                // This avoids having to deal with localization, and avoids possible confusion regarding line endings in the message.
                errorDiagnostic = this.GetExtractTokenErrorType(positionable[0]);
                return false;
            }
            if (tokenSelectorPath is not null)
            {
                return this.TryExtractFromTokenByPath(newToken, tokenSelectorPath, positionable[1], out errorDiagnostic, out newToken);
            }
            return true;
        }
        protected abstract JToken? ExtractTokenFromObject(string fileContent);

        protected abstract Diagnostic GetExtractTokenErrorType(IPositionable positionable);

        private bool TryExtractFromTokenByPath(JToken token, string tokenSelectorPath, IPositionable positionable, [NotNullWhen(false)] out IDiagnostic? errorDiagnostic, out JToken newToken)
        {
            newToken = token;
            errorDiagnostic = null;
            if (tokenSelectorPath is null)
            {
                return true;
            }

            try
            {
                var selectTokens = token.SelectTokens(tokenSelectorPath, false).ToList();

                switch (selectTokens.Count)
                {
                    case 0:
                        {
                            errorDiagnostic = DiagnosticBuilder.ForPosition(positionable).NoJsonTokenOnPathOrPathInvalid();
                            return false;
                        }
                    case 1:
                        {
                            newToken = selectTokens.First();
                            break;
                        }
                    default:
                        {
                            newToken = new JArray(selectTokens);
                            break;
                        }
                }
                return true;
            }
            catch (JsonException)
            {
                //path is invalid or user hasn't finished typing it yet
                errorDiagnostic = DiagnosticBuilder.ForPosition(positionable).NoJsonTokenOnPathOrPathInvalid();
                return false;
            }


        }

    }
}
