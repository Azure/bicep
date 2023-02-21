// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using MediatR;
using Microsoft.Extensions.Configuration;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bicep.LanguageServer.Settings
{
    public sealed class ConfigurationHandler
        : IDidChangeConfigurationSettingsHandler, IConfigurationProvider
    {
        /// <summary>
        ///     Create a new <see cref="ConfigurationHandler"/>.
        /// </summary>
        /// <param name="configuration">
        ///     The language server configuration.
        /// </param>
        public ConfigurationHandler(Configuration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            Configuration = configuration;
        }

        /// <summary>
        ///     Raised when configuration has changed.
        /// </summary>
        public event EventHandler<EventArgs>? ConfigurationChanged;

        /// <summary>
        ///     The language server configuration.
        /// </summary>
        public Configuration Configuration { get; }

        /// <summary>
        ///     The client's configuration capabilities.
        /// </summary>
        DidChangeConfigurationCapability? ConfigurationCapabilities { get; set; }

        /// <summary>
        ///     Called when configuration has changed.
        /// </summary>
        /// <param name="parameters">
        ///     The notification parameters.
        /// </param>
        /// <returns>
        ///     A <see cref="Task"/> representing the operation.
        /// </returns>
        Task<Unit> OnDidChangeConfiguration(DidChangeConfigurationObjectParams parameters)
        {
            //Configuration.UpdateFrom(parameters);

            if (ConfigurationChanged != null)
            {
                ConfigurationChanged(this, EventArgs.Empty);
            }

            return Unit.Task;
        }

        /// <summary>
        ///     Handle a change in configuration.
        /// </summary>
        /// <param name="request">The notification request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        ///     A <see cref="Task"/> representing the operation.
        /// </returns>
        Task<Unit> IRequestHandler<DidChangeConfigurationObjectParams, Unit>.Handle(DidChangeConfigurationObjectParams request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            try
            {
                return OnDidChangeConfiguration(request);
            }
            catch (Exception)
            {
                //Log.Error(unexpectedError, "Unhandled exception in {Method:l}.", "OnDidChangeConfiguration");
                return Unit.Task;
            }
        }

        public object GetRegistrationOptions(ClientCapabilities clientCapabilities)
        {
            return null!;
        }

        public void SetCapability(DidChangeConfigurationCapability capability, ClientCapabilities clientCapabilities)
        {
            ConfigurationCapabilities = capability;
        }
    }
}
