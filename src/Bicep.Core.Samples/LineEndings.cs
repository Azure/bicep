// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Core.Samples
{
    /// <summary>
    /// Controls how data set treats line endings in source files.
    /// </summary>
    public enum LineEndings
    {
        /// <summary>
        /// Line endings are replaced with the runtime default line endings. 
        /// </summary>
        Auto,

        /// <summary>
        /// Line endings are replaced with Windows-style CRLF line endings.
        /// </summary>
        CRLF,

        /// <summary>
        /// Line endings are replaced with Linux-style LF line endings. (MacOS X also uses this type of line endings.)
        /// </summary>
        LF
    }
}
