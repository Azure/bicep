// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Deployments.Core.Definitions.Schema;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.IO.Abstraction;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.SourceGraph
{
    public class ArmTemplateFile : ISourceFile
    {
        private readonly IConfigurationManager configurationManager;

        public ArmTemplateFile(Uri fileUri, IFileHandle fileHandle, string text, Template? template, JObject? templateObject, IConfigurationManager configurationManager, IFeatureProvider featureProvider)
        {
            if ((template is null && templateObject is not null) ||
                (template is not null && templateObject is null))
            {
                throw new ArgumentException($"Expected {nameof(template)} and {nameof(templateObject)} to both be non-null or both be null.");
            }

            this.Uri = fileUri;
            this.FileHandle = fileHandle;
            this.Text = text;
            this.Template = template;
            this.TemplateObject = templateObject;
            this.Features = featureProvider;

            this.configurationManager = configurationManager;
        }

        public Uri Uri { get; }

        public IFileHandle FileHandle { get; }

        public string Text { get; }

        public Template? Template { get; }

        public JObject? TemplateObject { get; }

        public RootConfiguration Configuration => this.configurationManager.GetConfiguration(this.Uri);

        public IFeatureProvider Features { get; }

        public BicepSourceFileKind FileKind => BicepSourceFileKind.ArmTemplateFile;

        public bool HasErrors() => this.Template is null;
    }
}
