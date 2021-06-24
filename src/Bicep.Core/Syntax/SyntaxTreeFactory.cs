// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.Linq;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Core.Json;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax.Converters;
using Bicep.Core.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Syntax
{
    public static class SyntaxTreeFactory
    {
        public static SyntaxTree CreateSyntaxTree(Uri fileUri, string fileContents, bool isEntryFile) => isEntryFile
            ? CreateBicepSyntaxTree(fileUri, fileContents)
            : CreateSyntaxTree(fileUri, fileContents);

        public static SyntaxTree CreateSyntaxTree(Uri fileUri, string fileContents) =>
            PathHelper.HasBicepExtension(fileUri) || !PathHelper.HasAnyExtension(fileUri)
                ? CreateBicepSyntaxTree(fileUri, fileContents)
                : CreateJsonSyntaxTree(fileUri, fileContents);

        private static SyntaxTree CreateBicepSyntaxTree(Uri fileUri, string fileContents)
        {
            var parser = new Parser(fileContents);
            var lineStarts = TextCoordinateConverter.GetLineStarts(fileContents);
            
            return new SyntaxTree(fileUri, lineStarts, parser.Program());
        }

        private static JsonSyntaxTree CreateJsonSyntaxTree(Uri fileUri, string fileContents)
        {
            var lineStarts = TextCoordinateConverter.GetLineStarts(fileContents);

            try
            {
                var templateObject = JObject.Parse(fileContents, new JsonLoadSettings
                {
                    CommentHandling = CommentHandling.Ignore,
                    LineInfoHandling = LineInfoHandling.Load,
                });

                var programSyntax = ProgramSyntaxConverter.ConvertToProgramSyntaxWithTypeInfoOnly(templateObject);

                if (!Template.TryFromJson<Template>(fileContents, out var _))
                {
                    return CreateErrorJsonSyntaxTree(fileUri, lineStarts, "Failed to desirialize the template.", templateObject);
                }

                return new JsonSyntaxTree(fileUri, lineStarts, templateObject, programSyntax);
            }
            catch (JsonReaderException e)
            {
                return CreateErrorJsonSyntaxTree(fileUri, lineStarts, e.Message, e.LineNumber, e.LinePosition);
            }
            catch (SyntaxConversionException e)
            {
                return CreateErrorJsonSyntaxTree(fileUri, lineStarts, e.Message, e.LineNumber, e.LinePosition);
            }
        }

        private static JsonSyntaxTree CreateErrorJsonSyntaxTree(Uri fileUri, ImmutableArray<int> lineStarts, string errorMessage, int errorLineNumber, int errorLinePosition)
        {
            var offset = TextCoordinateConverter.GetOffset(lineStarts, errorLineNumber - 1, errorLinePosition - 1);
            var diagnostic = DiagnosticBuilder.ForPosition(new TextSpan(offset, 0)).InvalidJsonTemplate(errorMessage);
            var program = new ProgramSyntax(
                Enumerable.Empty<SyntaxBase>(),
                SyntaxFactory.CreateToken(TokenType.EndOfFile),
                diagnostic.AsEnumerable());

            return new JsonSyntaxTree(fileUri, lineStarts, null, program);
        }

        private static JsonSyntaxTree CreateErrorJsonSyntaxTree(Uri fileUri, ImmutableArray<int> lineStarts, string errorMessage, IJsonLineInfo errorLineInfo) =>
            CreateErrorJsonSyntaxTree(fileUri, lineStarts, errorMessage, errorLineInfo.LineNumber, errorLineInfo.LinePosition);
    }
}
