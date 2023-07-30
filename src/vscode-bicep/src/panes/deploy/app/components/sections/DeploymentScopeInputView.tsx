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
      {scope && <>
        <VSCodeTextField value={scope.subscriptionId} disabled={true}>
          Subscription Id
        </VSCodeTextField>
        <VSCodeTextField value={scope.resourceGroup} disabled={true}>
          Resource Group
        </VSCodeTextField>
      </>}
    </FormSection>
  );
};