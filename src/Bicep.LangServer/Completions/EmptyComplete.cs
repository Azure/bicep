// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;

using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Completions
{
    /// <summary>
    /// Returns an empty auto complete list.
    /// </summary>
    public class EmptyComplete : IAutoComplete
    {
        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static readonly EmptyComplete Instance = new EmptyComplete();

        /// <summary>
        /// Singleton shared empty list.
        /// </summary>
        private static readonly List<CompletionItem> EmptyList = new List<CompletionItem>();

        /// <summary>
        /// Prevents a default instance of the <see cref="EmptyComplete"/> class from being created.
        /// </summary>
        private EmptyComplete()
        {
        }

        /// <summary>
        /// Gets the empty completion item list.
        /// </summary>
        /// <returns>Collection of completion items.</returns>
        public List<CompletionItem> GetCompletionItems()
        {
            return EmptyComplete.EmptyList;
        }

        /// <summary>
        /// Method to add a completion list item. Does nothing in this implementation.
        /// </summary>
        /// <param name="item">The item.</param>
        public void AddCompletionItem(CompletionItem item)
        {
        }
    }
}
