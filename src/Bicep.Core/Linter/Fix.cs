// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;

namespace Bicep.Core.Linter
{
    public class Fix
    {
        public Fix(string description, params Edit[] edits)
        {
            if (edits == null || edits.Length == 0)
            {
                throw new ArgumentException($"{nameof(edits)} must contain at least one value.");
            }

            this.Edits = edits;
            this.Description = description;
        }

        public IReadOnlyList<Edit> Edits { get; }

        public string Description { get; }
    }
}
