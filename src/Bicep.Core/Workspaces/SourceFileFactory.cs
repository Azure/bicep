// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Azure.Deployments.Core.Configuration;
using Azure.Deployments.Core.Constants;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Templates.Engines;
using Bicep.Core.FileSystem;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Workspaces
{
    public static class SourceFileFactory
    {
        public static ISourceFile CreateSourceFile(Uri fileUri, string fileContents, bool isEntryFile) => isEntryFile
            ? CreateBicepSourceFile(fileUri, fileContents)
            : CreateSourceFile(fileUri, fileContents);

        public static ISourceFile CreateSourceFile(Uri fileUri, string fileContents) =>
            PathHelper.HasExtension(fileUri, LanguageConstants.JsonFileExtension) ||
            PathHelper.HasExtension(fileUri, LanguageConstants.JsoncFileExtension) ||
            PathHelper.HasExtension(fileUri, LanguageConstants.ArmTemplateFileExtension)
                ? CreateArmTemplateSourceFile(fileUri, fileContents)
                : CreateBicepSourceFile(fileUri, fileContents);

        public static SyntaxTree CreateBicepSourceFile(Uri fileUri, string fileContents)
        {
            var parser = new Parser(fileContents);
            var lineStarts = TextCoordinateConverter.GetLineStarts(fileContents);
            
            return new SyntaxTree(fileUri, lineStarts, parser.Program());
        }

        public static ArmTemplateFile CreateArmTemplateSourceFile(Uri fileUri, string fileContents)
        {
            try
            {
                var template = TemplateEngine.ParseTemplate(fileContents);

                ValidateTemplate(template);

                var templateObject = JObject.Parse(fileContents, new JsonLoadSettings
                {
                    LineInfoHandling = LineInfoHandling.Ignore,
                });

                return new ArmTemplateFile(fileUri, template, templateObject);
            }
            catch (Exception)
            {
                return new ArmTemplateFile(fileUri, null, null);
            }
        }

        private static void ValidateTemplate(Template template)
        {
            // To validate resources we would need to know what API version the user will use to deploy the template, which is impossible.
            // Replacing resources with an empty array to skip validating them.
            var templateResources = template.Resources;
            template.Resources = Array.Empty<TemplateResource>();

            // The apiVersion and deploymentScope parameters don't matter here as they are only used when validating resources.
            TemplateEngine.ValidateTemplate(template, CoreConstants.ApiVersion20200101, TemplateDeploymentScope.NotSpecified);

            template.Resources = templateResources;
        }
    }
}
