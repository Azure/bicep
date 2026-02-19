// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.ComponentModel;
using Bicep.Core;
using Bicep.Core.Extensions;
using Bicep.Decompiler;
using Bicep.IO.Abstraction;
using ModelContextProtocol.Server;

namespace Bicep.McpServer;

[McpServerToolType]
public sealed class BicepDecompilerTools(
    IFileExplorer fileExplorer,
    BicepDecompiler decompiler)
{
    public record DecompileResultDefinition(
        [Description("The entrypoint file URI")]
        Uri EntrypointUri,
        [Description("The generated files to save")]
        ImmutableDictionary<Uri, string> FilesToSave);

    [McpServerTool(Title = "Decompile ARM template file", Destructive = false, Idempotent = true, OpenWorld = true, ReadOnly = true, UseStructuredContent = true)]
    [Description("""
    Converts an Azure Resource Manager (ARM) template JSON file into modern Bicep syntax (.bicep).
    
    Use this tool to:
    - Migrate existing ARM JSON templates to the more readable and maintainable Bicep language
    - Learn Bicep syntax by seeing how JSON templates translate to Bicep
    - Modernize legacy infrastructure-as-code
    
    Accepts files with .json, .jsonc, or .arm extensions. The file path must be absolute.
    The result includes the entrypoint URI and all generated Bicep files (which may include additional modules for nested/linked templates).
    
    Note: Decompilation is a best-effort process. Some ARM template features may require manual adjustment in the generated Bicep code. Review the output for any TODO comments or warnings.
    """)]
    public async Task<DecompileResultDefinition> DecompileArmTemplateFile(
        [Description("The path to the .json ARM template file")] string filePath)
    {
        var fileUri = IOUri.FromFilePath(filePath);
        if (!fileUri.HasArmTemplateLikeExtension())
        {
            throw new ArgumentException("The specified file must have a .json, .jsonc or .arm extension.", nameof(filePath));
        }

        var jsonContents = await fileExplorer.GetFile(fileUri).ReadAllTextAsync();
        var bicepUri = fileUri.WithExtension(LanguageConstants.LanguageFileExtension);
        var decompilation = await decompiler.Decompile(bicepUri, jsonContents);

        return new(
            decompilation.EntrypointUri.ToUri(),
            decompilation.FilesToSave.ToImmutableDictionary(
                kvp => kvp.Key.ToUri(),
                kvp => kvp.Value));
    }

    [McpServerTool(Title = "Decompile ARM parameters file", Destructive = false, Idempotent = true, OpenWorld = true, ReadOnly = true, UseStructuredContent = true)]
    [Description("""
    Converts an ARM template parameters JSON file into Bicep parameters syntax (.bicepparam).
    
    Use this tool to:
    - Migrate ARM JSON parameter files to Bicep parameters format
    - Convert deployment parameter files when modernizing to Bicep
    - Generate .bicepparam files from existing ARM deployments
    
    Accepts files with .json, .jsonc, or .arm extensions. The file path must be absolute.
    
    The generated .bicepparam file includes a 'using' statement placeholder that must be completed, all parameters with their values preserved, and KeyVault references converted to az.getSecret() function calls if present.
    """)]
    public async Task<DecompileResultDefinition> DecompileArmParametersFile(
        [Description("The path to the .json ARM parameters file")] string filePath)
    {
        var fileUri = IOUri.FromFilePath(filePath);
        if (!fileUri.HasArmTemplateLikeExtension())
        {
            throw new ArgumentException("The specified file must have a .json, .jsonc or .arm extension.", nameof(filePath));
        }

        var jsonContents = await fileExplorer.GetFile(fileUri).ReadAllTextAsync();
        var bicepParamsUri = fileUri.WithExtension(LanguageConstants.ParamsFileExtension);
        var decompilation = decompiler.DecompileParameters(jsonContents, bicepParamsUri, null);

        return new(
            decompilation.EntrypointUri.ToUri(),
            decompilation.FilesToSave.ToImmutableDictionary(
                kvp => kvp.Key.ToUri(),
                kvp => kvp.Value));
    }
}
