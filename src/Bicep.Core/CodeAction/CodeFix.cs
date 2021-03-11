// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;

namespace Bicep.Core.CodeAction
{
    public class CodeFix
    {
        private readonly CodeReplacement replacement;
        private readonly IEnumerable<CodeReplacement>? additionalReplacements;

        public CodeFix(string description, bool isPreferred, CodeReplacement replacement)
        {
            this.Description = description;
            this.IsPreferred = isPreferred;
            this.replacement = replacement;
            this.additionalReplacements = null;
        }

        public CodeFix(string description, bool isPreferred, CodeReplacement replacement, params CodeReplacement[] additionalReplacement)
            : this(description, isPreferred, replacement)
        {
            this.additionalReplacements = additionalReplacement;
        }

        public string Description { get; }

        /// <summary>
        /// Marks this as preferred. Only useful for the language server.
        /// In VSCode, preferred actions are used by the "auto fix" command and can be targeted by keybindings.
        /// </summary>
        public bool IsPreferred { get; }

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
