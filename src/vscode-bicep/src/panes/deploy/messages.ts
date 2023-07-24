// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { AccessToken } from "@azure/identity";

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
  | ParametersDataMessage;

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
