// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { AccessToken } from "@azure/identity";
import { DeployPaneState } from "./state";

interface SimpleMessage<T> {
  kind: T;
}

type MessageWithPayload<T, U = Record<string, unknown>> = SimpleMessage<T> & U;

export type ReadyMessage = SimpleMessage<"READY">;
export type GetDeploymentScopeMessage = SimpleMessage<"GET_DEPLOYMENT_SCOPE">;
export type NewDeploymentScopeMessage = MessageWithPayload<
  "NEW_DEPLOYMENT_SCOPE",
  {
    subscriptionId: string;
    resourceGroup: string;
    accessToken: AccessToken;
  }
>;
export type DeploymentDataMessage = MessageWithPayload<
  "DEPLOYMENT_DATA",
  {
    documentPath: string;
    templateJson: string;
    parametersJson: string | null;
  }
>;
export type PickParametersFileMessage = SimpleMessage<"PICK_PARAMETERS_FILE">;
export type ParametersDataMessage = MessageWithPayload<
  "PARAMETERS_DATA",
  {
    documentPath: string;
    parametersJson: string;
  }
>;

export type Message =
  | ReadyMessage
  | GetDeploymentScopeMessage
  | NewDeploymentScopeMessage
  | DeploymentDataMessage
  | PickParametersFileMessage
  | ParametersDataMessage
  | GetStateMessage
  | GetStateResultMessage
  | SaveStateMessage;

function createSimpleMessage<T>(kind: T): SimpleMessage<T> {
  return { kind };
}

function createMessageWithPayload<
  T extends string,
  U = Record<string, unknown>
>(kind: T, payload: U): MessageWithPayload<T, U> {
  return {
    kind,
    ...payload,
  };
}

export function createReadyMessage(): ReadyMessage {
  return createSimpleMessage("READY");
}

export function createGetDeploymentScopeMessage(): GetDeploymentScopeMessage {
  return createSimpleMessage("GET_DEPLOYMENT_SCOPE");
}

export function createNewDeploymentScopeMessage(
  subscriptionId: string,
  resourceGroup: string,
  accessToken: AccessToken
): NewDeploymentScopeMessage {
  return createMessageWithPayload("NEW_DEPLOYMENT_SCOPE", {
    subscriptionId,
    resourceGroup,
    accessToken,
  });
}

export function createPickParametersFileMessage(): PickParametersFileMessage {
  return createSimpleMessage("PICK_PARAMETERS_FILE");
}

export function createParametersDataMessage(
  documentPath: string,
  parametersJson: string
): ParametersDataMessage {
  return createMessageWithPayload("PARAMETERS_DATA", {
    documentPath,
    parametersJson,
  });
}

export function createDeploymentDataMessage(
  documentPath: string,
  templateJson: string,
  parametersJson: string | null
): DeploymentDataMessage {
  return createMessageWithPayload("DEPLOYMENT_DATA", {
    documentPath,
    templateJson,
    parametersJson,
  });
}

export type GetStateMessage = SimpleMessage<"GET_STATE">;
export function createGetStateMessage(): GetStateMessage {
  return createSimpleMessage("GET_STATE");
}

export type GetStateResultMessage = MessageWithPayload<
  "GET_STATE_RESULT",
  {
    state: DeployPaneState
  }
>;
export function createGetStateResultMessage(
  state: DeployPaneState
): GetStateResultMessage {
  return createMessageWithPayload("GET_STATE_RESULT", {
    state
  });
}

export type SaveStateMessage = MessageWithPayload<
  "SAVE_STATE",
  {
    state: DeployPaneState
  }
>;
export function createSaveStateMessage(
  state: DeployPaneState
): SaveStateMessage {
  return createMessageWithPayload("SAVE_STATE", {
    state
  });
}