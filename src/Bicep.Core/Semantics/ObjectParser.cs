// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Intermediate;
using Bicep.Core.Parsing;
using Bicep.Core.TypeSystem;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpYaml.Serialization;

namespace Bicep.Core.Semantics
{
    public abstract class ObjectParser : IObjectParser
    {
        public bool TryExtractFromObject(string fileContent, string? tokenSelectorPath, IPositionable positionable, [NotNullWhen(false)] out ErrorDiagnostic? errorDiagnostic, out JToken newToken){
            errorDiagnostic = null;
            newToken = this.ExtractTokenFromObject(fileContent);
            if (newToken is not { })
            {
                // Instead of catching and returning the YML parse exception, we simply return a generic error.
                // This avoids having to deal with localization, and avoids possible confusion regarding line endings in the message.
                errorDiagnostic = this.GetExtractTokenErrorType(positionable);
                return false;
            }
            if (tokenSelectorPath is not null){
                return this.TryExtractFromTokenByPath(newToken, tokenSelectorPath, positionable, out errorDiagnostic, out newToken);
            }
            return true;
        }
        public abstract JToken ExtractTokenFromObject(string fileContent);
        public abstract ErrorDiagnostic GetExtractTokenErrorType(IPositionable positionable);
        public bool TryExtractFromTokenByPath(JToken token, string tokenSelectorPath, IPositionable positionable, [NotNullWhen(false)] out ErrorDiagnostic? errorDiagnostic, out JToken newToken)
        {
            newToken = token;
            errorDiagnostic = null;
            if (tokenSelectorPath is null){
                return true;
            }

            var selectTokens = token.SelectTokens(tokenSelectorPath, false).ToList();

            switch (selectTokens.Count)
            {
                case 0: {
                    errorDiagnostic = DiagnosticBuilder.ForPosition(positionable).NoJsonTokenOnPathOrPathInvalid();
                    return false;
                }
                case 1: {
                    newToken = selectTokens.First();
                    break;
                }
                default: {
                    newToken = new JArray(selectTokens);
                    break;
                }
            }
            return true;
        }

    }
}
