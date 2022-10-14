// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Azure.Deployments.Core.Configuration;
using Azure.Deployments.Core.Constants;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Templates.Engines;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Parsing;
using Bicep.Core.Text;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Workspaces
{
    public static class SourceFileFactory
    {
        private static readonly Uri InMemoryMainTemplateUri = new("inmemory:///main.json");

        private static readonly JsonLoadSettings JsonLoadSettings = new()
        {
            LineInfoHandling = LineInfoHandling.Ignore,
        };

        public static BicepSourceFile? TryCreateSourceFileByBicepLanguageId(Uri fileUri, string fileContents, string languageId) => languageId switch
        {
            LanguageConstants.LanguageId => CreateBicepFile(fileUri, fileContents),
            LanguageConstants.ParamsLanguageId => CreateBicepParamFile(fileUri, fileContents),
            _ => null
        };


        public static BicepSourceFile? TryCreateSourceFileByFileKind(Uri fileUri, string fileContents, BicepSourceFileKind? fileKind) => fileKind switch
        {
            BicepSourceFileKind.BicepFile => CreateBicepFile(fileUri, fileContents),
            BicepSourceFileKind.ParamsFile => CreateBicepParamFile(fileUri, fileContents),
            null => null,
            _ => throw new NotImplementedException($"Unexpected file kind '{fileKind}'.")
        };

        public static ISourceFile? TryCreateSourceFile(Uri fileUri, string fileContents, ModuleReference? moduleReference = null)
        {
            if (PathHelper.HasArmTemplateLikeExtension(fileUri))
            {
                return moduleReference is TemplateSpecModuleReference
                    ? CreateTemplateSpecFile(fileUri, fileContents)
                    : CreateArmTemplateFile(fileUri, fileContents);
            }

            if (PathHelper.HasBicepExtension(fileUri))
            {
                return CreateBicepFile(fileUri, fileContents);
            }

            if (PathHelper.HasBicepparamsExension(fileUri))
            {
                return CreateBicepParamFile(fileUri, fileContents);
            }

            return null;
        }

        public static ISourceFile CreateSourceFile(Uri fileUri, string fileContents, ModuleReference? moduleReference = null) =>
            TryCreateSourceFile(fileUri, fileContents, moduleReference) ?? CreateBicepFile(fileUri, fileContents);

        public static BicepParamFile CreateBicepParamFile(Uri fileUri, string fileContents)
        {
            var parser = new ParamsParser(fileContents);
            var lineStarts = TextCoordinateConverter.GetLineStarts(fileContents);

            return new(fileUri, lineStarts, parser.Program());
        }

        public static BicepFile CreateBicepFile(Uri fileUri, string fileContents)
        {
            var parser = new Parser(fileContents);
            var lineStarts = TextCoordinateConverter.GetLineStarts(fileContents);

            return new(fileUri, lineStarts, parser.Program());
        }

        public static ArmTemplateFile CreateArmTemplateFile(Uri fileUri, string fileContents)
        {
            try
            {
                var template = TemplateEngine.ParseTemplate(fileContents);

                ValidateTemplate(template);

                var templateObject = ParseObject(fileContents);

                return new(fileUri, template, templateObject);
            }
            catch (Exception)
            {
                return new(fileUri, null, null);
            }
        }

        public static TemplateSpecFile CreateTemplateSpecFile(Uri fileUri, string fileContents)
        {
            TemplateSpecFile CreateErrorFile() => new(fileUri, null, new ArmTemplateFile(InMemoryMainTemplateUri, null, null));

            try
            {
                var templateSpecObject = ParseObject(fileContents);

                if (templateSpecObject.Value<string>("id") is not string templateSpecId ||
                    templateSpecObject.SelectToken("properties.mainTemplate") is not JObject mainTemplateObject)
                {
                    return CreateErrorFile();
                }

                var mainTemplateFile = CreateArmTemplateFile(new Uri("inmemory:///main.json"), mainTemplateObject.ToString());

                return new(fileUri, templateSpecId, mainTemplateFile);
            }
            catch (Exception)
            {
                return CreateErrorFile();
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

        private static JObject ParseObject(string fileContents) => JObject.Parse(fileContents, JsonLoadSettings);
    }
}
