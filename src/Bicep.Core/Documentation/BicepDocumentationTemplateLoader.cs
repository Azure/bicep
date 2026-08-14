// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.IO.Abstraction;
using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;
using Scriban.Syntax;

namespace Bicep.Core.Documentation;

internal sealed class BicepDocumentationTemplateLoader(IFileExplorer fileExplorer, IOUri root) : ITemplateLoader
{
    private readonly Dictionary<string, IOUri> resolvedPathsByKey = new(StringComparer.Ordinal);

    public string GetPath(TemplateContext context, SourceSpan callerSpan, string templateName)
    {
        IOUri resolved;

        try
        {
            resolved = root.Resolve(templateName);
        }
        catch (IOException ex)
        {
            throw new ScriptRuntimeException(callerSpan, $"Unable to resolve include path '{templateName}': {ex.Message}", ex);
        }

        var key = resolved.ToString();
        this.resolvedPathsByKey[key] = resolved;

        return key;
    }

    public string Load(TemplateContext context, SourceSpan callerSpan, string templatePath)
    {
        if (!this.resolvedPathsByKey.TryGetValue(templatePath, out var uri))
        {
            throw new ScriptRuntimeException(callerSpan, $"Unable to resolve include path '{templatePath}'.");
        }

        var file = fileExplorer.GetFile(uri);
        if (!file.Exists())
        {
            throw new ScriptRuntimeException(callerSpan, $"The include file '{templatePath}' does not exist.");
        }

        try
        {
            return file.ReadAllText();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            throw new ScriptRuntimeException(callerSpan, $"Unable to read include file '{templatePath}': {ex.Message}", ex);
        }
    }

    public ValueTask<string?> LoadAsync(TemplateContext context, SourceSpan callerSpan, string templatePath) =>
        new(Load(context, callerSpan, templatePath));
}
