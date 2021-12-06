// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.CommandLine.Invocation;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace Bicep.RegistryModuleTool.Commands
{
    internal abstract class BaseCommandHandler : ICommandHandler
    {
        protected BaseCommandHandler(IFileSystem fileSystem, ILogger logger)
        {
            this.FileSystem = fileSystem;
            this.Logger = logger;
        }

        protected IFileSystem FileSystem { get; }

        protected ILogger Logger { get; }

        public virtual Task<int> InvokeAsync(InvocationContext context)
        {
            try
            {
                this.InvokeInternal(context);
            }
            catch (BicepException bicepException)
            {
                this.Logger.LogError("{message}", bicepException.Message);

                return Task.FromResult(1);
            }
            catch (Exception exception)
            {
                this.Logger.LogError(exception, "Unexpected exception.");

                return Task.FromResult(1);
            }

            return Task.FromResult(0);
        }

        protected abstract void InvokeInternal(InvocationContext context);
    }
}
