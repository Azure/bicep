// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { VscodeIcon } from "@vscode-elements/react-elements";
import { DeploymentScope } from "../../../models";

interface PortalButtonProps {
  scope: DeploymentScope;
  resourceId: string;
  resourceType: string;
}

export function PortalButton(props: PortalButtonProps) {
  const { resourceId, resourceType, scope } = props;

  let portalResourceUrl;
  switch (resourceType.toLowerCase()) {
    case 'microsoft.resources/deployments':
      // Deployments have a dedicated Portal blade to track progress
      portalResourceUrl = `${scope.portalUrl}/#@${scope.tenantId}/blade/HubsExtension/DeploymentDetailsBlade/overview/id/${encodeURIComponent(resourceId)}`;
      break;
    default:
      portalResourceUrl = `${scope.portalUrl}/#@${scope.tenantId}/resource${resourceId}`;
      break;
  }

  return (
    <a style={{ verticalAlign: 'middle' }} title="Open in Portal" href={`${portalResourceUrl}`}>
      <VscodeIcon name="globe" size={12} style={{ color: 'inherit' }} />
    </a>
  );
}