import { VSCodeTextField, VSCodeButton } from "@vscode/webview-ui-toolkit/react";
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
      <div className="controls">
        <VSCodeButton onClick={onPickScope} appearance={!scope ? "primary" : "secondary"}>Pick Scope</VSCodeButton>
      </div>

      {scope && (scope.scopeType === 'resourceGroup' || scope.scopeType === 'subscription') &&
        <VSCodeTextField value={scope.subscriptionId} disabled={true}>
          Subscription Id
        </VSCodeTextField>}
      {scope && (scope.scopeType === 'resourceGroup') &&
        <VSCodeTextField value={scope.resourceGroup} disabled={true}>
          Resource Group
        </VSCodeTextField>}
      {scope && scope.scopeType !== 'resourceGroup' &&
        <VSCodeTextField value={scope.location} disabled={true}>
          Location
        </VSCodeTextField>}
    </FormSection>
  );
};