// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.Linq;
using Azure.Deployments.Core.Configuration;
using Azure.Deployments.Core.Constants;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Templates.Engines;
using Azure.Deployments.Templates.Exceptions;
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
            PathHelper.HasExtension(fileUri, LanguageConstants.JsonFileExtension) ||
            PathHelper.HasExtension(fileUri, LanguageConstants.JsoncFileExtension) ||
            PathHelper.HasExtension(fileUri, LanguageConstants.ArmTemplateFileExtension)
                ? CreateJsonSyntaxTree(fileUri, fileContents)
                : CreateBicepSyntaxTree(fileUri, fileContents);

        public static SyntaxTree CreateBicepSyntaxTree(Uri fileUri, string fileContents)
        {
            var parser = new Parser(fileContents);
            var lineStarts = TextCoordinateConverter.GetLineStarts(fileContents);
            
            return new SyntaxTree(fileUri, lineStarts, parser.Program());
        }

        public static JsonSyntaxTree CreateJsonSyntaxTree(Uri fileUri, string fileContents)
        {
            var lineStarts = TextCoordinateConverter.GetLineStarts(fileContents);

            try
            {
                var template = TemplateEngine.ParseTemplate(fileContents);

                ValidateTemplate(template);

                var programSyntax = ProgramSyntaxConverter.ConvertToProgramSyntaxWithTypeInfoOnly(template);

                var templateObject = JObject.Parse(fileContents, new JsonLoadSettings
                {
                    CommentHandling = CommentHandling.Ignore,
                });

                return new JsonSyntaxTree(fileUri, lineStarts, templateObject, programSyntax);
            }
            catch (TemplateParsingException e)
            {
                return e.InnerException is JsonSerializationException { LineNumber: var lineNumber, LinePosition: var linePosition }
                    ? CreateErrorJsonSyntaxTree(fileUri, lineStarts, e.Message, lineNumber, linePosition)
                    : CreateErrorJsonSyntaxTree(fileUri, lineStarts, e.Message, 1, 1);
            }
            catch (TemplateValidationException e)
            {
                return e.TemplateErrorAdditionalInfo is { LineNumber: var lineNumber, LinePosition: var linePosition }
                    ? CreateErrorJsonSyntaxTree(fileUri, lineStarts, e.Message, lineNumber, linePosition)
                    : CreateErrorJsonSyntaxTree(fileUri, lineStarts, e.Message, 1, 1);
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

        private static void ValidateTemplate(Template template)
        {
            // To validate resources we would need to know what API version the user will use to deploy the template (which is impossible).
            // Replacing resources with an empty array to skip validating them.
            var templateResources = template.Resources;
            template.Resources = Array.Empty<TemplateResource>();

            // The apiVersion and deploymentScope parameters don't matter here as they are only used when validating resources.
            TemplateEngine.ValidateTemplate(template, CoreConstants.ApiVersion20200101, TemplateDeploymentScope.NotSpecified);

            template.Resources = templateResources;
        }
    }
}
