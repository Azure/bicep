// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { WhatIfChange, WhatIfPropertyChange } from "@azure/arm-resources";
import type { FC } from "react";

import {
  VscodeTable,
  VscodeTableBody,
  VscodeTableCell,
  VscodeTableHeader,
  VscodeTableHeaderCell,
  VscodeTableRow,
} from "@vscode-elements/react-elements";
import { FormSection } from "./FormSection";

interface WhatIfChangesViewProps {
  changes?: WhatIfChange[];
}

export const WhatIfChangesView: FC<WhatIfChangesViewProps> = ({ changes }) => {
  if (!changes) {
    return null;
  }

  const filteredChanges = changes.filter((x) => x.changeType !== "Ignore");
  return (
    <FormSection title="What-If Changes">
      <VscodeTable>
        <VscodeTableHeader slot="header">
          <VscodeTableHeaderCell id="1">Resource Id</VscodeTableHeaderCell>
          <VscodeTableHeaderCell id="2">Change Type</VscodeTableHeaderCell>
          <VscodeTableHeaderCell id="3">Changes</VscodeTableHeaderCell>
        </VscodeTableHeader>
        <VscodeTableBody slot="body">
          {filteredChanges.map((change) => (
            <VscodeTableRow key={change.resourceId}>
              <VscodeTableCell id="1">{change.resourceId}</VscodeTableCell>
              <VscodeTableCell id="2">{change.changeType}</VscodeTableCell>
              <VscodeTableCell id="3">{getWhatIfPropertyChanges(change.delta)}</VscodeTableCell>
            </VscodeTableRow>
          ))}
        </VscodeTableBody>
      </VscodeTable>
    </FormSection>
  );
};

function getWhatIfPropertyChanges(changes?: WhatIfPropertyChange[]) {
  if (!changes) {
    return null;
  }

  const filteredChanges = changes.filter((x) => x.propertyChangeType !== "NoEffect");
  return (
    <VscodeTable>
      <VscodeTableHeader slot="header">
        <VscodeTableHeaderCell id="1">Path</VscodeTableHeaderCell>
        <VscodeTableHeaderCell id="2">Change Type</VscodeTableHeaderCell>
      </VscodeTableHeader>
      <VscodeTableBody slot="body">
        {filteredChanges.map((change) => (
          <VscodeTableRow key={change.path}>
            <VscodeTableCell id="1">{change.path}</VscodeTableCell>
            <VscodeTableCell id="2">{change.propertyChangeType}</VscodeTableCell>
          </VscodeTableRow>
        ))}
      </VscodeTableBody>
    </VscodeTable>
  );
}
