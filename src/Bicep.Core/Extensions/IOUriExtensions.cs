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
    public static class IOUriExtensions
    {
        public static bool HasArmTemplateLikeExtension(this IOUri uri) =>
                uri.HasExtension(LanguageConstants.JsonFileExtension) ||
                uri.HasExtension(LanguageConstants.JsoncFileExtension) ||
                uri.HasExtension(LanguageConstants.ArmTemplateFileExtension);

        public static bool HasBicepExtension(this IOUri uri) => uri.HasExtension(LanguageConstants.LanguageFileExtension);

        public static bool HasBicepParamExtension(this IOUri uri) => uri.HasExtension(LanguageConstants.ParamsFileExtension);
    }
}
