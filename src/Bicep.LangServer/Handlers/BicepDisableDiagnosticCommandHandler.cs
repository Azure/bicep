// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.LanguageServer.CompilationManager;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepDiagnosticCommandHandler : ExecuteTypedCommandHandlerBase<DocumentUri>
    {
        private readonly ICompilationManager compilationManager;

        public BicepDiagnosticCommandHandler(ICompilationManager compilationManager, ISerializer serializer)
            : base(LanguageConstants.DisableDiagnostic, serializer)
        {
            this.compilationManager = compilationManager;
        }

        public override async Task<Unit> Handle(DocumentUri documentUri, CancellationToken cancellationToken)
        {
            compilationManager.RefreshCompilation(documentUri);

            return await Unit.Task;
        }
    }
}
