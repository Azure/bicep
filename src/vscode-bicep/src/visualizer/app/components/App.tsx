// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// import cytoscape from "cytoscape";
import { useEffect, useState, VFC } from "react";
import { ElementDefinition } from "cytoscape";
import { DefaultTheme, ThemeProvider } from "styled-components";

import { StatusBar } from "./StatusBar";
import {
  Graph,
  createContainerNodeBackgroundUri,
  createChildlessNodeBackgroundUri,
} from "./Graph";

import { DeploymentGraphMessage, Message, READY_MESSAGE } from "../../messages";
import { DeploymentGraph } from "../../../language";
import { darkTheme, lightTheme, highContrastTheme } from "./themes";

declare function acquireVsCodeApi(): {
  postMessage(message: unknown): void;
  setState(state: unknown): void;
  getState<T>(): T;
};

const vscode = acquireVsCodeApi();

async function mapToElements(
  graph: DeploymentGraphMessage["deploymentGraph"],
  theme: DefaultTheme
): Promise<ElementDefinition[]> {
  if (!graph) {
    return [];
  }

  console.log(theme.fontFamily);

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
            ? createContainerNodeBackgroundUri(
                symbol,
                node.isCollection,
                theme
              )
            : await createChildlessNodeBackgroundUri(
                symbol,
                node.type,
                node.isCollection,
                theme
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
  const [graph, setGraph] = useState<DeploymentGraph | null>(null);
  const [theme, setTheme] = useState<DefaultTheme>(darkTheme);

  const handleMessageEvent = (e: MessageEvent<Message>) => {
    const message = e.data;
    if (message.kind === "DEPLOYMENT_GRAPH") {
      vscode.setState(message.documentPath);
      setGraph(message.deploymentGraph);
    }
  };

  const applyTheme = (bodyClassName: string) => {
    switch (bodyClassName) {
      case "vscode-dark":
        setTheme(darkTheme);
        break;
      case "vscode-light":
        setTheme(lightTheme);
        break;
      case "vscode-high-contrast":
        setTheme(highContrastTheme);
        break;
    }
  };

  useEffect(() => {
    mapToElements(graph, theme).then(setElements);
  }, [graph, theme]);

  useEffect(() => {
    window.addEventListener("message", handleMessageEvent);
    vscode.postMessage(READY_MESSAGE);
    return () => window.removeEventListener("message", handleMessageEvent);
  }, []);

  useEffect(() => {
    applyTheme(document.body.className);

    const observer = new MutationObserver((mutationRecords) =>
      mutationRecords.forEach((mutationRecord) =>
        applyTheme((mutationRecord.target as HTMLElement).className)
      )
    );

    observer.observe(document.body, {
      attributes: true,
      attributeFilter: ["class"],
    });

    return () => observer.disconnect();
  }, []);

  return (
    <ThemeProvider theme={theme}>
      <Graph theme={theme} elements={elements} />
      <StatusBar
        errorCount={graph?.errorCount ?? 0}
        hasNodes={elements.length > 0}
      />
    </ThemeProvider>
  );
};
