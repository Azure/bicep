// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Bicep.Core.TypeSystem.ResourceTypeProviders
{
    ///<summary>
    /// Strongly typed representation of a provider reference string.
    /// </summary>
    public abstract class ResourceTypeProviderReference
    {
        protected ResourceTypeProviderReference(string scheme, Uri parentModuleUri)
        {
            this.Scheme = scheme;
            this.ParentModuleUri = parentModuleUri;
        }

        public string Scheme { get; }

        /// <summary>
        /// The URI of the template in which this provider reference appears.
        /// </summary>
        public Uri ParentModuleUri { get; }

        /// <summary>
        /// Gets the fully qualified provider reference, which includes the scheme.
        /// </summary>
        public virtual string FullyQualifiedReference => $"{this.Scheme}:{this.UnqualifiedReference}";

        /// <summary>
        /// Gets the unqualified provider reference, which does not include the scheme.
        /// </summary>
        public abstract string UnqualifiedReference { get; }

        /// <summary>
        /// Gets a value indicating whether this reference points to an external provider.
        /// </summary>
        public abstract bool IsExternal { get; }
    }
}
