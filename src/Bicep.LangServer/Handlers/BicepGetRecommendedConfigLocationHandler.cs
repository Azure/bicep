// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using System.Runtime.InteropServices;

namespace Bicep.LanguageServer.Handlers
{
    [Method("bicep/getRecommendedConfigLocation", Direction.ClientToServer)]
    public record BicepGetRecommendedConfigLocationParams : IRequest<BicepGetRecommendedConfigLocationResult>
    {
        public string? BicepFilePath;
    }

    public record BicepGetRecommendedConfigLocationResult(string? recommendedFolder, string? error);


    /// <summary>
    /// Retrieves the recommended folder to place a new bicepconfig.json file (used by client)
    /// </summary>
    public class BicepGetRecommendedConfigLocationHandler : IJsonRpcRequestHandler<BicepGetRecommendedConfigLocationParams, BicepGetRecommendedConfigLocationResult>
    {
        private readonly ILanguageServerFacade server;

        public BicepGetRecommendedConfigLocationHandler(ILanguageServerFacade server)
        {
            this.server = server;
        }

        public async Task<BicepGetRecommendedConfigLocationResult> Handle(BicepGetRecommendedConfigLocationParams request, CancellationToken cancellationToken)
        {
            try
            {
                var path = await GetRecommendedConfigFileLocation(this.server, request.BicepFilePath);
                return new BicepGetRecommendedConfigLocationResult(path, null);
            }
            catch (Exception ex)
            {
                return new BicepGetRecommendedConfigLocationResult(null, ex.Message);
            }
        }

        public static async Task<string> GetRecommendedConfigFileLocation(ILanguageServerFacade server, string? bicepFilePath)
        {
            var workspaceFolders = await server.Workspace.RequestWorkspaceFolders(new());
            var workspaceFolderPaths = workspaceFolders?.Select(wf => wf.Uri.GetFileSystemPath()).ToArray();
            return GetRecommendedConfigFileLocation(workspaceFolderPaths, bicepFilePath);
        }

        public static string GetRecommendedConfigFileLocation(string[]? workspaceFolderPaths, string? bicepFilePath)
        {
            if (bicepFilePath is null && workspaceFolderPaths?.Length > 0)
            {
                // No bicep source file indicated - use first workspace folder root
                return workspaceFolderPaths.First();
            }

            // If the bicep source file indicated is contained in a workspace folder, use its root
            var applicableWorkspaceFolder = GetFolderContainingPath(workspaceFolderPaths, bicepFilePath);
            if (applicableWorkspaceFolder is not null)
            {
                return applicableWorkspaceFolder;
            }

            // No workspace folder exists, use folder containing the bicep file source
            if (bicepFilePath is not null)
            {
                var dir = Path.GetDirectoryName(bicepFilePath);
                if (!string.IsNullOrWhiteSpace(dir))
                {
                    return dir;
                }
            }

            // If all else fails
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile, Environment.SpecialFolderOption.Create);
        }

        private static string? GetFolderContainingPath(string[]? workspaceFolderPaths, string? bicepFilePath)
        {
            bool caseSensitive = !RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            if (workspaceFolderPaths is null || bicepFilePath is null)
            {
                return null;
            }

            foreach (var folder in workspaceFolderPaths)
            {
                string folderWithSeparator = folder[folder.Length - 1] == Path.DirectorySeparatorChar ? folder : folder + Path.DirectorySeparatorChar;
                if (bicepFilePath.Contains(folderWithSeparator, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase))
                {
                    return folder;
                }
            }

            return null;
        }
    }
}
