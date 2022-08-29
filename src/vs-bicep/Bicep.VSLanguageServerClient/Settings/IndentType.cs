// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.VSLanguageServerClient.Settings
{
    /// <summary>
    /// Indentation type for Bicep formatting
    /// </summary>
    public enum IndentType
    {
        /// <summary>
        /// Indent elements using tab characters
        /// </summary>
        Tabs,
        /// <summary>
        /// Indent elements using space charaters
        /// </summary>
        Spaces
    }
}
