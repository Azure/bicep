// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.IO.Abstraction;

namespace Bicep.Core.Documentation;

/// <summary>
/// Options controlling how <see cref="IBicepDocumentationGenerator"/> renders documentation for a module.
/// </summary>
/// <param name="TemplateFile">An optional Scriban template file.</param>
/// <param name="TemplateRoot">An optional root directory for template includes.</param>
/// <param name="CustomValues">Optional string values exposed to the template.</param>
public record BicepDocumentationGenerationOptions(
    IOUri? TemplateFile,
    IOUri? TemplateRoot,
    IReadOnlyDictionary<string, string>? CustomValues)
{
    /// <summary>
    /// Gets the built-in Markdown options.
    /// </summary>
    public static BicepDocumentationGenerationOptions Default { get; } = new(
        TemplateFile: null,
        TemplateRoot: null,
        CustomValues: null);
}
