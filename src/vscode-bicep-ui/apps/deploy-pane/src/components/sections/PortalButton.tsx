// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { DeploymentScope } from "../../models";

import { Codicon } from "@vscode-bicep-ui/components";

interface PortalButtonProps {
  scope: DeploymentScope;
  resourceId: string;
  resourceType: string;
}

export function PortalButton(props: PortalButtonProps) {
  const { resourceId, resourceType, scope } = props;

  let portalResourceUrl;
  switch (resourceType.toLowerCase()) {
    case "microsoft.resources/deployments":
      // Deployments have a dedicated Portal blade to track progress
      portalResourceUrl = `${scope.portalUrl}/#@${scope.tenantId}/blade/HubsExtension/DeploymentDetailsBlade/overview/id/${encodeURIComponent(resourceId)}`;
      break;
    default:
      portalResourceUrl = `${scope.portalUrl}/#@${scope.tenantId}/resource${resourceId}`;
      break;
  }

  return (
    <a
      style={{ verticalAlign: "middle" }}
      title="Open in Portal"
      href={`${portalResourceUrl}`} //CodeQL [SM01507] False positive - we use C# code to pass in the base URL
      >
      {" "}
      <Codicon name="globe" size={12} />
    </a>
  );
}
