import { VSCodeDataGrid, VSCodeDataGridRow, VSCodeDataGridCell, VSCodeProgressRing } from "@vscode/webview-ui-toolkit/react";
import { FC } from "react";
import { DeploymentOperation } from "@azure/arm-resources";
import { getPreformattedJson, isFailed, isInProgress } from "../utils";
import { FormSection } from "./FormSection";
import { DeploymentScope } from "../../../models";

interface DeploymentOperationsViewProps {
  scope: DeploymentScope;
  operations?: DeploymentOperation[];
}

export const DeploymentOperationsView: FC<DeploymentOperationsViewProps> = ({ scope, operations }) => {
  if (!operations) {
    return null;
  }

  const filteredOperations = operations
    .filter(x => x.properties?.provisioningOperation !== "EvaluateDeploymentOutput")

  if (filteredOperations.length == 0) {
    return null;
  }

  return (
    <FormSection title="Operations">
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
            <VSCodeDataGridCell gridColumn="1">{getResourceNameContents(scope, operation)}</VSCodeDataGridCell>
            <VSCodeDataGridCell gridColumn="2">{operation.properties?.targetResource?.resourceType}</VSCodeDataGridCell>
            <VSCodeDataGridCell gridColumn="3">{operation.properties?.provisioningOperation}</VSCodeDataGridCell>
            <VSCodeDataGridCell gridColumn="4">
              {isInProgress(operation) ? <VSCodeProgressRing /> : operation.properties?.provisioningState}
            </VSCodeDataGridCell>
            <VSCodeDataGridCell gridColumn="5">{getPreformattedJson(operation.properties?.statusMessage)}</VSCodeDataGridCell>
          </VSCodeDataGridRow>
        ))}
      </VSCodeDataGrid>
    </FormSection>
  );
};

function getResourceNameContents(scope: DeploymentScope, operation: DeploymentOperation) {
  const resourceId = operation.properties?.targetResource?.id;
  const resourceName = operation.properties?.targetResource?.resourceName;
  if (!resourceId) {
    return <>{resourceName}</>;
  }

  return <a href={`https://portal.azure.com/#@${scope.tenantId}/resource${resourceId}`}>{resourceName}</a>;
}