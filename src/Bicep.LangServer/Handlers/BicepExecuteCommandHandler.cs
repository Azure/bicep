using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using System;
using Bicep.Core.Text;
using Bicep.Core.SemanticModel;
using Bicep.Core.Emit;
using Bicep.Core.Syntax;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepExecuteCommandHandler: ExecuteCommandHandler
    {
        private readonly ISerializer serializer;

        public BicepExecuteCommandHandler(ISerializer serializer)
            : base(RegistrationOptions)
        {
            this.serializer = serializer;
        }

        private static ExecuteCommandRegistrationOptions RegistrationOptions
            => new ExecuteCommandRegistrationOptions
            {
                DocumentSelector = DocumentSelector.ForLanguage(LanguageServerConstants.LanguageId),
                Commands = new Container<string>(
                    "compile"
                ),
            };

        public override Task<Unit> Handle(ExecuteCommandParams request, CancellationToken cancellationToken)
        {
            switch (request.Command)
            {
                case "compile":
                    var text = request.Arguments[0].ToString();
                    var outFile = request.Arguments[1].ToString();

                    var lineStarts = TextCoordinateConverter.GetLineStarts(text);
                    var compilation = new Compilation(SyntaxFactory.CreateFromText(text));
                    var emitter = new TemplateEmitter(compilation.GetSemanticModel());
                    var result = emitter.Emit(outFile);

                    if (result.Diagnostics.Any())
                    {
                        throw new Exception("Stuff went wrong");
                    }
                    break;
                default:
                    throw new System.NotImplementedException();
            }

            return Unit.Task;
        }
    }
}