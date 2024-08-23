// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { DeploymentOperation } from "@azure/arm-resources";
import {
  VSCodeDataGrid,
  VSCodeDataGridCell,
  VSCodeDataGridRow,
  VSCodeLink,
  VSCodeProgressRing,
} from "@vscode/webview-ui-toolkit/react";
import { FC } from "react";
import { DeploymentScope } from "../../../models";
import { getPreformattedJson, isFailed, isInProgress } from "../utils";
import { FormSection } from "./FormSection";
import { Codicon } from "@vscode-bicep-ui/components";

interface DeploymentOperationsViewProps {
  scope: DeploymentScope;
  operations?: DeploymentOperation[];
}

export const DeploymentOperationsView: FC<DeploymentOperationsViewProps> = ({ scope, operations }) => {
  if (!operations) {
    return null;
  }

  const filteredOperations = operations.filter(
    (x) => x.properties?.provisioningOperation !== "EvaluateDeploymentOutput",
  );

  if (filteredOperations.length == 0) {
    return null;
  }

  return (
    <FormSection title="Operations">
      <VSCodeDataGrid>
        <VSCodeDataGridRow rowType="header">
          <VSCodeDataGridCell gridColumn="1" cellType="columnheader">
            Resource Name
          </VSCodeDataGridCell>
          <VSCodeDataGridCell gridColumn="2" cellType="columnheader">
            Resource Type
          </VSCodeDataGridCell>
          <VSCodeDataGridCell gridColumn="3" cellType="columnheader">
            Operation
          </VSCodeDataGridCell>
          <VSCodeDataGridCell gridColumn="4" cellType="columnheader">
            State
          </VSCodeDataGridCell>
          <VSCodeDataGridCell gridColumn="5" cellType="columnheader">
            Status
          </VSCodeDataGridCell>
        </VSCodeDataGridRow>
        {filteredOperations.map((operation) => (
          <VSCodeDataGridRow
            key={operation.id}
            style={isFailed(operation) ? { background: "rgba(255, 72, 45, 0.3)" } : {}}
          >
            <VSCodeDataGridCell gridColumn="1">{getResourceNameContents(scope, operation)}</VSCodeDataGridCell>
            <VSCodeDataGridCell gridColumn="2">{operation.properties?.targetResource?.resourceType}</VSCodeDataGridCell>
            <VSCodeDataGridCell gridColumn="3">{operation.properties?.provisioningOperation}</VSCodeDataGridCell>
            <VSCodeDataGridCell gridColumn="4">
              {isInProgress(operation) ? <VSCodeProgressRing /> : operation.properties?.provisioningState}
            </VSCodeDataGridCell>
            <VSCodeDataGridCell gridColumn="5">
              {getPreformattedJson(operation.properties?.statusMessage)}
            </VSCodeDataGridCell>
          </VSCodeDataGridRow>
        ))}
      </VSCodeDataGrid>
    </FormSection>
  );
};

function getResourceNameContents(scope: DeploymentScope, operation: DeploymentOperation) {
  const resourceName = operation.properties?.targetResource?.resourceName;

  return (
    <>
      {resourceName}
      {getPortalLinkButton(scope, operation)}
    </>
  );
}

function getPortalLinkButton(scope: DeploymentScope, operation: DeploymentOperation) {
  const { targetResource, provisioningOperation } = operation.properties ?? {};
  if (!targetResource || !targetResource.id || !targetResource.resourceType) {
    return;
  }

  if (provisioningOperation !== 'Create' && provisioningOperation !== 'Read') {
    // It only makes sense to share a link to the portal if we're doing a PUT / GET on a resource (as opposed to a POST action)
    return;
  }

  let portalResourceUrl;
  switch (targetResource.resourceType.toLowerCase()) {
    case 'microsoft.resources/deployments':
      // Deployments have a dedicated Portal blade to track progress
      portalResourceUrl = `${scope.portalUrl}/#blade/HubsExtension/DeploymentDetailsBlade/overview/id/${encodeURIComponent(targetResource.id)}`;
      break;
    default:
      portalResourceUrl = `${scope.portalUrl}/#@${scope.tenantId}/resource${targetResource.id}`;
      break;
  }

  return (
    <VSCodeLink style={{verticalAlign: 'middle'}} title="Open in Portal" href={`${portalResourceUrl}`}>
      <Codicon name="globe" size={12} />
    </VSCodeLink>
  );
}