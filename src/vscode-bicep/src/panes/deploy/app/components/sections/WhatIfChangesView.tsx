import { VSCodeDataGrid, VSCodeDataGridRow, VSCodeDataGridCell } from "@vscode/webview-ui-toolkit/react";
import { FC } from "react";
import { WhatIfChange, WhatIfPropertyChange } from "@azure/arm-resources";
import { FormSection } from "./FormSection";

interface WhatIfChangesViewProps {
  changes?: WhatIfChange[];
}

export const WhatIfChangesView: FC<WhatIfChangesViewProps> = ({ changes }) => {
  if (!changes) {
    return null;
  }

  const filteredChanges = changes.filter(x => x.changeType !== "Ignore");
  return (
    <FormSection title="What-If Changes">
      <VSCodeDataGrid>
        <VSCodeDataGridRow rowType="header">
          <VSCodeDataGridCell gridColumn="1" cellType="columnheader">Resource Id</VSCodeDataGridCell>
          <VSCodeDataGridCell gridColumn="2" cellType="columnheader">Change Type</VSCodeDataGridCell>
          <VSCodeDataGridCell gridColumn="3" cellType="columnheader">Changes</VSCodeDataGridCell>
        </VSCodeDataGridRow>
        {filteredChanges.map(change => (
          <VSCodeDataGridRow key={change.resourceId}>
            <VSCodeDataGridCell gridColumn="1">{change.resourceId}</VSCodeDataGridCell>
            <VSCodeDataGridCell gridColumn="2">{change.changeType}</VSCodeDataGridCell>
            <VSCodeDataGridCell gridColumn="3">{getWhatIfPropertyChanges(change.delta)}</VSCodeDataGridCell>
          </VSCodeDataGridRow>
        ))}
      </VSCodeDataGrid>
    </FormSection>
  );
};

function getWhatIfPropertyChanges(changes?: WhatIfPropertyChange[]) {
  if (!changes) {
    return null;
  }

  const filteredChanges = changes.filter(x => x.propertyChangeType !== "NoEffect");
  return (
    <VSCodeDataGrid>
      <VSCodeDataGridRow rowType="header">
        <VSCodeDataGridCell gridColumn="1" cellType="columnheader">Path</VSCodeDataGridCell>
        <VSCodeDataGridCell gridColumn="2" cellType="columnheader">Change Type</VSCodeDataGridCell>
      </VSCodeDataGridRow>
      {filteredChanges.map(change => (
        <VSCodeDataGridRow key={change.path}>
          <VSCodeDataGridCell gridColumn="1">{change.path}</VSCodeDataGridCell>
          <VSCodeDataGridCell gridColumn="2">{change.propertyChangeType}</VSCodeDataGridCell>
        </VSCodeDataGridRow>
      ))}
    </VSCodeDataGrid>
  );
}