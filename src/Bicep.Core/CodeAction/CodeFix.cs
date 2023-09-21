// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;

namespace Bicep.Core.CodeAction
{
    public class CodeFix
    {
        private readonly CodeReplacement replacement;
        private readonly IEnumerable<CodeReplacement>? additionalReplacements;

        public CodeFix(string title, bool isPreferred, CodeFixKind kind, CodeReplacement replacement)
        {
            this.Title = title;
            this.IsPreferred = isPreferred;
            this.Kind = kind;
            this.replacement = replacement;
            this.additionalReplacements = null;

            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Code fix title should not be empty");
            }
            if (title.EndsWith(".") || title.EndsWith("!"))
            {
                throw new ArgumentException($"Code fix title should not end with a period or exclamation mark: \"{title}\"", nameof(title));
            }
            if (char.IsLower(title[0]))
            {
                throw new ArgumentException($"Code fix title should not start with a lowercase letter: \"{title}\"", nameof(title));
            }
        }

        public CodeFix(string title, bool isPreferred, CodeFixKind kind, CodeReplacement replacement, params CodeReplacement[] additionalReplacement)
            : this(title, isPreferred, kind, replacement)
        {
            this.additionalReplacements = additionalReplacement;
        }

        public string Title { get; }

        /// <summary>
        /// Marks this as preferred. Only useful for the language server.
        /// In VSCode, preferred actions are used by the "auto fix" command and can be targeted by keybindings.
        /// </summary>
        public bool IsPreferred { get; }

        public CodeFixKind Kind { get; }

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
