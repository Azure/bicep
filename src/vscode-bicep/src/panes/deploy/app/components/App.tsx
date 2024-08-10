// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { VSCodeButton, VSCodeProgressRing } from "@vscode/webview-ui-toolkit/react";
import { FC, useState } from "react";

import "./index.css";

import { ParamData } from "../../models";
import { useAzure } from "./hooks/useAzure";
import { useMessageHandler } from "./hooks/useMessageHandler";
import { LocalDeployOperations, LocalDeployOutputs, LocalDeployResult } from "./localDeploy";
import { DeploymentOperationsView } from "./sections/DeploymentOperationsView";
import { DeploymentOutputsView } from "./sections/DeploymentOutputsView";
import { DeploymentScopeInputView } from "./sections/DeploymentScopeInputView";
import { FormSection } from "./sections/FormSection";
import { ParametersInputView } from "./sections/ParametersInputView";
import { ResultsView } from "./sections/ResultsView";
import { WhatIfChangesView } from "./sections/WhatIfChangesView";
import { Codicon } from "@vscode-bicep-ui/components";

export const App: FC = () => {
  const [errorMessage, setErrorMessage] = useState<string>();
  const [localDeployRunning, setLocalDeployRunning] = useState(false);
  const messages = useMessageHandler({ setErrorMessage, setLocalDeployRunning });
  const azure = useAzure({
    scope: messages.scope,
    acquireAccessToken: messages.acquireAccessToken,
    templateMetadata: messages.templateMetadata,
    parametersMetadata: messages.paramsMetadata,
    setErrorMessage,
  });

  function setParamValue(key: string, data: ParamData) {
    const parameters = Object.assign({}, messages.paramsMetadata.parameters, { [key]: data });
    messages.setParamsMetadata({ ...messages.paramsMetadata, parameters });
  }

  function handleEnableParamEditing() {
    messages.setParamsMetadata({ ...messages.paramsMetadata, sourceFilePath: undefined });
  }

  const azureDisabled = !messages.scope || !messages.templateMetadata || azure.running;

  async function handleDeployClick() {
    messages.publishTelemetry("deployPane/deploy", {});
    await azure.deploy();
  }

  async function handleValidateClick() {
    messages.publishTelemetry("deployPane/validate", {});
    await azure.validate();
  }

  async function handleWhatIfClick() {
    messages.publishTelemetry("deployPane/whatIf", {});
    await azure.whatIf();
  }

  async function handleLocalDeployClick() {
    messages.publishTelemetry("deployPane/localDeploy", {});
    messages.startLocalDeploy();
  }

  if (!messages.messageState.initialized) {
    return <VSCodeProgressRing />;
  }

  const showLocalDeployControls =
    messages.messageState.localDeployEnabled &&
    // if there's an error, this'll cause sourceFilePath to be empty - we still want to show the controls to display the error
    (errorMessage || messages.paramsMetadata.sourceFilePath?.endsWith(".bicepparam"));

  return (
    <main id="webview-body">
      {!messages.messageState.localDeployEnabled && (
        <>
          <FormSection title="Experimental Warning">
            <div className="alert-error">
              <Codicon name="beaker" size={14} />
              The Bicep Deployment Pane is an experimental feature.
              <br />
              Documentation is available{" "}
              <a href="https://github.com/Azure/bicep/blob/main/docs/experimental/deploy-ui.md">here</a>. Please raise
              issues or feature requests <a href={createNewIssueUrl()}>here</a>.
            </div>
          </FormSection>
          <DeploymentScopeInputView scope={messages.scope} onPickScope={messages.pickScope} />

          <ParametersInputView
            parameters={messages.paramsMetadata}
            template={messages.templateMetadata}
            disabled={azure.running}
            onValueChange={setParamValue}
            onEnableEditing={handleEnableParamEditing}
            onPickParametersFile={messages.pickParamsFile}
          />

          <FormSection title="Actions">
            {errorMessage && (
              <div className="alert-error">
                <Codicon name="error" size={14} />
                {errorMessage}
              </div>
            )}
            <div className="controls">
              <VSCodeButton onClick={handleDeployClick} disabled={azureDisabled}>
                Deploy
              </VSCodeButton>
              <VSCodeButton onClick={handleValidateClick} disabled={azureDisabled}>
                Validate
              </VSCodeButton>
              <VSCodeButton onClick={handleWhatIfClick} disabled={azureDisabled}>
                What-If
              </VSCodeButton>
            </div>
            {azure.running && <VSCodeProgressRing></VSCodeProgressRing>}
          </FormSection>

          {messages.scope && (
            <>
              <ResultsView result={azure.result} />
              <DeploymentOperationsView scope={messages.scope} operations={azure.operations} />
              <DeploymentOutputsView outputs={azure.outputs} />
              <WhatIfChangesView changes={azure.whatIfChanges} />
            </>
          )}
        </>
      )}

      {messages.messageState.localDeployEnabled && (
        <>
          <FormSection title="Experimental Warning">
            <div className="alert-error">
                <Codicon name="error" size={14} />
              Local Deployment is an experimental feature.
            </div>
          </FormSection>
          {showLocalDeployControls && (
            <>
              <ParametersInputView
                parameters={messages.paramsMetadata}
                template={messages.templateMetadata}
                disabled={localDeployRunning}
                onValueChange={setParamValue}
                onEnableEditing={handleEnableParamEditing}
                onPickParametersFile={messages.pickParamsFile}
              />

              <FormSection title="Actions">
                {errorMessage && (
                  <div className="alert-error">
                    <Codicon name="error" size={14} />
                    {errorMessage}
                  </div>
                )}
                <div className="controls">
                  <VSCodeButton onClick={handleLocalDeployClick} disabled={localDeployRunning}>
                    Deploy
                  </VSCodeButton>
                </div>
                {localDeployRunning && <VSCodeProgressRing></VSCodeProgressRing>}
              </FormSection>

              {!localDeployRunning && messages.localDeployResult && (
                <>
                  <LocalDeployResult result={messages.localDeployResult} />
                  <LocalDeployOperations result={messages.localDeployResult} />
                  <LocalDeployOutputs result={messages.localDeployResult} />
                </>
              )}
            </>
          )}
          {!showLocalDeployControls && (
            <>
              <div className="alert-error">
                Local Deployment is only currently supported for .bicepparam files. Relaunch this pane for a .bicepparam
                file.
              </div>
            </>
          )}
        </>
      )}
    </main>
  );
};

function createNewIssueUrl() {
  const title = "Deployment Pane: <description>";
  const labels = ["story: deploy pane"];
  const body = `

`;

  return `https://github.com/Azure/bicep/issues/new?title=${encodeURIComponent(title)}&body=${encodeURIComponent(body)}&labels=${encodeURIComponent(labels.join(","))}`;
}
