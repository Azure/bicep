// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;

namespace Bicep.Core.Documentation;

/// <summary>
/// Generates documentation for Bicep modules.
/// </summary>
public interface IBicepDocumentationGenerator
{
    /// <summary>
    /// Builds the deterministic, typed documentation model for the entrypoint module of the given compilation.
    /// </summary>
    /// <param name="compilation">A successfully-compiled module. Compilations with errors are rejected.</param>
    /// <param name="customValues">Optional string values exposed to templates.</param>
    /// <returns>The typed documentation model.</returns>
    /// <exception cref="BicepDocumentationException">The compilation contains errors.</exception>
    BicepDocumentationModel BuildModel(Compilation compilation, IReadOnlyDictionary<string, string>? customValues = null);

    /// <summary>
    /// Renders a previously-built documentation model using the built-in template or a caller-supplied template file.
    /// </summary>
    /// <param name="model">The documentation model.</param>
    /// <param name="options">Optional rendering settings.</param>
    /// <returns>The rendered document.</returns>
    /// <exception cref="BicepDocumentationException">The template cannot be loaded or rendered.</exception>
    string Render(BicepDocumentationModel model, BicepDocumentationGenerationOptions? options = null);

    /// <summary>
    /// Builds the documentation model for the entrypoint module of the given compilation and renders it.
    /// </summary>
    /// <param name="compilation">The module compilation.</param>
    /// <param name="options">Optional rendering settings.</param>
    /// <returns>The rendered document.</returns>
    /// <exception cref="BicepDocumentationException">The model cannot be built or rendered.</exception>
    string Generate(Compilation compilation, BicepDocumentationGenerationOptions? options = null);
}
