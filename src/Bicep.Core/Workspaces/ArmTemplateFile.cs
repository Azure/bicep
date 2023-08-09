// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Azure.Deployments.Core.Definitions.Schema;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Workspaces
{
    public class ArmTemplateFile : ISourceFile
    {
        private readonly string originalSource;

        public ArmTemplateFile(Uri fileUri, string originalSource, Template? template, JObject? templateObject)
        {
            if ((template is null && templateObject is not null) ||
                (template is not null && templateObject is null))
            {
                throw new ArgumentException($"Expected {nameof(template)} and {nameof(templateObject)} to both be non-null or both be null.");
            }

            this.FileUri = fileUri;
            this.Template = template;
            this.TemplateObject = templateObject;
            this.originalSource = originalSource;
        }

        public Uri FileUri { get; }

        public string GetOriginalSource() => originalSource;

        public Template? Template { get; }

        public JObject? TemplateObject { get; }

        public bool HasErrors() => this.Template is null;
    }
}
