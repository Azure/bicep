// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { VSCodeDataGrid, VSCodeDataGridCell, VSCodeDataGridRow } from "@vscode/webview-ui-toolkit/react";
import { FC } from "react";
import { LocalDeploymentOperationContent, LocalDeployResponse } from "../../../../language";
import { FormSection } from "./sections/FormSection";
import { getPreformattedJson } from "./utils";

export const LocalDeployResult: FC<{ result: LocalDeployResponse }> = ({ result }) => {
  const error = result.deployment.error;
  return (
    <FormSection title="Result">
      <p>{result.deployment.provisioningState}</p>
      {error && (
        <VSCodeDataGrid gridTemplateColumns="max-content auto">
          {error && (
            <VSCodeDataGridRow key={0}>
              <VSCodeDataGridCell gridColumn="1">Code</VSCodeDataGridCell>
              <VSCodeDataGridCell gridColumn="2">{error.code}</VSCodeDataGridCell>
            </VSCodeDataGridRow>
          )}
          {error.message && (
            <VSCodeDataGridRow key={1}>
              <VSCodeDataGridCell gridColumn="1">Message</VSCodeDataGridCell>
              <VSCodeDataGridCell gridColumn="2">{error.message}</VSCodeDataGridCell>
            </VSCodeDataGridRow>
          )}
        </VSCodeDataGrid>
      )}
    </FormSection>
  );
};

export const LocalDeployOperations: FC<{ result: LocalDeployResponse }> = ({ result }) => {
  if (!result.operations) {
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
            State
          </VSCodeDataGridCell>
          <VSCodeDataGridCell gridColumn="3" cellType="columnheader">
            Error
          </VSCodeDataGridCell>
        </VSCodeDataGridRow>
        {result.operations.map((operation) => (
          <VSCodeDataGridRow
            key={operation.resourceName}
            style={isFailed(operation) ? { background: "rgba(255, 72, 45, 0.3)" } : {}}
          >
            <VSCodeDataGridCell gridColumn="1">{operation.resourceName}</VSCodeDataGridCell>
            <VSCodeDataGridCell gridColumn="2">{operation.provisioningState}</VSCodeDataGridCell>
            <VSCodeDataGridCell gridColumn="3">
              {operation.error ? getPreformattedJson(operation.error) : ""}
            </VSCodeDataGridCell>
          </VSCodeDataGridRow>
        ))}
      </VSCodeDataGrid>
    </FormSection>
  );
};

export const LocalDeployOutputs: FC<{ result: LocalDeployResponse }> = ({ result }) => {
  if (!result.deployment.outputs) {
    return null;
  }

  return (
    <FormSection title="Outputs">
      <VSCodeDataGrid>
        <VSCodeDataGridRow rowType="header">
          <VSCodeDataGridCell gridColumn="1" cellType="columnheader">
            Name
          </VSCodeDataGridCell>
          <VSCodeDataGridCell gridColumn="2" cellType="columnheader">
            Value
          </VSCodeDataGridCell>
        </VSCodeDataGridRow>
        {Object.keys(result.deployment.outputs).map((name) => (
          <VSCodeDataGridRow key={name}>
            <VSCodeDataGridCell gridColumn="1">{name}</VSCodeDataGridCell>
            <VSCodeDataGridCell gridColumn="2">
              {getPreformattedJson(result.deployment.outputs[name])}
            </VSCodeDataGridCell>
          </VSCodeDataGridRow>
        ))}
      </VSCodeDataGrid>
    </FormSection>
  );
};

function isFailed(operation: LocalDeploymentOperationContent) {
  return operation.provisioningState.toLowerCase() === "failed";
}
