// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { FC } from "react";
import type { LocalDeploymentOperationContent, LocalDeployResponse } from "../messages";

import {
  VscodeTable,
  VscodeTableBody,
  VscodeTableCell,
  VscodeTableHeader,
  VscodeTableHeaderCell,
  VscodeTableRow,
} from "@vscode-elements/react-elements";
import { FormSection } from "./sections/FormSection";
import { getPreformattedJson } from "./utils";

export const LocalDeployResult: FC<{ result: LocalDeployResponse }> = ({ result }) => {
  const error = result.deployment.error;
  return (
    <FormSection title="Result">
      <p>{result.deployment.provisioningState}</p>
      {error && (
        <VscodeTable>
          {error && (
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
        </VscodeTable>
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
      <VscodeTable>
        <VscodeTableHeader slot="header">
          <VscodeTableHeaderCell id="1">Resource Name</VscodeTableHeaderCell>
          <VscodeTableHeaderCell id="2">State</VscodeTableHeaderCell>
          <VscodeTableHeaderCell id="3">Error</VscodeTableHeaderCell>
        </VscodeTableHeader>
        <VscodeTableBody slot="body">
          {result.operations.map((operation) => (
            <VscodeTableRow
              key={operation.resourceName}
              style={isFailed(operation) ? { background: "rgba(255, 72, 45, 0.3)" } : {}}
            >
              <VscodeTableCell id="1">{operation.resourceName}</VscodeTableCell>
              <VscodeTableCell id="2">{operation.provisioningState}</VscodeTableCell>
              <VscodeTableCell id="3">{operation.error ? getPreformattedJson(operation.error) : ""}</VscodeTableCell>
            </VscodeTableRow>
          ))}
        </VscodeTableBody>
      </VscodeTable>
    </FormSection>
  );
};

export const LocalDeployOutputs: FC<{ result: LocalDeployResponse }> = ({ result }) => {
  if (!result.deployment.outputs) {
    return null;
  }

  return (
    <FormSection title="Outputs">
      <VscodeTable>
        <VscodeTableHeader slot="header">
          <VscodeTableHeaderCell id="1">Name</VscodeTableHeaderCell>
          <VscodeTableHeaderCell id="2">Value</VscodeTableHeaderCell>
        </VscodeTableHeader>
        <VscodeTableBody slot="body">
          {Object.keys(result.deployment.outputs).map((name) => (
            <VscodeTableRow key={name}>
              <VscodeTableCell id="1">{name}</VscodeTableCell>
              <VscodeTableCell id="2">{getPreformattedJson(result.deployment.outputs[name])}</VscodeTableCell>
            </VscodeTableRow>
          ))}
        </VscodeTableBody>
      </VscodeTable>
    </FormSection>
  );
};

function isFailed(operation: LocalDeploymentOperationContent) {
  return operation.provisioningState.toLowerCase() === "failed";
}
