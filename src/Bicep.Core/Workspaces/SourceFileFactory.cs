// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CodeDom;
using Azure.Deployments.Core.Configuration;
using Azure.Deployments.Core.Constants;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Templates.Engines;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Parsing;
using Bicep.Core.Registry;
using Bicep.Core.Text;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Workspaces
{
    public class SourceFileFactory : ISourceFileFactory
    {
        private static readonly Uri InMemoryMainTemplateUri = new("inmemory:///main.json");

        private static readonly JsonLoadSettings JsonLoadSettings = new()
        {
            LineInfoHandling = LineInfoHandling.Ignore,
        };

        private readonly IConfigurationManager configurationManager;

        private readonly IFeatureProviderFactory featureProviderFactory;

        public SourceFileFactory(IConfigurationManager configurationManager, IFeatureProviderFactory featureProviderFactory)
        {
            this.configurationManager = configurationManager;
            this.featureProviderFactory = featureProviderFactory;
        }

        public ISourceFile CreateSourceFile(Uri fileUri, string fileContents, Type? sourceFileType = null)
        {
            if (sourceFileType == typeof(BicepFile))
            {
                return CreateBicepFile(fileUri, fileContents);
            }

            if (sourceFileType == typeof(BicepParamFile))
            {
                return CreateBicepParamFile(fileUri, fileContents);
            }

            if (sourceFileType == typeof(ArmTemplateFile))
            {
                return CreateArmTemplateFile(fileUri, fileContents);
            }

            if (sourceFileType == typeof(TemplateSpecFile))
            {
                return CreateTemplateSpecFile(fileUri, fileContents);
            }

            if (sourceFileType is not null)
            {
                throw new ArgumentException($"Unexpected source file type {sourceFileType.Name}");
            }

            if (PathHelper.HasArmTemplateLikeExtension(fileUri))
            {
                return CreateArmTemplateFile(fileUri, fileContents);
            }

            if (PathHelper.HasBicepExtension(fileUri))
            {
                return CreateBicepFile(fileUri, fileContents);
            }

            if (PathHelper.HasBicepparamsExtension(fileUri))
            {
                return CreateBicepParamFile(fileUri, fileContents);
            }

            // The file does not have an extension. Assuming it is a Bicep file. Note that
            // this is only possible when a module reference path is provided without an
            // extension. When an untilted file (whose URI has no extension) in VS Code,
            // sourceFileType will be set by BicepCompilationManager.
            return CreateBicepFile(fileUri, fileContents);
        }

        public BicepParamFile CreateBicepParamFile(Uri fileUri, string fileContents)
        {
            var parser = new ParamsParser(fileContents);
            var lineStarts = TextCoordinateConverter.GetLineStarts(fileContents);

            return new(fileUri, lineStarts, parser.Program(), this.configurationManager, this.featureProviderFactory, parser.LexingErrorLookup, parser.ParsingErrorLookup);
        }

        public BicepFile CreateBicepFile(Uri fileUri, string fileContents)
        {
            var parser = new Parser(fileContents);
            var lineStarts = TextCoordinateConverter.GetLineStarts(fileContents);

            return new(fileUri, lineStarts, parser.Program(), this.configurationManager, this.featureProviderFactory, parser.LexingErrorLookup, parser.ParsingErrorLookup);
        }

        public ArmTemplateFile CreateArmTemplateFile(Uri fileUri, string fileContents)
        {
            try
            {
                var template = TemplateEngine.ParseTemplate(fileContents);

                ValidateTemplate(template);

                var templateObject = ParseObject(fileContents);

                return new(fileUri, fileContents, template, templateObject);
            }
            catch (Exception)
            {
                return new(fileUri, fileContents, null, null);
            }
        }

        public TemplateSpecFile CreateTemplateSpecFile(Uri fileUri, string fileContents)
        {
            TemplateSpecFile CreateErrorFile() => new(fileUri, fileContents, null, new ArmTemplateFile(InMemoryMainTemplateUri, fileContents, null, null));

            try
            {
                var templateSpecObject = ParseObject(fileContents);

                if (templateSpecObject.Value<string>("id") is not string templateSpecId ||
                    templateSpecObject.SelectToken("properties.mainTemplate") is not JObject mainTemplateObject)
                {
                    return CreateErrorFile();
                }

                var mainTemplateFile = CreateArmTemplateFile(new Uri("inmemory:///main.json"), mainTemplateObject.ToString());

                return new(fileUri, fileContents, templateSpecId, mainTemplateFile);
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
            template.Resources = [];

            // The apiVersion and deploymentScope parameters don't matter here as they are only used when validating resources.
            TemplateEngine.ValidateTemplate(template, CoreConstants.ApiVersion20200101, TemplateDeploymentScope.NotSpecified);

            template.Resources = templateResources;
        }

        private static JObject ParseObject(string fileContents) => JObject.Parse(fileContents, JsonLoadSettings);
    }
}
