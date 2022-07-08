// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Workspaces
{
    public static class ParamSourceFileFactory
    {
        public static BicepParamFile CreateBicepParamFile(Uri fileUri, string fileContents)
        {
            var parser = new ParamsParser(fileContents);
            var lineStarts = TextCoordinateConverter.GetLineStarts(fileContents);

            return new(fileUri, lineStarts, parser.Program());
        }

    }
}
