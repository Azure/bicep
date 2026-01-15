// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Deployments.Core.Configuration;
using Azure.Deployments.Core.Constants;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Templates.Engines;
using Bicep.Core.Configuration;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.Parsing;
using Bicep.Core.Text;
using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.SourceGraph
{
    public class SourceFileFactory : ISourceFileFactory
    {
        private static readonly JsonLoadSettings JsonLoadSettings = new()
        {
            LineInfoHandling = LineInfoHandling.Ignore,
        };

        private readonly IConfigurationManager configurationManager;
        private readonly IFeatureProviderFactory featureProviderFactory;
        private readonly IAuxiliaryFileCache auxiliaryFileCache;
        private readonly IFileExplorer fileExplorer;

        public SourceFileFactory(IConfigurationManager configurationManager, IFeatureProviderFactory featureProviderFactory, IAuxiliaryFileCache auxiliaryFileCache, IFileExplorer fileExplorer)
        {
            this.configurationManager = configurationManager;
            this.featureProviderFactory = featureProviderFactory;
            this.auxiliaryFileCache = auxiliaryFileCache;
            this.fileExplorer = fileExplorer;
        }

        public ISourceFile CreateSourceFile(IOUri fileUri, string fileContents, Type? sourceFileType = null)
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

            if (fileUri.HasArmTemplateLikeExtension())
            {
                return CreateArmTemplateFile(fileUri, fileContents);
            }

            if (fileUri.HasBicepExtension())
            {
                return CreateBicepFile(fileUri, fileContents);
            }

            if (fileUri.HasBicepParamExtension())
            {
                return CreateBicepParamFile(fileUri, fileContents);
            }

            // The file does not have an extension. Assuming it is a Bicep file. Note that
            // this is only possible when a module reference path is provided without an
            // extension. When an untilted file (whose URI has no extension) in VS Code,
            // sourceFileType will be set by BicepCompilationManager.
            return CreateBicepFile(fileUri, fileContents);
        }

        public BicepParamFile CreateBicepParamFile(IOUri fileUri, string fileContents)
        {
            var parser = new ParamsParser(fileContents, this.featureProviderFactory.GetFeatureProvider(fileUri));
            var lineStarts = TextCoordinateConverter.GetLineStarts(fileContents);
            var fileHandle = this.CreateFileHandle(fileUri);

            return new(fileHandle, lineStarts, parser.Program(), this.configurationManager, this.featureProviderFactory, this.auxiliaryFileCache, parser.LexingErrorLookup, parser.ParsingErrorLookup);
        }

        public BicepFile CreateBicepFile(IOUri fileUri, string fileContents)
        {
            var parser = new Parser(fileContents);
            var lineStarts = TextCoordinateConverter.GetLineStarts(fileContents);
            var fileHandle = this.CreateFileHandle(fileUri);

            return new(fileHandle, lineStarts, parser.Program(), this.configurationManager, this.featureProviderFactory, this.auxiliaryFileCache, parser.LexingErrorLookup, parser.ParsingErrorLookup);
        }

        public BicepFile CreateBicepFile(IFileHandle fileHandle, string fileContents)
        {
            var parser = new Parser(fileContents);
            var lineStarts = TextCoordinateConverter.GetLineStarts(fileContents);

            return new(fileHandle, lineStarts, parser.Program(), this.configurationManager, this.featureProviderFactory, this.auxiliaryFileCache, parser.LexingErrorLookup, parser.ParsingErrorLookup);
        }

        public BicepReplFile CreateBicepReplFile(IFileHandle fileHandle, IDirectoryHandle auxiliaryDirectoryHandle, string fileContents)
        {
            var parser = new ReplParser(fileContents);
            var lineStarts = TextCoordinateConverter.GetLineStarts(fileContents);

            return new(fileHandle, auxiliaryDirectoryHandle, lineStarts, parser.Program(), this.configurationManager, this.featureProviderFactory, this.auxiliaryFileCache, parser.LexingErrorLookup, parser.ParsingErrorLookup);
        }

        public ArmTemplateFile CreateArmTemplateFile(IOUri fileUri, string fileContents) =>
            CreateArmTemplateFile(this.CreateFileHandle(fileUri), fileContents);

        public TemplateSpecFile CreateTemplateSpecFile(IOUri fileUri, string fileContents)
        {
            TemplateSpecFile CreateErrorFile() => new(this.CreateFileHandle(fileUri), fileContents, null, CreateDummyArmTemplateFile(""));

            try
            {
                var templateSpecObject = ParseObject(fileContents);

                if (templateSpecObject.Value<string>("id") is not string templateSpecId ||
                    templateSpecObject.SelectToken("properties.mainTemplate") is not JObject mainTemplateObject)
                {
                    return CreateErrorFile();
                }

                var mainTemplateFile = CreateDummyArmTemplateFile(mainTemplateObject.ToString());
                var fileHandle = this.CreateFileHandle(fileUri);

                return new(fileHandle, fileContents, templateSpecId, mainTemplateFile);
            }
            catch (Exception)
            {
                return CreateErrorFile();
            }
        }

        public ArmTemplateFile CreateArmTemplateFile(IFileHandle fileHandle, string fileContents)
        {
            try
            {
                var template = TemplateEngine.ParseTemplate(fileContents);

                ValidateTemplate(template);

                var templateObject = ParseObject(fileContents);

                return new(fileHandle, fileContents, template, templateObject);
            }
            catch (Exception)
            {
                return new(fileHandle, fileContents, null, null);
            }
        }

        private ArmTemplateFile CreateDummyArmTemplateFile(string content) => CreateArmTemplateFile(DummyFileHandle.Default, content);

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

        // TODO: it might be better to use a language server sepcific file explorer to handle untitled files.
        // Will get this done when removing Workspace.
        private IFileHandle CreateFileHandle(IOUri uri) => uri.Scheme.IsUntitled ? new DummyFileHandle(uri) : this.fileExplorer.GetFile(uri);
    }
}
