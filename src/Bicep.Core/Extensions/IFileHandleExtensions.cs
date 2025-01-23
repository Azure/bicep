// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Diagnostics;
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

        public static ResultWithDiagnosticBuilder<string> TryReadAllText(this IFileHandle fileHandle)
        {
            try
            {
                return new(fileHandle.ReadAllText());
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                // TOOD: Add URI to the error message.
                return new ResultWithDiagnosticBuilder<string>(x => x.ErrorOccurredReadingFile(exception.Message));
            }
        }
    }
}
