// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;

namespace Bicep.Core.TypeSystem.ThirdParty
{
    public class TypeIndex
    {
        public TypeIndex(
            IReadOnlyDictionary<string, TypeLocation> resources,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyList<TypeLocation>>> functions)
        {
            Resources = resources;
            Functions = functions;
        }

        /// <summary>
        /// Available resource types, indexed by resource type name.
        /// </summary>
        public IReadOnlyDictionary<string, TypeLocation> Resources { get; }

        /// <summary>
        /// Available resource function types, indexed by resource type -> api version.
        /// </summary>
        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyList<TypeLocation>>> Functions { get; }
    }
}

