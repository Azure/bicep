import { VSCodeDivider, VSCodeDataGrid, VSCodeDataGridRow, VSCodeDataGridCell, VSCodeProgressRing } from "@vscode/webview-ui-toolkit/react";
import { FC } from "react";
import { DeploymentOperation } from "@azure/arm-resources";
import { getPreformattedJson, isFailed, isInProgress } from "../utils";

interface DeploymentOperationsViewProps {
  operations?: DeploymentOperation[];
}

export const DeploymentOperationsView: FC<DeploymentOperationsViewProps> = ({ operations }) => {
  if (!operations) {
    return null;
  }

  const filteredOperations = operations
    .filter(x => x.properties?.provisioningOperation !== "EvaluateDeploymentOutput")

  if (filteredOperations.length == 0) {
    return null;
  }

  return (
    <section>
      <VSCodeDivider />
      <h2>Operations</h2>
      <VSCodeDataGrid>
        <VSCodeDataGridRow rowType="header">
          <VSCodeDataGridCell gridColumn="1" cellType="columnheader">Resource Name</VSCodeDataGridCell>
          <VSCodeDataGridCell gridColumn="2" cellType="columnheader">Resource Type</VSCodeDataGridCell>
          <VSCodeDataGridCell gridColumn="3" cellType="columnheader">Operation</VSCodeDataGridCell>
          <VSCodeDataGridCell gridColumn="4" cellType="columnheader">State</VSCodeDataGridCell>
          <VSCodeDataGridCell gridColumn="5" cellType="columnheader">Status</VSCodeDataGridCell>
        </VSCodeDataGridRow>
        {filteredOperations.map(operation => (
          <VSCodeDataGridRow key={operation.id} style={isFailed(operation) ? { background: "rgba(255, 72, 45, 0.3)" } : {}}>
            <VSCodeDataGridCell gridColumn="1">{operation.properties?.targetResource?.resourceName}</VSCodeDataGridCell>
            <VSCodeDataGridCell gridColumn="2">{operation.properties?.targetResource?.resourceType}</VSCodeDataGridCell>
            <VSCodeDataGridCell gridColumn="3">{operation.properties?.provisioningOperation}</VSCodeDataGridCell>
            <VSCodeDataGridCell gridColumn="4">
              {isInProgress(operation) ? <VSCodeProgressRing /> : operation.properties?.provisioningState}
            </VSCodeDataGridCell>
            <VSCodeDataGridCell gridColumn="5">{getPreformattedJson(operation.properties?.statusMessage)}</VSCodeDataGridCell>
          </VSCodeDataGridRow>
        ))}
      </VSCodeDataGrid>
    </section>
  );
};