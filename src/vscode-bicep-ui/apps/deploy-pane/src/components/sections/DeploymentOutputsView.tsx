// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { FC } from "react";

import {
  VscodeTable,
  VscodeTableBody,
  VscodeTableCell,
  VscodeTableHeader,
  VscodeTableHeaderCell,
  VscodeTableRow,
} from "@vscode-elements/react-elements";
import { getPreformattedJson } from "../utils";
import { FormSection } from "./FormSection";

interface DeploymentOutputsViewProps {
  outputs?: Record<string, unknown>;
}

export const DeploymentOutputsView: FC<DeploymentOutputsViewProps> = ({ outputs }) => {
  if (!outputs) {
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
          {Object.keys(outputs).map((name) => (
            <VscodeTableRow key={name}>
              <VscodeTableCell id="1">{name}</VscodeTableCell>
              <VscodeTableCell id="2">
                {getPreformattedJson((outputs[name] as { value: unknown }).value)}
              </VscodeTableCell>
            </VscodeTableRow>
          ))}
        </VscodeTableBody>
      </VscodeTable>
    </FormSection>
  );
};
