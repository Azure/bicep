// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Workspace.VSIntegration.Contracts;
using StreamJsonRpc;

namespace Bicep.VSLanguageServerClient
{
    /// <summary>
    /// VS lsp doesn't support 'workspace/didChangeWatchedFiles' notification. This class
    /// watches workspace files with extension - arm, bicep, json, jsonc and sends across
    /// notification to language server whenever there is a file change/create/delete event.
    /// </summary>
    public class DidChangeWatchedFilesNotifier : IDisposable
    {
        private readonly string[] filters = { "*.arm", "*.bicep", "*.json", "*.jsonc" };
        private readonly string? location;
        private readonly JsonRpc rpc;
        private List<FileSystemWatcher> fileSystemWatchers = new List<FileSystemWatcher>();

        public DidChangeWatchedFilesNotifier(JsonRpc rpc)
        {
            this.rpc = rpc;

            var sVsSolution = ServiceProvider.GlobalProvider.GetService(typeof(SVsSolution).GUID);

            // Handles project system scenario
            if (sVsSolution is IVsSolution vsSolution)
            {
                vsSolution.GetSolutionInfo(out string solutionDirectory, out _, out _);

                if (!string.IsNullOrWhiteSpace(solutionDirectory))
                {
                    location = solutionDirectory;
                }
            }
            // Handles open folder scenario
            else
            {
                var serviceForComponentModel = ServiceProvider.GlobalProvider.GetService(typeof(SComponentModel).GUID);
                if (serviceForComponentModel is IComponentModel componentModel)
                {
                    var workspaceServices = componentModel.DefaultExportProvider.GetExports<IVsFolderWorkspaceService>();

                    if (workspaceServices is not null && workspaceServices.Any())
                    {
                        var workspace = workspaceServices.First();
                        var currentWorkspace = workspace.Value.CurrentWorkspace;

                        if (currentWorkspace is not null)
                        {
                            location = currentWorkspace.Location;
                        }
                    }
                }
            }
        }

        public void CreateFileSystemWatchers()
        {
            if (rpc is not null && location is not null)
            {
                foreach (string filter in filters)
                {
                    var fileSystemWatcher = new FileSystemWatcher(location);

                    fileSystemWatcher.NotifyFilter = NotifyFilters.Attributes
                         | NotifyFilters.CreationTime
                         | NotifyFilters.DirectoryName
                         | NotifyFilters.FileName
                         | NotifyFilters.LastAccess
                         | NotifyFilters.LastWrite
                         | NotifyFilters.Security
                         | NotifyFilters.Size;
                    fileSystemWatcher.IncludeSubdirectories = true;
                    fileSystemWatcher.EnableRaisingEvents = true;
                    fileSystemWatcher.Filter = filter;
                    fileSystemWatcher.Changed += OnChanged;
                    fileSystemWatcher.Created += OnCreated;
                    fileSystemWatcher.Deleted += OnDeleted;

                    fileSystemWatchers.Add(fileSystemWatcher);
                }
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e) =>
            SendWorkspaceDidChangeConfigurationNotificationAsync(FileChangeType.Changed, e.FullPath);

        private void OnCreated(object sender, FileSystemEventArgs e) =>
            SendWorkspaceDidChangeConfigurationNotificationAsync(FileChangeType.Created, e.FullPath);

        private void OnDeleted(object sender, FileSystemEventArgs e) =>
            SendWorkspaceDidChangeConfigurationNotificationAsync(FileChangeType.Deleted, e.FullPath);

        private Task SendWorkspaceDidChangeConfigurationNotificationAsync(FileChangeType fileChangeType, string fullPath)
        {
            var fileEvent = new FileEvent()
            {
                FileChangeType = fileChangeType,
                Uri = new Uri(fullPath)

            };
            var didChangeWatchedFilesParams = new DidChangeWatchedFilesParams()
            {
                Changes = new FileEvent[] { fileEvent }
            };

            _ = rpc.NotifyWithParameterObjectAsync(Methods.WorkspaceDidChangeWatchedFilesName, didChangeWatchedFilesParams);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if (fileSystemWatchers.Any())
            {
                foreach (var fileSystemWatcher in fileSystemWatchers)
                {
                    fileSystemWatcher.EnableRaisingEvents = false;
                    fileSystemWatcher.Dispose();
                }

                fileSystemWatchers.Clear();
            }
        }
    }
}
