// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Azure.Deployments.Core.Definitions.Schema;
using Bicep.Core.Modules;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Workspaces
{
    public class TemplateSpecMainTemplateFile : ArmTemplateFile
    {
        public TemplateSpecMainTemplateFile(Uri fileUri, Template? template, JObject? templateObject, TemplateSpecModuleReference moduleReference)
            : base(fileUri, template, templateObject)
        {
            this.ModuleReference = moduleReference;
        }

        public TemplateSpecModuleReference ModuleReference { get; }
    }
}
