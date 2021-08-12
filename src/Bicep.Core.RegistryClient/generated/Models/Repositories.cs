// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Collections.Generic;
using Azure.Core;

namespace Bicep.Core.RegistryClient.Models
{
    /// <summary> List of repositories. </summary>
    internal partial class Repositories
    {
        /// <summary> Initializes a new instance of Repositories. </summary>
        internal Repositories()
        {
            RepositoriesValue = new ChangeTrackingList<string>();
        }

        /// <summary> Initializes a new instance of Repositories. </summary>
        /// <param name="repositoriesValue"> Repository names. </param>
        /// <param name="link"> . </param>
        internal Repositories(IReadOnlyList<string> repositoriesValue, string link)
        {
            RepositoriesValue = repositoriesValue;
            Link = link;
        }

        /// <summary> Repository names. </summary>
        public IReadOnlyList<string> RepositoriesValue { get; }
        public string Link { get; }
    }
}
