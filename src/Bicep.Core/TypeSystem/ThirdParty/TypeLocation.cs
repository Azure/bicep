// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem.ThirdParty
{
    public class TypeLocation
    {
        public TypeLocation(string relativePath, int index)
        {
            RelativePath = relativePath;
            Index = index;
        }

        public string RelativePath { get; set; }

        public int Index { get; set; }
    }
}
