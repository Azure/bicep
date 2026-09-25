// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Uri, ViewColumn } from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import {
  visualGraphUpdateRequestType,
  visualResourceTypeNamespacesRequestType,
  visualResourceTypesRequestType,
  visualResourceTypeVersionsRequestType,
} from "../protocol";
import { BicepVisualizerView } from "../visualizer-view";

const host = vi.hoisted(() => ({
  receiveMessage: undefined as ((message: unknown) => void) | undefined,
  sendRequest: vi.fn(),
  postMessage: vi.fn().mockResolvedValue(true),
  openTextDocument: vi.fn().mockResolvedValue({}),
  error: vi.fn(),
}));

vi.mock("vscode", () => {
  const disposable = () => ({ dispose: vi.fn() });
  return {
    Uri: {
      file: (fsPath: string) => ({ fsPath }),
      joinPath: () => "mock-resource-uri",
    },
    ViewColumn: { One: 1 },
    EventEmitter: class {
      event = vi.fn(disposable);
      fire = vi.fn();
      dispose = vi.fn();
    },
    window: {
      createWebviewPanel: () => ({
        webview: {
          onDidReceiveMessage: (listener: (message: unknown) => void, receiver: unknown) => {
            host.receiveMessage = listener.bind(receiver);
            return disposable();
          },
          postMessage: host.postMessage,
          asWebviewUri: () => "mock-resource-uri",
          cspSource: "mock-csp",
        },
        onDidDispose: disposable,
        onDidChangeViewState: disposable,
        dispose: vi.fn(),
      }),
    },
    workspace: { openTextDocument: host.openTextDocument },
    commands: { executeCommand: vi.fn() },
  };
});

vi.mock("vscode-languageclient/node", () => ({
  LanguageClient: class {
    sendRequest = host.sendRequest;
    code2ProtocolConverter = {
      asTextDocumentIdentifier: () => ({ uri: "file:///main.bicep" }),
    };
  },
}));

vi.mock("../../../infrastructure/logging", () => ({
  getLogger: () => ({ error: host.error, debug: vi.fn(), warn: vi.fn() }),
}));

describe("visualizer palette host requests", () => {
  let view: BicepVisualizerView;

  beforeEach(() => {
    vi.clearAllMocks();
    host.sendRequest.mockReset();
    view = BicepVisualizerView.create(
      new LanguageClient("test", "Test", { command: "unused" }, {}),
      ViewColumn.One,
      Uri.file("extension"),
      Uri.file("main.bicep"),
    );
  });

  afterEach(() => view.dispose());

  function request(method: string, params?: unknown) {
    if (!host.receiveMessage) {
      throw new Error("The webview message listener was not registered.");
    }
    host.receiveMessage({ id: "request-1", method, params });
  }

  it("forwards target scope even when graph topology is unchanged", async () => {
    const result = { patches: [], targetScope: "subscription" };
    host.sendRequest.mockResolvedValue(result);

    request("getGraphUpdate", { current: null });

    await vi.waitFor(() => expect(host.postMessage).toHaveBeenCalledWith({ id: "request-1", result }));
    expect(host.sendRequest).toHaveBeenCalledWith(visualGraphUpdateRequestType, {
      textDocument: { uri: "file:///main.bicep" },
      current: null,
    });
  });

  it("loads API versions with previews and preserves their server ordering and catalog identity", async () => {
    const result = { catalogId: "catalog-a", apiVersions: ["2025-01-01-preview", "2024-01-01"] };
    host.sendRequest.mockResolvedValue(result);

    request("resourceTypeCatalog/versions", { fullyQualifiedType: " Microsoft.Storage/storageAccounts " });

    await vi.waitFor(() => expect(host.postMessage).toHaveBeenCalledWith({ id: "request-1", result }));
    expect(host.sendRequest).toHaveBeenCalledWith(visualResourceTypeVersionsRequestType, {
      textDocument: { uri: "file:///main.bicep" },
      fullyQualifiedType: "Microsoft.Storage/storageAccounts",
    });
  });

  it.each([undefined, null, "", {}, { fullyQualifiedType: 42 }, { fullyQualifiedType: " " }])(
    "rejects malformed version requests: %j",
    async (params) => {
      request("resourceTypeCatalog/versions", params);

      await vi.waitFor(() =>
        expect(host.postMessage).toHaveBeenCalledWith({
          id: "request-1",
          error: { message: "A resource type is required." },
        }),
      );
      expect(host.sendRequest).not.toHaveBeenCalled();
    },
  );

  it("reports version lookup failures instead of returning an empty successful catalog", async () => {
    host.sendRequest.mockRejectedValue(new Error("server unavailable"));

    request("resourceTypeCatalog/versions", { fullyQualifiedType: "Microsoft.Storage/storageAccounts" });

    await vi.waitFor(() =>
      expect(host.postMessage).toHaveBeenCalledWith({
        id: "request-1",
        error: { message: "Failed to load API versions for this resource type." },
      }),
    );
    expect(host.error).toHaveBeenCalledWith(expect.stringContaining("server unavailable"));
  });

  it("includes preview-only types in namespace discovery", async () => {
    const result = { catalogId: "catalog-a", namespaces: [{ name: "Test.Rp", resourceTypeCount: 1 }] };
    host.sendRequest.mockResolvedValue(result);

    request("resourceTypeCatalog/namespaces");

    await vi.waitFor(() => expect(host.postMessage).toHaveBeenCalledWith({ id: "request-1", result }));
    expect(host.sendRequest).toHaveBeenCalledWith(visualResourceTypeNamespacesRequestType, {
      textDocument: { uri: "file:///main.bicep" },
    });
  });

  it("includes previews when loading paged resource types without overriding the default version", async () => {
    host.sendRequest
      .mockResolvedValueOnce({
        catalogId: "catalog-a",
        items: [{ fullyQualifiedType: "Test.Rp/stable", apiVersion: "2024-01-01", isPreview: false }],
        continuationToken: "1",
      })
      .mockResolvedValueOnce({
        catalogId: "catalog-a",
        items: [{ fullyQualifiedType: "Test.Rp/previewOnly", apiVersion: "2025-01-01-preview", isPreview: true }],
      });

    request("resourceTypeCatalog/load", { providerNamespace: "Test.Rp" });

    await vi.waitFor(() =>
      expect(host.postMessage).toHaveBeenCalledWith({
        id: "request-1",
        result: {
          catalogId: "catalog-a",
          groups: [
            {
              group: "Test.Rp",
              resourceTypes: [
                { resourceType: "previewOnly", apiVersion: "2025-01-01-preview" },
                { resourceType: "stable", apiVersion: "2024-01-01" },
              ],
            },
          ],
        },
      }),
    );
    expect(host.sendRequest).toHaveBeenNthCalledWith(2, visualResourceTypesRequestType, {
      textDocument: { uri: "file:///main.bicep" },
      providerNamespace: "Test.Rp",
      query: undefined,
      pageSize: 200,
      continuationToken: "1",
    });
  });
});
