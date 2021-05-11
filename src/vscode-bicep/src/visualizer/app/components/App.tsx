// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// import cytoscape from "cytoscape";
import { useEffect, useState, VFC } from "react";
import { ElementDefinition } from "cytoscape";

import { StatusBar } from "./StatusBar";
import {
  Graph,
  createContainerNodeBackgroundUri,
  createChildlessNodeBackgroundUri,
} from "./Graph";

import { DeploymentGraphMessage, Message, READY_MESSAGE } from "../../messages";

declare function acquireVsCodeApi(): {
  postMessage(message: unknown): void;
  setState(state: unknown): void;
  getState<T>(): T;
};

const vscode = acquireVsCodeApi();

async function mapToElements(
  graph: DeploymentGraphMessage["deploymentGraph"]
): Promise<ElementDefinition[]> {
  if (!graph) {
    return [];
  }

  const nodes = await Promise.all(
    graph.nodes.map(async (node) => {
      const idSegments = node.id.split("::");
      const symbol = idSegments.pop() as string;
      const parent = idSegments.length > 0 ? idSegments.join("::") : undefined;

      return {
        data: {
          id: node.id,
          parent,
          hasError: node.hasError,
          backgroundDataUri: node.hasChildren
            ? await createContainerNodeBackgroundUri(symbol, node.isCollection)
            : await createChildlessNodeBackgroundUri(
                symbol,
                node.type,
                node.isCollection
              ),
        },
      };
    })
  );

  const edges = graph.edges.map(({ sourceId, targetId }) => ({
    data: {
      id: `${sourceId}>${targetId}`,
      source: sourceId,
      target: targetId,
    },
  }));

  return [...nodes, ...edges];
}

export const App: VFC = () => {
  const [elements, setElements] = useState<ElementDefinition[]>([]);
  const [errorCount, setErrorCount] = useState<number>(0);

  const handleMessageEvent = (e: MessageEvent<Message>) => {
    const message = e.data;
    if (message.kind === "DEPLOYMENT_GRAPH") {
      vscode.setState(message.documentPath);
      setErrorCount(message.deploymentGraph?.errorCount ?? 0);
      mapToElements(message.deploymentGraph).then(setElements);
    }
  };

  useEffect(() => {
    window.addEventListener("message", handleMessageEvent);
    vscode.postMessage(READY_MESSAGE);
    return () => {
      window.removeEventListener("message", handleMessageEvent);
    };
  }, []);

  return (
    <>
      <StatusBar errorCount={errorCount} hasNodes={elements.length > 0} />
      <Graph elements={elements} />
    </>
  );
};
