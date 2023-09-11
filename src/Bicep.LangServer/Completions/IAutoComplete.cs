// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;

using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Completions
{
    /// <summary>
    /// Interface for managing completion lists.
    /// </summary>
    public interface IAutoComplete
    {
        /// <summary>
        /// Method that returns the current completion list.
        /// </summary>
        /// <returns>List of completion items.</returns>
        List<CompletionItem> GetCompletionItems();

        /// <summary>
        /// Method that adds an item to the current completion list.
        /// </summary>
        /// <param name="item">The item.</param>
        void AddCompletionItem(CompletionItem item);
    }
}
