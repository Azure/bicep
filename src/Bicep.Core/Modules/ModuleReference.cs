// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Text.RegularExpressions;

namespace Bicep.Core.Modules
{
    /// <summary>
    /// Strongly typed representation of a module reference string.
    /// </summary>
    public abstract class ModuleReference
    {
        protected ModuleReference(string scheme, Uri parentModuleUri)
        {
            this.Scheme = scheme;
            this.ParentModuleUri = parentModuleUri;
        }

        protected static Regex ModuleAliasNameRegex { get; } = new(@"[\w-]");

        public string Scheme { get; }

        /// <summary>
        /// The URI of the template in which this module reference appears.
        /// </summary>
        public Uri ParentModuleUri { get; }

        /// <summary>
        /// Gets the fully qualified module reference, which includes the scheme.
        /// </summary>
        public virtual string FullyQualifiedReference => $"{this.Scheme}:{this.UnqualifiedReference}";

        /// <summary>
        /// Gets the unqualified module reference, which does not include the scheme.
        /// </summary>
        public abstract string UnqualifiedReference { get; }

        /// <summary>
        /// Gets a value indicating whether this reference points to an external module.
        /// </summary>
        public abstract bool IsExternal { get; }
    }
}
