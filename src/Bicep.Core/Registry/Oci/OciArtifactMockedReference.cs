// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.SourceGraph;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Registry.Oci
{
    /// <summary>
    /// Represents an OCI module reference that is mocked via a local filesystem path.
    /// When a module alias is added to moduleAliasesMock configuration,
    /// module references are resolved to local .bicep files instead of pulling from a container registry.
    /// </summary>
    public class OciArtifactMockedReference : ArtifactReference
    {
        private readonly IFileHandle fileHandle;
        private readonly string modulePath;
        private readonly string? fullyQualifiedReference;

        public OciArtifactMockedReference(BicepSourceFile referencingFile, string modulePath, IFileHandle fileHandle, string? fullyQualifiedReference = null)
            : base(referencingFile.Features, referencingFile.Configuration, OciArtifactReferenceFacts.MockedScheme)
        {
            this.modulePath = modulePath;
            this.fileHandle = fileHandle;
            this.fullyQualifiedReference = fullyQualifiedReference;
        }

        // Override FullyQualifiedReference so user-facing diagnostics shows "br:..."
        public override string FullyQualifiedReference => fullyQualifiedReference ?? $"{OciArtifactReferenceFacts.Scheme}:{UnqualifiedReference}";

        public override string UnqualifiedReference => modulePath;

        public override bool IsExternal => false;

        public override ResultWithDiagnosticBuilder<IFileHandle> TryGetEntryPointFileHandle()
        {
            return new(fileHandle);
        }

        // Extracts the module path from an unqualified reference string by removing any tag or digest suffix
        public static string ExtractModulePath(string unqualifiedReference)
        {
            // Check for digest separator (@)
            var digestIndex = unqualifiedReference.IndexOf('@');
            if (digestIndex >= 0)
            {
                return unqualifiedReference[..digestIndex];
            }

            // Check for tag separator (:)
            var tagIndex = unqualifiedReference.LastIndexOf(':');
            if (tagIndex >= 0)
            {
                return unqualifiedReference[..tagIndex];
            }

            // No tag or digest — use the whole reference as the module path
            return unqualifiedReference;
        }

        // referencingFile is the Bicep source file containing the module reference
        // mapToFilePath is the path from the alias configuration
        // configFileUri is the URI of the bicepconfig.json file, used to resolve relative paths
        // unqualifiedReference is the unqualified reference string (e.g., "keyvault:1.0.0")
        // fileExplorer is the file explorer used to create file handles
        // aliasName is the name of the module alias, used in diagnostics
        public static ResultWithDiagnosticBuilder<OciArtifactMockedReference> TryParse(
            BicepSourceFile referencingFile,
           string mapToFilePath,
            IOUri configFileUri,
            string unqualifiedReference,
            IFileExplorer fileExplorer,
            string? aliasName = null)
        {
            var modulePath = ExtractModulePath(unqualifiedReference);

            if (string.IsNullOrEmpty(modulePath))
            {
                return new(x => x.ModulePathHasNotBeenSpecified());
            }

            var segments = modulePath.Split('/');
            foreach (var segment in segments)
            {
                if (!OciArtifactReferenceFacts.IsOciNamespaceSegment(segment))
                {
                    return new(x => x.InvalidOciArtifactReferenceInvalidPathSegment(aliasName, unqualifiedReference, segment));
                }
            }

            IOUri baseUri;
            // Ensure the mapToFilePath path ends with '/' so it's treated as a directory.
            var directoryPath = mapToFilePath.EndsWith('/') || mapToFilePath.EndsWith('\\')
                 ? mapToFilePath
                 : mapToFilePath + "/";

            if (IOUri.IsAbsoluteFilePath(mapToFilePath))
            {
                try
                {
                    baseUri = IOUri.FromFilePath(directoryPath);
                }
                catch (IOException ex)
                {
                    return new(x => x.InvalidOciArtifactModuleAliasMapToFilePath(aliasName, mapToFilePath, ex.Message));
                }
            }
            else
            {
                baseUri = configFileUri.Resolve(directoryPath);
            }

            // Construct the file URI by appending the module path with a .bicep extension.
            var moduleFileName = modulePath + ".bicep";
            var moduleFileUri = baseUri.Resolve(moduleFileName);

            var fileHandle = fileExplorer.GetFile(moduleFileUri);

            return new(new OciArtifactMockedReference(referencingFile, modulePath, fileHandle));
        }

        public override bool Equals(object? obj)
        {
            if (obj is not OciArtifactMockedReference other)
            {
                return false;
            }

            return StringComparer.Ordinal.Equals(modulePath, other.modulePath) &&
                   fileHandle.Equals(other.fileHandle);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(modulePath, StringComparer.Ordinal);
            hash.Add(fileHandle);

            return hash.ToHashCode();
        }
    }
}
