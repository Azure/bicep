// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Extensions
{
    public static class IFileHandleExtensions
    {
        public static bool IsArmTemplateLikeFile(this IFileHandle fileHandle) =>
                fileHandle.Uri.HasExtension(LanguageConstants.JsonFileExtension) ||
                fileHandle.Uri.HasExtension(LanguageConstants.JsoncFileExtension) ||
                fileHandle.Uri.HasExtension(LanguageConstants.ArmTemplateFileExtension);

        public static bool IsBicepFile(this IFileHandle fileHandle) => fileHandle.Uri.HasExtension(LanguageConstants.LanguageFileExtension);

        public static bool IsBicepParamsFile(this IFileHandle fileHandle) => fileHandle.Uri.HasExtension(LanguageConstants.ParamsFileExtension);
    }
}
