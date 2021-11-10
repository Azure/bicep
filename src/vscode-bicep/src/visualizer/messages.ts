// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import type { DeploymentGraph } from "../language/protocol";
import vscode from "vscode";

interface SimpleMessage<T> {
  kind: T;
}

type MessageWithPayload<T, U = Record<string, unknown>> = SimpleMessage<T> & U;

export type ReadyMessage = SimpleMessage<"READY">;

export type DeploymentGraphMessage = MessageWithPayload<
  "DEPLOYMENT_GRAPH",
  {
    documentPath: string;
    deploymentGraph: DeploymentGraph | null;
  }
>;

export type RevealResourceMessage = MessageWithPayload<
  "REVEAL_RESOURCE",
  {
    filePath: string;
    range: vscode.Range;
  }
>;

export type Message =
  | ReadyMessage
  | DeploymentGraphMessage
  | RevealResourceMessage;

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

export const READY_MESSAGE: ReadyMessage = createSimpleMessage("READY");

export function createDeploymentGraphMessage(
  documentPath: string,
  deploymentGraph: DeploymentGraph | null
): DeploymentGraphMessage {
  return createMessageWithPayload("DEPLOYMENT_GRAPH", {
    documentPath,
    deploymentGraph,
  });
}

export function createRevealResourceMessage(
  filePath: string,
  range: vscode.Range
): RevealResourceMessage {
  return createMessageWithPayload("REVEAL_RESOURCE", {
    filePath,
    range,
  });
}
