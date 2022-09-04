// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;
using Bicep.RegistryModuleTool.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.CommandLine.Invocation;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace Bicep.RegistryModuleTool.Commands
{
    public abstract class BaseCommandHandler : ICommandHandler
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
                var exitCode = this.InvokeInternal(context);

                return Task.FromResult(exitCode);
            }
            catch (Exception exception)
            {
                switch (exception)
                {
                    case BicepException:
                    case IOException:
                    case UnauthorizedAccessException:
                        this.Logger.LogDebug(exception, "Command failure.");
                        context.Console.WriteError(exception.Message);

                        break;

                    default:
                        this.Logger.LogCritical(exception, "Unexpected exception.");
                        break;
                }

                return Task.FromResult(1);
            }
        }

        protected abstract int InvokeInternal(InvocationContext context);

        public int Invoke(InvocationContext context)
        {
            try
            {
                return this.InvokeInternal(context);
            }
            catch (Exception exception)
            {
                switch (exception)
                {
                    case BicepException:
                    case IOException:
                    case UnauthorizedAccessException:
                        this.Logger.LogDebug(exception, "Command failure.");
                        context.Console.WriteError(exception.Message);

                        break;

                    default:
                        this.Logger.LogCritical(exception, "Unexpected exception.");
                        break;
                }

                return 1;
            }
        }
    }
}
