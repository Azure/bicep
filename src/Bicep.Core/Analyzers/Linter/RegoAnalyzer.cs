// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.CodeAction;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Regorus;

namespace Bicep.Core.Analyzers.Linter;

public class RegoLinter
{
    private static ImmutableDictionary<string, string> RegoRules = new Dictionary<string, string>() {
        ["Storage/ANF_VolumesShouldUseKerberosEncryption.rego"] = """
package policy

ExportPolicyRules := field("Microsoft.NetApp/netAppAccounts/capacityPools/volumes/exportPolicy.rules[*]")
KerberosEnabled := field("Microsoft.NetApp/netAppAccounts/capacityPools/volumes/kerberosEnabled")
ProtocolTypes := field("Microsoft.NetApp/netAppAccounts/capacityPools/volumes/protocolTypes[*]")

effect := parameters.effect if {
  type == "Microsoft.NetApp/netAppAccounts/capacityPools/volumes"
  checkTypes
  checkKerberosEnabledAndRules
}

checkTypes := true if {
  some type in ProtocolTypes
  type == "NFSv4.1"
}

checkKerberosEnabledAndRules := true if {
  not exists(KerberosEnabled)
} else if {
  KerberosEnabled == false
} else if {
  KerberosEnabled
  checkRules
}

checkRules := true if {
  some rule in ExportPolicyRules
  checkKerberos5ReadOnlyKerberos5ReadWriteEtc(rule)
}

checkKerberos5ReadOnlyKerberos5ReadWriteEtc(rule) := true if {
  rule.kerberos5pReadWrite == false
  rule.kerberos5pReadOnly == false
} else if {
  rule.kerberos5ReadOnly
} else if {
  rule.kerberos5ReadWrite
} else if {
  rule.kerberos5iReadOnly
} else if {
  rule.kerberos5iReadWrite
}
"""
    }.ToImmutableDictionary();

    public void Analyze(SemanticModel model, IDiagnosticWriter diagnostics)
    {
        foreach (var resource in model.DeclaredResources)
        {
            if (!resource.IsAzResource)
            {
                continue;
            }

            if (resource.Symbol.DeclaringResource.TryGetBody() is not { } resourceBody)
            {
                continue;
            }

            foreach (var rule in RegoRules)
            {
                var engine = new Engine();
                engine.AddPolicy(rule.Key, rule.Value);
                engine.SetInputJson(resourceBody.ToString());

                var value = engine.EvalQuery("data.policy.effect");
                var valueDoc = System.Text.Json.JsonDocument.Parse(value);
            }
        }
    }
}