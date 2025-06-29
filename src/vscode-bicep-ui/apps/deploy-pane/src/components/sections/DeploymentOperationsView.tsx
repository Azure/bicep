// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { DeploymentOperation } from "@azure/arm-resources";
import type { FC } from "react";
import type { DeploymentScope } from "../../models";

import {
  VscodeProgressRing,
  VscodeTable,
  VscodeTableBody,
  VscodeTableCell,
  VscodeTableHeader,
  VscodeTableHeaderCell,
  VscodeTableRow,
} from "@vscode-elements/react-elements";
import { getPreformattedJson, isFailed, isInProgress } from "../utils";
import { FormSection } from "./FormSection";
import { PortalButton } from "./PortalButton";

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
      <VscodeTable>
        <VscodeTableHeader slot="header">
          <VscodeTableHeaderCell key="1">Resource Name</VscodeTableHeaderCell>
          <VscodeTableHeaderCell key="2">Resource Type</VscodeTableHeaderCell>
          <VscodeTableHeaderCell key="3">Operation</VscodeTableHeaderCell>
          <VscodeTableHeaderCell key="4">State</VscodeTableHeaderCell>
          <VscodeTableHeaderCell key="5">Status</VscodeTableHeaderCell>
        </VscodeTableHeader>
        <VscodeTableBody slot="body">
          {filteredOperations.map((operation) => (
            <VscodeTableRow
              key={operation.id}
              style={isFailed(operation) ? { background: "rgba(255, 72, 45, 0.3)" } : {}}
            >
              <VscodeTableCell key="1">{getResourceNameContents(scope, operation)}</VscodeTableCell>
              <VscodeTableCell key="2">{operation.properties?.targetResource?.resourceType}</VscodeTableCell>
              <VscodeTableCell key="3">{operation.properties?.provisioningOperation}</VscodeTableCell>
              <VscodeTableCell key="4">
                {isInProgress(operation) ? <VscodeProgressRing /> : operation.properties?.provisioningState}
              </VscodeTableCell>
              <VscodeTableCell key="5">{getPreformattedJson(operation.properties?.statusMessage)}</VscodeTableCell>
            </VscodeTableRow>
          ))}
        </VscodeTableBody>
      </VscodeTable>
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

  if (provisioningOperation !== "Create" && provisioningOperation !== "Read") {
    // It only makes sense to share a link to the portal if we're doing a PUT / GET on a resource (as opposed to a POST action)
    return;
  }

  return <PortalButton scope={scope} resourceId={targetResource.id} resourceType={targetResource.resourceType} />;
}
