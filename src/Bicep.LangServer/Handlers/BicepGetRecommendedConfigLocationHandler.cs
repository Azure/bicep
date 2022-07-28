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
using Bicep.LanguageServer.Providers;

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
        private readonly IClientCapabilitiesProvider clientCapabilitiesProvider;

        public BicepGetRecommendedConfigLocationHandler(ILanguageServerFacade server, IClientCapabilitiesProvider clientCapabilitiesProvider)
        {
            this.server = server;
            this.clientCapabilitiesProvider = clientCapabilitiesProvider;
        }

        public async Task<BicepGetRecommendedConfigLocationResult> Handle(BicepGetRecommendedConfigLocationParams request, CancellationToken cancellationToken)
        {
            try
            {
                var path = await GetRecommendedConfigFileLocation(this.server, this.clientCapabilitiesProvider, request.BicepFilePath);
                return new BicepGetRecommendedConfigLocationResult(path, null);
            }
            catch (Exception ex)
            {
                return new BicepGetRecommendedConfigLocationResult(null, ex.Message);
            }
        }

        public static async Task<string> GetRecommendedConfigFileLocation(ILanguageServerFacade server, IClientCapabilitiesProvider clientCapabilitiesProvider, string? bicepFilePath)
        {
            string[]? workspaceFolderPaths = null;

            if (clientCapabilitiesProvider.DoesClientSupportWorkspaceFolders())
            {
                var workspaceFolders = await server.Workspace.RequestWorkspaceFolders(new());
                workspaceFolderPaths = workspaceFolders?.Select(wf => wf.Uri.GetFileSystemPath()).ToArray();
            }

            return GetRecommendedConfigFileLocation(workspaceFolderPaths, bicepFilePath);
        }

        public static string GetRecommendedConfigFileLocation(string[]? workspaceFolderPaths, string? bicepFilePath)
        {
            var bicepFileFolder = Path.GetDirectoryName(bicepFilePath);
            if (string.IsNullOrWhiteSpace(bicepFileFolder))
            {
                // No bicep source file provided, or it wasn't an absolute path (e.g. an unsaved file)...

                // Use first workspace folder root if one exists
                if (workspaceFolderPaths?.Length > 0)
                {
                    return workspaceFolderPaths.First();
                }

                // Just use user folder
                return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile, Environment.SpecialFolderOption.Create);
            }
            else
            {
                // If the bicep source file is contained in a workspace folder, use that workspace folder's root
                var applicableWorkspaceFolder = GetFolderContainingPath(workspaceFolderPaths, bicepFileFolder);
                if (applicableWorkspaceFolder is not null)
                {
                    return applicableWorkspaceFolder;
                }

                // No matching workspace folders, use folder containing the bicep file source
                return bicepFileFolder;
            }
        }

        private static string? GetFolderContainingPath(string[]? workspaceFolderPaths, string bicepFolder)
        {
            if (workspaceFolderPaths is null)
            {
                return null;
            }

            bool caseSensitive = !RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            bicepFolder = AddSeparator(bicepFolder);

            foreach (var folder in workspaceFolderPaths)
            {
                string folderWithSeparator = AddSeparator(folder);
                if (bicepFolder.Contains(folderWithSeparator, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase))
                {
                    return folder;
                }
            }

            return null;
        }

        private static string AddSeparator(string folder)
        {
            return folder[folder.Length - 1] == Path.DirectorySeparatorChar ? folder : folder + Path.DirectorySeparatorChar;
        }
    }
}
