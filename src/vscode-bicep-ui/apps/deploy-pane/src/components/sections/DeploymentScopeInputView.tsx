// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { FC } from "react";
import type { DeploymentScope } from "../../models";

import {
  VscodeBadge,
  VscodeButton,
  VscodeTable,
  VscodeTableBody,
  VscodeTableCell,
  VscodeTableRow,
} from "@vscode-elements/react-elements";
import { FormSection } from "./FormSection";

interface DeploymentScopeInputViewProps {
  scope?: DeploymentScope;
  onPickScope: () => void;
}

export const DeploymentScopeInputView: FC<DeploymentScopeInputViewProps> = ({ scope, onPickScope }) => {
  return (
    <FormSection title="Deployment Scope">
      {scope && (
        <VscodeTable>
          <VscodeTableBody slot="body">
            {(scope.scopeType === "resourceGroup" || scope.scopeType === "subscription") &&
              getGridRow("Subscription Id", scope.subscriptionId)}
            {scope.scopeType === "resourceGroup" && getGridRow("Resource Group", scope.resourceGroup)}
            {(scope.scopeType === "managementGroup" || scope.scopeType === "tenant") &&
              getGridRow("Tenant Id", scope.tenantId)}
            {scope.scopeType === "managementGroup" && getGridRow("Management Group", scope.managementGroup)}
            {(scope.scopeType === "managementGroup" || scope.scopeType === "tenant") &&
              getGridRow("Authenticated Subscription Id", scope.associatedSubscriptionId)}
            {scope.scopeType !== "resourceGroup" && getGridRow("Location", scope.location)}
          </VscodeTableBody>
        </VscodeTable>
      )}

      <div className="controls">
        <VscodeButton onClick={onPickScope} secondary={!!scope}>
          {!scope ? "Pick Scope" : "Change Scope"}
        </VscodeButton>
      </div>
    </FormSection>
  );
};

function getGridRow(label: string, value: string) {
  return (
    <VscodeTableRow key={label}>
      <VscodeTableCell id="1">{label}</VscodeTableCell>
      <VscodeTableCell id="2">
        <VscodeBadge>{value}</VscodeBadge>
      </VscodeTableCell>
    </VscodeTableRow>
  );
}
