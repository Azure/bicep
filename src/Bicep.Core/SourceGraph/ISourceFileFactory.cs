// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.SourceGraph
{
    public interface ISourceFileFactory
    {
        ISourceFile CreateSourceFile(Uri fileUri, string fileContents, Type? sourceFileType = null);

        BicepFile CreateBicepFile(Uri fileUri, string fileContents);

        BicepParamFile CreateBicepParamFile(Uri fileUri, string fileContents);

        ArmTemplateFile CreateArmTemplateFile(Uri fileUri, string fileContents);

        TemplateSpecFile CreateTemplateSpecFile(Uri fileUri, string fileContents);
    }
}
