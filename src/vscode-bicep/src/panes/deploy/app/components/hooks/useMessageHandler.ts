import { useEffect, useState } from 'react';
import { vscode } from '../../vscode';
import { VscodeMessage, createGetDeploymentScopeMessage, createGetStateMessage, createPickParamsFileMessage, createReadyMessage, createSaveStateMessage } from '../../../messages';
import { parseParametersJson, parseTemplateJson } from '../utils';
import { DeployPaneState, DeploymentScope, ParamData, TemplateMetadata } from '../models';
import { AccessToken } from '@azure/identity';

export function useMessageHandler() {
  const [persistedState, setPersistedState] = useState<DeployPaneState>();
  const [templateMetadata, setTemplateMetadata] = useState<TemplateMetadata>();
  const [paramValues, setParamValues] = useState<Record<string, ParamData>>({});
  const [accessToken, setAccessToken] = useState<AccessToken>();
  const [scope, setScope] = useState<DeploymentScope>();

  const handleMessageEvent = (e: MessageEvent<VscodeMessage>) => {
    const message = e.data;
    console.log(`view received: ${JSON.stringify(message)}`);
    switch (message.kind) {
      case "DEPLOYMENT_DATA": {
        setTemplateMetadata(parseTemplateJson(message.templateJson));
        if (message.parametersJson) {
          setParamValues(parseParametersJson(message.parametersJson));
        }
        return;
      }
      case "GET_STATE_RESULT": {
        setPersistedState(message.state);
        setScope(message.state.scope);
        return;
      }
      case "PICK_PARAMS_FILE_RESULT": {
        setParamValues(parseParametersJson(message.parametersJson));
        return;
      }
      case "GET_ACCESS_TOKEN_RESULT": {
        setAccessToken(message.accessToken);
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

  function acquireAccessToken() {
    return new Promise<AccessToken>(
      (result, reject) => accessToken ? result(accessToken) : reject(`Failed to acquire access token`));
  }

  return {
    pickParamsFile,
    paramValues,
    setParamValues,
    templateMetadata,
    acquireAccessToken,
    pickScope,
    scope,
  };
}