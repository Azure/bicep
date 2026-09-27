// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Uri, ViewColumn } from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import {
  createResourceDeclarationInsertionRequestType,
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
  openTextDocument: vi.fn(),
  applyEdit: vi.fn(),
  document: {
    version: 1,
    isClosed: false,
    text: "base",
    uri: { toString: () => "file:///main.bicep" },
    getText: () => "",
    offsetAt: (position: { character: number }) => position.character,
    positionAt: (offset: number) => ({ line: 0, character: offset }),
  },
  resourceEditingEnabled: true,
  error: vi.fn(),
}));

vi.mock("vscode", () => {
  const disposable = () => ({ dispose: vi.fn() });
  return {
    Uri: {
      file: (fsPath: string) => ({ fsPath, toString: () => "file:///main.bicep" }),
      joinPath: () => "mock-resource-uri",
    },
    ViewColumn: { One: 1 },
    Range: class {
      constructor(
        public start: { line: number; character: number },
        public end: { line: number; character: number },
      ) {}
    },
    WorkspaceEdit: class {
      edits: { range: { start: { character: number }; end: { character: number } }; newText: string }[] = [];

      replace(_uri: unknown, range: { start: { character: number }; end: { character: number } }, newText: string) {
        this.edits.push({ range, newText });
      }

      entries() {
        return [[host.document.uri, this.edits]];
      }
    },
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
    workspace: { openTextDocument: host.openTextDocument, applyEdit: host.applyEdit },
    commands: { executeCommand: vi.fn() },
  };
});

vi.mock("vscode-languageclient/node", () => ({
  LanguageClient: class {
    sendRequest = host.sendRequest;
    code2ProtocolConverter = {
      asTextDocumentIdentifier: () => ({ uri: "file:///main.bicep" }),
      asVersionedTextDocumentIdentifier: (document: { version: number }) => ({
        uri: "file:///main.bicep",
        version: document.version,
      }),
    };
    protocol2CodeConverter = {
      asWorkspaceEdit: async (edit: unknown) => edit,
    };
  },
}));

vi.mock("../resource-editing-setting", () => ({
  isResourceEditingEnabled: () => host.resourceEditingEnabled,
}));

vi.mock("../../../infrastructure/logging", () => ({
  getLogger: () => ({ error: host.error, debug: vi.fn(), warn: vi.fn() }),
}));

describe("visualizer palette host requests", () => {
  let view: BicepVisualizerView;

  beforeEach(() => {
    vi.clearAllMocks();
    host.sendRequest.mockReset();
    host.resourceEditingEnabled = true;
    host.document.version = 1;
    host.document.isClosed = false;
    host.document.text = "base";
    host.document.getText = () => host.document.text;
    host.openTextDocument.mockResolvedValue(host.document);
    host.applyEdit.mockReset();
    host.applyEdit.mockImplementation(
      async (edit: {
        entries: () => [
          unknown,
          { range: { start: { character: number }; end: { character: number } }; newText: string }[],
        ][];
      }) => {
        const entries = edit.entries();
        const [change] = entries[0][1];
        const start = change.range.start.character;
        const end = change.range.end.character;
        host.document.text = host.document.text.slice(0, start) + change.newText + host.document.text.slice(end);
        host.document.version++;
        return true;
      },
    );
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

  const creationRequest = {
    version: 1,
    operationId: "create-1",
    resourceType: { fullyQualifiedType: "Microsoft.Storage/storageAccounts", apiVersion: "2024-01-01" },
  };
  const replayRequest = (direction: "undo" | "redo") => ({
    version: 1,
    operationId: creationRequest.operationId,
    direction,
    historyEpoch: 0,
  });
  const insertion = {
    operationId: "create-1",
    edit: {
      documentChanges: [
        {
          textDocument: { uri: "file:///main.bicep", version: 1 },
          edits: [
            {
              range: { start: { line: 0, character: 4 }, end: { line: 0, character: 4 } },
              newText: "\nresource storage",
            },
          ],
        },
      ],
      entries: () => [
        [
          host.document.uri,
          [
            {
              range: {
                isEmpty: true,
                start: { line: 0, character: 4 },
                end: { line: 0, character: 4 },
              },
              newText: "\nresource storage",
            },
          ],
        ],
      ],
    },
    expectedNodeId: "storage",
    symbolicName: "storage",
    unresolvedRequiredProperties: [],
  };

  it.each([
    ["missing payload", undefined],
    ["null payload", null],
    ["missing resource type", { version: 1, operationId: "create-1" }],
    ["non-string operation ID", { ...creationRequest, operationId: 42 }],
    ["blank API version", { ...creationRequest, resourceType: { ...creationRequest.resourceType, apiVersion: " " } }],
  ])("reports an invalid resource creation request: %s", async (_description, params) => {
    request("resources/create", params);

    await vi.waitFor(() =>
      expect(host.postMessage).toHaveBeenCalledWith({
        id: "request-1",
        error: expect.objectContaining({ retryable: false }),
      }),
    );
    expect(host.sendRequest).not.toHaveBeenCalled();
    expect(host.applyEdit).not.toHaveBeenCalled();
  });

  it("rejects resource creation without the opt-in before requesting an edit", async () => {
    host.resourceEditingEnabled = false;

    request("resources/create", creationRequest);

    await vi.waitFor(() =>
      expect(host.postMessage).toHaveBeenCalledWith({
        id: "request-1",
        error: {
          version: 1,
          operationId: "create-1",
          code: "editingDisabled",
          message: expect.stringContaining("Resource editing is disabled"),
          retryable: false,
        },
      }),
    );
    expect(host.sendRequest).not.toHaveBeenCalled();
    expect(host.applyEdit).not.toHaveBeenCalled();
  });

  it("applies a versioned source edit once through VS Code's edit history", async () => {
    host.sendRequest.mockResolvedValue(insertion);

    request("resources/create", creationRequest);

    await vi.waitFor(() => expect(host.applyEdit).toHaveBeenCalledOnce());
    expect(host.sendRequest).toHaveBeenCalledWith(createResourceDeclarationInsertionRequestType, {
      textDocument: { uri: "file:///main.bicep", version: 1 },
      operationId: creationRequest.operationId,
      resourceType: creationRequest.resourceType,
    });
    expect(host.applyEdit).toHaveBeenCalledWith(insertion.edit);
    await vi.waitFor(() =>
      expect(host.postMessage).toHaveBeenCalledWith({
        id: "request-1",
        result: {
          version: 1,
          operationId: insertion.operationId,
          expectedNodeId: insertion.expectedNodeId,
          symbolicName: insertion.symbolicName,
          unresolvedRequiredProperties: [],
          historyEpoch: 0,
          historyTrackingError: undefined,
        },
      }),
    );
    expect(host.document.text).toBe("base\nresource storage");
  });

  it("rejects an in-flight creation if the setting is turned off", async () => {
    let finishRequest!: (result: typeof insertion) => void;
    host.sendRequest.mockReturnValue(new Promise<typeof insertion>((resolve) => (finishRequest = resolve)));

    request("resources/create", creationRequest);
    await vi.waitFor(() => expect(host.sendRequest).toHaveBeenCalledOnce());
    host.resourceEditingEnabled = false;
    finishRequest(insertion);

    await vi.waitFor(() =>
      expect(host.postMessage).toHaveBeenCalledWith({
        id: "request-1",
        error: expect.objectContaining({ code: "editingDisabled", retryable: false }),
      }),
    );
    expect(host.applyEdit).not.toHaveBeenCalled();
  });

  it("rejects a stale document version before applying a source edit", async () => {
    host.sendRequest.mockImplementation(async () => {
      host.document.version = 2;
      return insertion;
    });

    request("resources/create", creationRequest);

    await vi.waitFor(() =>
      expect(host.postMessage).toHaveBeenCalledWith({
        id: "request-1",
        error: expect.objectContaining({ code: "documentChanged", retryable: true }),
      }),
    );
    expect(host.applyEdit).not.toHaveBeenCalled();
  });

  it("reports an editor rejection instead of claiming resource creation succeeded", async () => {
    host.sendRequest.mockResolvedValue(insertion);
    host.applyEdit.mockResolvedValue(false);

    request("resources/create", creationRequest);

    await vi.waitFor(() =>
      expect(host.postMessage).toHaveBeenCalledWith({
        id: "request-1",
        error: expect.objectContaining({ code: "editRejected", retryable: true }),
      }),
    );
    expect(host.postMessage).not.toHaveBeenCalledWith(
      expect.objectContaining({ id: "request-1", result: expect.anything() }),
    );
  });

  async function createTrackedResource() {
    host.sendRequest.mockResolvedValue(insertion);
    request("resources/create", creationRequest);
    await vi.waitFor(() =>
      expect(host.postMessage).toHaveBeenCalledWith({
        id: "request-1",
        result: expect.objectContaining({ historyTrackingError: undefined, historyEpoch: 0 }),
      }),
    );
  }

  it("replays only its own resource insertion through version-checked workspace edits", async () => {
    await createTrackedResource();

    request("undoHistory/replaySourceEdit", replayRequest("undo"));
    await vi.waitFor(() =>
      expect(host.postMessage).toHaveBeenCalledWith({
        id: "request-1",
        result: { version: 1, operationId: "create-1", direction: "undo", historyEpoch: 0 },
      }),
    );
    expect(host.document.text).toBe("base");

    request("undoHistory/replaySourceEdit", replayRequest("redo"));
    await vi.waitFor(() =>
      expect(host.postMessage).toHaveBeenCalledWith({
        id: "request-1",
        result: { version: 1, operationId: "create-1", direction: "redo", historyEpoch: 0 },
      }),
    );
    expect(host.document.text).toBe("base\nresource storage");
    expect(host.applyEdit).toHaveBeenCalledTimes(3);
  });

  it("refuses to undo a designer edit after the Bicep document changes directly", async () => {
    await createTrackedResource();
    host.document.text += "\noutside edit";
    host.document.version++;

    request("undoHistory/replaySourceEdit", replayRequest("undo"));

    await vi.waitFor(() =>
      expect(host.postMessage).toHaveBeenCalledWith({
        id: "request-1",
        error: expect.objectContaining({ code: "historyConflict", historyEpoch: 1, retryable: false }),
      }),
    );
    expect(host.document.text).toContain("outside edit");
    expect(host.applyEdit).toHaveBeenCalledOnce();
  });

  it("gates source undo when resource editing is turned off", async () => {
    await createTrackedResource();
    host.resourceEditingEnabled = false;

    request("undoHistory/replaySourceEdit", replayRequest("undo"));

    await vi.waitFor(() =>
      expect(host.postMessage).toHaveBeenCalledWith({
        id: "request-1",
        error: expect.objectContaining({ code: "editingDisabled", retryable: false }),
      }),
    );
    expect(host.applyEdit).toHaveBeenCalledOnce();
  });

  it("reports a failed source undo without consuming its history step", async () => {
    await createTrackedResource();
    host.applyEdit.mockResolvedValueOnce(false);

    request("undoHistory/replaySourceEdit", replayRequest("undo"));
    await vi.waitFor(() =>
      expect(host.postMessage).toHaveBeenCalledWith({
        id: "request-1",
        error: expect.objectContaining({ code: "editRejected", retryable: true }),
      }),
    );
    expect(host.document.text).toBe("base\nresource storage");

    request("undoHistory/replaySourceEdit", replayRequest("undo"));
    await vi.waitFor(() =>
      expect(host.postMessage).toHaveBeenCalledWith({
        id: "request-1",
        result: expect.objectContaining({ direction: "undo", historyEpoch: 0 }),
      }),
    );
    expect(host.document.text).toBe("base");
  });

  it("rejects a stale history epoch before touching the source", async () => {
    await createTrackedResource();

    request("undoHistory/replaySourceEdit", { ...replayRequest("undo"), historyEpoch: 1 });

    await vi.waitFor(() =>
      expect(host.postMessage).toHaveBeenCalledWith({
        id: "request-1",
        error: expect.objectContaining({ code: "historyConflict", historyEpoch: 0, retryable: false }),
      }),
    );
    expect(host.applyEdit).toHaveBeenCalledOnce();
  });

  it.each([undefined, null, { version: 1, operationId: "create-1", direction: "undo" }])(
    "rejects malformed designer replay requests: %j",
    async (params) => {
      request("undoHistory/replaySourceEdit", params);

      await vi.waitFor(() =>
        expect(host.postMessage).toHaveBeenCalledWith({
          id: "request-1",
          error: expect.objectContaining({ code: "invalidHistoryRequest", retryable: false }),
        }),
      );
      expect(host.applyEdit).not.toHaveBeenCalled();
    },
  );

  it("does not apply the same resource creation operation twice", async () => {
    await createTrackedResource();

    request("resources/create", creationRequest);

    await vi.waitFor(() =>
      expect(host.postMessage).toHaveBeenCalledWith({
        id: "request-1",
        error: expect.objectContaining({ code: "duplicateOperation", retryable: false }),
      }),
    );
    expect(host.applyEdit).toHaveBeenCalledOnce();
  });
});
