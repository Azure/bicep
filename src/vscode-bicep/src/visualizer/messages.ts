// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import type { DeploymentGraph } from "../language/protocol";

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

export type Message = ReadyMessage | DeploymentGraphMessage;

export const READY_MESSAGE: ReadyMessage = {
  kind: "READY",
};

export function createDeploymentGraphMessage(
  documentPath: string,
  deploymentGraph: DeploymentGraph | null
): DeploymentGraphMessage {
  return {
    kind: "DEPLOYMENT_GRAPH",
    documentPath,
    deploymentGraph,
  };
}
