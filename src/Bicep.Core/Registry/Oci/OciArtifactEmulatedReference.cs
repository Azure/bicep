// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Modules;
using Bicep.Core.SourceGraph;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Registry.Oci
{
    /// <summary>
    /// Represents an OCI module reference that is emulated via a local filesystem path.
    /// When a module alias specifies a "fileSystem" property instead of "registry",
    /// module references are resolved to local .bicep files instead of pulling from a container registry.
    /// </summary>
    public class OciArtifactEmulatedReference : ArtifactReference
    {
        private readonly IFileHandle fileHandle;

        public OciArtifactEmulatedReference(BicepSourceFile referencingFile, string modulePath, IFileHandle fileHandle) : 
            base(referencingFile, OciArtifactReferenceFacts.EmulatedScheme)
        {
            this.modulePath = modulePath;
            this.fileHandle = fileHandle;
        }

        private readonly string modulePath;

        // Override FullyQualifiedReference so user-facing diagnostics shows "br:..."
        public override string FullyQualifiedReference => $"{OciArtifactReferenceFacts.Scheme}:{UnqualifiedReference}";

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
        // fileSystemPath is the filesystem path from the alias configuration
        // configFileUri is the URI of the bicepconfig.json file, used to resolve relative paths
        // unqualifiedReference is the unqualified reference string (e.g., "keyvault:1.0.0")
        // fileExplorer is the file explorer used to create file handles
        // aliasName is the name of the module alias, used in diagnostics
        public static ResultWithDiagnosticBuilder<OciArtifactEmulatedReference> TryParse(
            BicepSourceFile referencingFile,
            string fileSystemPath,
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
            // Resolve the filesystem base directory relative to bicepconfig.json
            // Ensure the fileSystem path ends with '/' so it's treated as a directory
            var directoryPath = fileSystemPath.EndsWith('/') || fileSystemPath.EndsWith('\\')
                ? fileSystemPath
                : fileSystemPath + "/";
            baseUri = configFileUri.Resolve(directoryPath);

            // Construct the file URI by appending the module path with a .bicep extension.
            var moduleFileName = modulePath + ".bicep";
            var moduleFileUri = baseUri.Resolve(moduleFileName);

            var fileHandle = fileExplorer.GetFile(moduleFileUri);

            return new(new OciArtifactEmulatedReference(referencingFile, modulePath, fileHandle));
        }

        public override bool Equals(object? obj)
        {
            if (obj is not OciArtifactEmulatedReference other)
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
