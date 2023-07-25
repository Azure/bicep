import { useEffect, useState } from 'react';
import { vscode } from '../../vscode';
import { VscodeMessage, createGetAccessTokenMessage, createGetDeploymentScopeMessage, createGetStateMessage, createPickParamsFileMessage, createReadyMessage, createSaveStateMessage, createShowUserErrorDialogMessage } from '../../../messages';
import { parseParametersJson, parseTemplateJson } from '../utils';
import { DeployPaneState, DeploymentScope, ParametersMetadata, TemplateMetadata } from '../models';
import { AccessToken } from '@azure/identity';

// TODO see if there's a way to use react hooks instead of this hackery
let accessTokenResolver: {
  resolve: (accessToken: AccessToken) => void,
  reject: (error: any) => void,
};

export function useMessageHandler() {
  const [persistedState, setPersistedState] = useState<DeployPaneState>();
  const [templateMetadata, setTemplateMetadata] = useState<TemplateMetadata>();
  const [paramsMetadata, setParamsMetadata] = useState<ParametersMetadata>({ parameters: {} });
  const [scope, setScope] = useState<DeploymentScope>();

  const handleMessageEvent = (e: MessageEvent<VscodeMessage>) => {
    const message = e.data;
    switch (message.kind) {
      case "DEPLOYMENT_DATA": {
        const templateMetadata = parseTemplateJson(message.templateJson);
        setTemplateMetadata(templateMetadata);

        if (templateMetadata.template['$schema'] !== 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#') {
          showErrorDialog('handleMessageEvent', 'The deployment pane currently only supports resourceGroup-scoped Bicep files.');
          return;
        }

        if (message.parametersJson) {
          setParamsMetadata({
            sourceFilePath: message.documentPath.endsWith('.bicep') ? undefined : message.documentPath,
            parameters: parseParametersJson(message.parametersJson),
          });
        }
        return;
      }
      case "GET_STATE_RESULT": {
        setPersistedState(message.state);
        setScope(message.state.scope);
        return;
      }
      case "PICK_PARAMS_FILE_RESULT": {
        setParamsMetadata({
          sourceFilePath: message.documentPath,
          parameters: parseParametersJson(message.parametersJson),
        });
        return;
      }
      case "GET_ACCESS_TOKEN_RESULT": {
        if (message.accessToken) {
          accessTokenResolver.resolve(message.accessToken);
        } else {
          accessTokenResolver.reject(message.error ?? 'Failed to authenticate with Azure');
        }
        return;
      }
      case "GET_DEPLOYMENT_SCOPE_RESULT": {
        setScope(message.scope);
        savePersistedState({ ...persistedState, scope: message.scope });
        return;
      }
    }
  };

  useEffect(() => {
    window.addEventListener("message", handleMessageEvent);
    vscode.postMessage(createReadyMessage());
    vscode.postMessage(createGetStateMessage());
    return () => window.removeEventListener("message", handleMessageEvent);
  }, []);

  function savePersistedState(state: DeployPaneState) {
    vscode.postMessage(createSaveStateMessage(state));
    setPersistedState(state);
  }

  function pickParamsFile() {
    vscode.postMessage(createPickParamsFileMessage());
  }

  function pickScope() {
    vscode.postMessage(createGetDeploymentScopeMessage());
  }

  function showErrorDialog(callbackId: string, error: any) {
    vscode.postMessage(createShowUserErrorDialogMessage(callbackId, error));
  }

  function acquireAccessToken() {
    const promise = new Promise<AccessToken>(
      (resolve, reject) => accessTokenResolver = { resolve, reject });

    vscode.postMessage(createGetAccessTokenMessage(scope!));
    return promise;
  }

  return {
    showErrorDialog,
    pickParamsFile,
    paramsMetadata,
    setParamsMetadata,
    templateMetadata,
    acquireAccessToken,
    pickScope,
    scope,
  };
}