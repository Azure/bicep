// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ErrorResponse } from "@azure/arm-resources";
import type { FC } from "react";
import type { DeploymentScope, DeployState } from "../../models";

import {
  VscodeBadge,
  VscodeCheckbox,
  VscodeProgressRing,
  VscodeTable,
  VscodeTableBody,
  VscodeTableCell,
  VscodeTableRow,
} from "@vscode-elements/react-elements";
import { useState } from "react";
import { getDeploymentResourceId } from "../hooks/useAzure";
import { getPreformattedJson } from "../utils";
import { FormSection } from "./FormSection";
import { PortalButton } from "./PortalButton";

interface ResultsViewProps {
  deployState: DeployState;
  scope: DeploymentScope;
}

export const ResultsView: FC<ResultsViewProps> = ({ deployState, scope }) => {
  const [showJson, setShowJson] = useState(false);

  if (!deployState.status) {
    return null;
  }

  return (
    <FormSection title="Result">
      <VscodeTable>
        <VscodeTableBody slot="body">
          {deployState.name && (
            <VscodeTableRow key={1}>
              <VscodeTableCell key="1">Deployment Name</VscodeTableCell>
              <VscodeTableCell key="2">
                <VscodeBadge>
                  {deployState.name}
                  <PortalButton
                    scope={scope}
                    resourceId={getDeploymentResourceId(scope, deployState.name)}
                    resourceType="Microsoft.Resources/deployments"
                  />
                </VscodeBadge>
              </VscodeTableCell>
            </VscodeTableRow>
          )}
          <VscodeTableRow key={2}>
            <VscodeTableCell key="1">Status</VscodeTableCell>
            <VscodeTableCell key="2">
              {deployState.status === "running" && <VscodeProgressRing />}
              {deployState.status !== "running" && (
                <VscodeBadge>{deployState.status === "succeeded" ? "Succeeded" : "Failed"}</VscodeBadge>
              )}
            </VscodeTableCell>
          </VscodeTableRow>
        </VscodeTableBody>
      </VscodeTable>

      {deployState.error && (
        <VscodeCheckbox onChange={() => setShowJson(!showJson)} checked={showJson}>
          Show JSON?
        </VscodeCheckbox>
      )}
      {deployState.error && getError(deployState.error, showJson)}
    </FormSection>
  );
};

function getError(error: ErrorResponse, showJson: boolean) {
  if (showJson) {
    return getPreformattedJson(error);
  }

  return (
    <VscodeTable>
      <VscodeTableBody slot="body">
        {error.code && (
          <VscodeTableRow key={0}>
            <VscodeTableCell id="1">Code</VscodeTableCell>
            <VscodeTableCell id="2">{error.code}</VscodeTableCell>
          </VscodeTableRow>
        )}
        {error.message && (
          <VscodeTableRow key={1}>
            <VscodeTableCell id="1">Message</VscodeTableCell>
            <VscodeTableCell id="2">{error.message}</VscodeTableCell>
          </VscodeTableRow>
        )}
        {error.target && (
          <VscodeTableRow key={2}>
            <VscodeTableCell id="1">Target</VscodeTableCell>
            <VscodeTableCell id="2">{error.target}</VscodeTableCell>
          </VscodeTableRow>
        )}
        {error.details && (
          <VscodeTableRow key={3}>
            <VscodeTableCell id="1">Details</VscodeTableCell>
            <VscodeTableCell id="2">{error.details.map((x) => getError(x, showJson))}</VscodeTableCell>
          </VscodeTableRow>
        )}
      </VscodeTableBody>
    </VscodeTable>
  );
}
