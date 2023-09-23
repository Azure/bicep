import { VSCodeButton, VSCodeBadge, VSCodeDataGrid, VSCodeDataGridCell, VSCodeDataGridRow } from "@vscode/webview-ui-toolkit/react";
import { FC } from "react";
import { DeploymentScope } from "../../../models";
import { FormSection } from "./FormSection";

interface DeploymentScopeInputViewProps {
  scope?: DeploymentScope;
  onPickScope: () => void;
}

export const DeploymentScopeInputView: FC<DeploymentScopeInputViewProps> = ({ scope, onPickScope }) => {
  return (
    <FormSection title="Deployment Scope">
      <VSCodeDataGrid>
      {scope && (scope.scopeType === 'resourceGroup' || scope.scopeType === 'subscription') && getGridRow('Subscription Id', scope.subscriptionId)}
      {scope && (scope.scopeType === 'resourceGroup') && getGridRow('Resource Group', scope.resourceGroup)}
      {scope && (scope.scopeType === 'managementGroup' || scope.scopeType === 'tenant') && getGridRow('Tenant Id', scope.tenantId)}
      {scope && (scope.scopeType === 'managementGroup') && getGridRow('Management Group', scope.managementGroup)}
      {scope && (scope.scopeType === 'managementGroup' || scope.scopeType === 'tenant') && getGridRow('Authenticated Subscription Id', scope.associatedSubscriptionId)}
      {scope && scope.scopeType !== 'resourceGroup' && getGridRow('Location', scope.location)}
      </VSCodeDataGrid>

      <div className="controls">
        <VSCodeButton onClick={onPickScope} appearance={!scope ? "primary" : "secondary"}>{!scope ? "Pick Scope" : "Change Scope"}</VSCodeButton>
      </div>
    </FormSection>
  );
};

function getGridRow(label: string, value: string) {
  return (
    <VSCodeDataGridRow key={label}>
      <VSCodeDataGridCell gridColumn="1">{label}</VSCodeDataGridCell>
      <VSCodeDataGridCell gridColumn="2"><VSCodeBadge>{value}</VSCodeBadge></VSCodeDataGridCell>
    </VSCodeDataGridRow>
  )
}