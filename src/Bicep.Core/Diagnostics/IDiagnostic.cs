// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.CodeAction;
using Bicep.Core.Text;

namespace Bicep.Core.Diagnostics;

public interface IDiagnostic : IFixable, IPositionable
{
    string Code { get; }

    DiagnosticSource Source { get; }

    DiagnosticStyling Styling { get; }

    DiagnosticLevel Level { get; }

    string Message { get; }

    Uri? Uri { get; }
}
