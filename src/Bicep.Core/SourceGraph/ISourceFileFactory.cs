// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.IO.Abstraction;

namespace Bicep.Core.SourceGraph
{
    public interface ISourceFileFactory
    {
        ISourceFile CreateSourceFile(IOUri fileUri, string fileContents, Type? sourceFileType = null);

        BicepFile CreateBicepFile(IOUri fileUri, string fileContents);

        BicepFile CreateBicepFile(IFileHandle fileHandle, string fileContents);

        BicepParamFile CreateBicepParamFile(IOUri fileUri, string fileContents);

        BicepReplFile CreateBicepReplFile(IFileHandle fileHandle, string fileContents);

        ArmTemplateFile CreateArmTemplateFile(IOUri fileUri, string fileContents);

        ArmTemplateFile CreateArmTemplateFile(IFileHandle fileHandle, string fileContents);

        TemplateSpecFile CreateTemplateSpecFile(IOUri fileUri, string fileContents);
    }
}
