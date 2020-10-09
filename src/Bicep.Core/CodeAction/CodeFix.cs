// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;

namespace Bicep.Core.CodeAction
{
    public class CodeFix
    {
        private readonly CodeReplacement replacement;
        private readonly IEnumerable<CodeReplacement>? additionalReplacements;

        public CodeFix(string description, CodeReplacement replacement)
        {
            this.Description = description;
            this.replacement = replacement;
            this.additionalReplacements = null;
        }

        public CodeFix(string description, CodeReplacement replacement, params CodeReplacement[] additionalReplacement)
            : this(description, replacement)
        {
            this.additionalReplacements = additionalReplacement;
        }

        public string Description { get; }

        public IEnumerable<CodeReplacement> Replacements
        {
            get
            {
                yield return this.replacement;

                if (this.additionalReplacements != null)
                {
                    foreach (var replacement in this.additionalReplacements)
                    {
                        yield return replacement;
                    }
                }
            }
        }
    }
}
