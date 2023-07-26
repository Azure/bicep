import { VSCodeDataGrid, VSCodeDataGridRow, VSCodeDataGridCell } from "@vscode/webview-ui-toolkit/react";
import { FC } from "react";
import { getPreformattedJson } from "../utils";
import { FormSection } from "./FormSection";

interface DeploymentOutputsViewProps {
  outputs?: Record<string, any>;
}

export const DeploymentOutputsView: FC<DeploymentOutputsViewProps> = ({ outputs }) => {
  if (!outputs) {
    return null;
  }

  return (
    <FormSection title="Outputs">
      <VSCodeDataGrid>
        <VSCodeDataGridRow rowType="header">
          <VSCodeDataGridCell gridColumn="1" cellType="columnheader">Name</VSCodeDataGridCell>
          <VSCodeDataGridCell gridColumn="2" cellType="columnheader">Value</VSCodeDataGridCell>
        </VSCodeDataGridRow>
        {Object.keys(outputs).map(name => (
          <VSCodeDataGridRow key={name}>
            <VSCodeDataGridCell gridColumn="1">{name}</VSCodeDataGridCell>
            <VSCodeDataGridCell gridColumn="2">{getPreformattedJson(outputs[name].value)}</VSCodeDataGridCell>
          </VSCodeDataGridRow>
        ))}
      </VSCodeDataGrid>
    </FormSection>
  );
};