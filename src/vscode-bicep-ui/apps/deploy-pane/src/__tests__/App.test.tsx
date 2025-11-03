// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { VscodeMessage } from "../messages";

import { act, fireEvent, render, screen, waitFor } from "@testing-library/react";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { App } from "../components/App";
import {
  createDeploymentDataMessage,
  createGetDeploymentScopeMessage,
  createGetDeploymentScopeResultMessage,
  createGetStateMessage,
  createGetStateResultMessage,
  createReadyMessage,
} from "../messages";
import { vscode } from "../vscode";
import {
  fileUri,
  getDeploymentOperations,
  getDeployResponse,
  getValidateResponse,
  getWhatIfResponse,
  parametersJson,
  scope,
  templateJson,
} from "./mockData";

const mockClient = {
  deployments: {
    beginCreateOrUpdateAtScope: vi.fn(async () => {
      return {
        isDone: vi.fn(() => true),
        getResult: vi.fn(getDeployResponse),
      };
    }),
    beginValidateAtScopeAndWait: vi.fn(async () => getValidateResponse()),
    beginWhatIfAndWait: vi.fn(async () => getWhatIfResponse()),
  },
  deploymentOperations: {
    listAtScope: vi.fn(getDeploymentOperations),
  },
};

beforeEach(() => {
  vi.mock("@azure/arm-resources", async (importOriginal) => {
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    const mod: any = await importOriginal();
    return {
      ...mod,
      ResourceManagementClient: vi.fn(() => mockClient),
    };
  });
  vi.mock("../components/hooks/time", () => ({
    getDate: () => "1737601964200",
  }));
});

afterEach(() => vi.resetAllMocks());

describe("App", () => {
  it("renders the loading spinner before initialization", async () => {
    const { container } = render(<App />);
    expect(container).toMatchSnapshot();
  });

  it("renders the App component with deployment state", async () => {
    const { container } = render(<App />);

    await initialize();

    expect(container).toMatchSnapshot();
  });

  it("runs a deployment", async () => {
    const { container } = render(<App />);

    await initialize();

    const deployButton = screen.getByText("Deploy");
    fireEvent.click(deployButton);

    await waitFor(() => {
      expect(screen.getAllByText("Succeeded")[0]).toBeInTheDocument();
    });

    expect(container).toMatchSnapshot();
  });

  it("handles synchronous deployment failures correctly", async () => {
    const { container } = render(<App />);

    await initialize();

    mockClient.deployments.beginCreateOrUpdateAtScope.mockImplementation(async () => {
      throw new Error("Deployment failed");
    });

    const deployButton = screen.getByText("Deploy");
    fireEvent.click(deployButton);

    await waitFor(() => {
      expect(screen.getAllByText("Failed")[0]).toBeInTheDocument();
    });

    expect(mockClient.deploymentOperations.listAtScope).not.toHaveBeenCalled();
    expect(container).toMatchSnapshot();
  });

  it("validates a deployment", async () => {
    const { container } = render(<App />);

    await initialize();

    const validateButton = screen.getByText("Validate");
    fireEvent.click(validateButton);

    await waitFor(() => {
      expect(screen.getAllByText("InvalidTemplate")[0]).toBeInTheDocument();
    });

    expect(container).toMatchSnapshot();
  });

  it("what-ifs a deployment", async () => {
    const { container } = render(<App />);

    await initialize();

    const whatIfButton = screen.getByText("What-If");
    fireEvent.click(whatIfButton);

    await waitFor(() => {
      expect(screen.getAllByText("Succeeded")[0]).toBeInTheDocument();
    });

    expect(container).toMatchSnapshot();
  });

  it("supports the scope picker", async () => {
    const { container } = render(<App />);

    const scope = await initialize();

    const changeScopeButton = screen.getByText("Change Scope");
    fireEvent.click(changeScopeButton);

    await act(async () => {
      await waitFor(() => expect(vscode.postMessage).toBeCalledWith(createGetDeploymentScopeMessage("resourceGroup")));
      sendMessage(
        createGetDeploymentScopeResultMessage({
          ...scope,
          tenantId: "newTenantId",
          subscriptionId: "newSubscriptionId",
          resourceGroup: "newResourceGroup",
        }),
      );
    });

    expect(container).toMatchSnapshot();
  });
});

async function initialize() {
  const stateResult = getStateResultMessage();
  await act(async () => {
    await waitFor(() => expect(vscode.postMessage).toBeCalledWith(createReadyMessage()));
    sendMessage(getDeploymentDataMessage());

    await waitFor(() => expect(vscode.postMessage).toBeCalledWith(createGetStateMessage()));
    sendMessage(stateResult);
  });

  return scope;
}

function sendMessage(message: VscodeMessage) {
  fireEvent(window, new MessageEvent<VscodeMessage>("message", { data: message }));
}

function getDeploymentDataMessage() {
  return createDeploymentDataMessage(fileUri, false, templateJson, parametersJson);
}

function getStateResultMessage() {
  return createGetStateResultMessage({
    scope: scope,
  });
}
