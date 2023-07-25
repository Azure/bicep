import { VSCodeTextField, VSCodeButton, VSCodeDivider } from "@vscode/webview-ui-toolkit/react";
import { FC } from "react";
import { DeploymentScope } from "../models";

interface DeploymentScopeInputViewProps {
  scope?: DeploymentScope;
  onPickScope: () => void;
}

export const DeploymentScopeInputView: FC<DeploymentScopeInputViewProps> = ({ scope, onPickScope }) => {
  return (
    <section className="form-section">
      <VSCodeDivider />
      <h2>Deployment Scope</h2>
      <div className="controls">
        <VSCodeButton onClick={onPickScope} appearance={!scope ? "primary" : "secondary"}>Pick Scope</VSCodeButton>
      </div>
      <VSCodeTextField value={scope?.subscriptionId} disabled={true}>
        Subscription Id
      </VSCodeTextField>
      <VSCodeTextField value={scope?.resourceGroup} disabled={true}>
        Resource Group
      </VSCodeTextField>
    </section>
  );
};