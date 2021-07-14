// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Azure.Deployments.Core.Definitions.Schema;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Workspaces
{
    public class ArmTemplateFile : ISourceFile
    {
        public ArmTemplateFile(Uri fileUri, Template? template, JObject? templateObject)
        {
            if ((template is null && templateObject is not null) ||
                (template is not null && templateObject is null))
            {
                throw new ArgumentException($"Expected {nameof(template)} and {nameof(templateObject)} to be both non-null or null.");
            }

            this.FileUri = fileUri;
            this.Template = template;
            this.TemplateObject = templateObject;
        }

        public Uri FileUri { get; }

        public Template? Template { get; }

        public JObject? TemplateObject { get; }

        public bool HasErrors() => this.Template is null;
    }
}
