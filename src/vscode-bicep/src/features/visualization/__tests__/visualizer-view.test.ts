// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PrepareVisualResourceReplayParams } from "../protocol";

import { Uri, ViewColumn } from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import {
  prepareVisualResourceCreationRequestType,
  prepareVisualResourceReplayRequestType,
  visualGraphLayoutRequestType,
  visualGraphRequestType,
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
  colorThemeMatched: true,
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
    workspace: {
      openTextDocument: host.openTextDocument,
      applyEdit: host.applyEdit,
      getConfiguration: () => ({ get: () => "on" }),
    },
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
      asRange: (range: unknown) => range,
    };
  },
}));

vi.mock("../resource-editing-setting", () => ({
  isResourceEditingEnabled: () => host.resourceEditingEnabled,
}));

vi.mock("../color-theme-setting", () => ({
  isColorThemeMatched: () => host.colorThemeMatched,
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
    host.colorThemeMatched = true;
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

  it("forwards the measured graph for layout and returns the positions", async () => {
    const graph = { nodes: [{ id: "a", kind: "resource", parentId: null, width: 200, height: 76 }], edges: [] };
    const result = { status: "ok", positions: [{ nodeId: "a", x: 0, y: 0 }], bounds: { width: 200, height: 76 } };
    host.sendRequest.mockResolvedValue(result);

    request("graph/layout", { graph });

    await vi.waitFor(() => expect(host.postMessage).toHaveBeenCalledWith({ id: "request-1", result }));
    expect(host.sendRequest).toHaveBeenCalledWith(visualGraphLayoutRequestType, {
      textDocument: { uri: "file:///main.bicep" },
      graph,
    });
  });

  it("forwards target scope even when graph topology is unchanged", async () => {
    const result = { graph: { nodes: [], edges: [] }, targetScope: "subscription", errorCount: 2 };
    host.sendRequest.mockResolvedValue(result);

    request("graph/get", {});

    await vi.waitFor(() =>
      expect(host.postMessage).toHaveBeenCalledWith({
        id: "request-1",
        result: { ...result, replayableSourceSteps: [] },
      }),
    );
    expect(host.sendRequest).toHaveBeenCalledWith(visualGraphRequestType, {
      textDocument: { uri: "file:///main.bicep" },
    });
  });

  it("loads API versions with previews and preserves their server ordering and catalog identity", async () => {
    const result = { catalogId: "catalog-a", apiVersions: ["2025-01-01-preview", "2024-01-01"] };
    host.sendRequest.mockResolvedValue(result);

    request("resourceTypes/versions", { fullyQualifiedType: " Microsoft.Storage/storageAccounts " });

    await vi.waitFor(() => expect(host.postMessage).toHaveBeenCalledWith({ id: "request-1", result }));
    expect(host.sendRequest).toHaveBeenCalledWith(visualResourceTypeVersionsRequestType, {
      textDocument: { uri: "file:///main.bicep" },
      fullyQualifiedType: "Microsoft.Storage/storageAccounts",
    });
  });

  it.each([undefined, null, "", {}, { fullyQualifiedType: 42 }, { fullyQualifiedType: " " }])(
    "rejects malformed version requests: %j",
    async (params) => {
      request("resourceTypes/versions", params);

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

    request("resourceTypes/versions", { fullyQualifiedType: "Microsoft.Storage/storageAccounts" });

    await vi.waitFor(() =>
      expect(host.postMessage).toHaveBeenCalledWith({
        id: "request-1",
        error: { message: "Failed to load API versions for this resource type." },
      }),
    );
    expect(host.error).toHaveBeenCalledWith(expect.stringContaining("server unavailable"));
  });

  it("forwards the known catalog and returns every resource type, including previews", async () => {
    host.sendRequest.mockResolvedValue({
      catalogId: "catalog-a",
      resourceTypes: [
        { fullyQualifiedType: "Test.Rp/stable", apiVersion: "2024-01-01", isPreview: false },
        { fullyQualifiedType: "Test.Rp/previewOnly", apiVersion: "2025-01-01-preview", isPreview: true },
      ],
    });

    request("resourceTypes/list", { knownCatalogId: "catalog-old" });

    await vi.waitFor(() =>
      expect(host.postMessage).toHaveBeenCalledWith({
        id: "request-1",
        result: {
          catalogId: "catalog-a",
          resourceTypes: [
            { fullyQualifiedType: "Test.Rp/stable", apiVersion: "2024-01-01" },
            { fullyQualifiedType: "Test.Rp/previewOnly", apiVersion: "2025-01-01-preview" },
          ],
        },
      }),
    );
    expect(host.sendRequest).toHaveBeenCalledWith(visualResourceTypesRequestType, {
      textDocument: { uri: "file:///main.bicep" },
      knownCatalogId: "catalog-old",
    });
  });

  it("reports an unchanged catalog without resending its types", async () => {
    host.sendRequest.mockResolvedValue({ catalogId: "catalog-a", resourceTypes: null });

    request("resourceTypes/list", { knownCatalogId: "catalog-a" });

    await vi.waitFor(() =>
      expect(host.postMessage).toHaveBeenCalledWith({
        id: "request-1",
        result: { catalogId: "catalog-a", resourceTypes: null },
      }),
    );
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
    expect(host.sendRequest).toHaveBeenCalledWith(prepareVisualResourceCreationRequestType, {
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
          unresolvedRequiredProperties: [],
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

  /** Answers like the language server: undo while the creation's text is present, redo (at the end) while it is not. */
  function answerLikeLanguageServer(type: unknown, params: unknown) {
    if (type === prepareVisualResourceCreationRequestType) {
      return Promise.resolve(insertion);
    }
    if (type === visualGraphRequestType) {
      return Promise.resolve({ graph: null, targetScope: null, errorCount: 0 });
    }
    if (type === prepareVisualResourceReplayRequestType) {
      const { replays } = params as PrepareVisualResourceReplayParams;
      return Promise.resolve({
        replays: replays.map(({ operationId, direction, insertedText }) => {
          const text = host.document.text;
          const offset = text.indexOf(insertedText);
          const range = (start: number, end: number) => ({
            start: { line: 0, character: start },
            end: { line: 0, character: end },
          });
          const edit =
            direction === "undo"
              ? offset < 0
                ? null
                : { range: range(offset, offset + insertedText.length), newText: "" }
              : offset >= 0
                ? null
                : { range: range(text.length, text.length), newText: insertedText };
          return { operationId, edit };
        }),
      });
    }
    return Promise.reject(new Error("Unexpected request."));
  }

  async function createTrackedResource() {
    host.sendRequest.mockImplementation(answerLikeLanguageServer);
    request("resources/create", creationRequest);
    await vi.waitFor(() =>
      expect(host.postMessage).toHaveBeenCalledWith({
        id: "request-1",
        result: expect.objectContaining({ historyTrackingError: undefined }),
      }),
    );
    host.postMessage.mockClear();
  }

  function replayResponse(direction: "undo" | "redo") {
    return { id: "request-1", result: { version: 1, operationId: "create-1", direction } };
  }

  it("sends the settings the webview depends on once it is ready", () => {
    host.receiveMessage?.({ method: "webview/ready" });

    expect(host.postMessage).toHaveBeenCalledWith({
      method: "settings/didChange",
      params: { motionPolicy: "reduce", isResourceEditingEnabled: true, isColorThemeMatched: true },
    });
  });

  it("resends the settings when one changes", () => {
    host.receiveMessage?.({ method: "webview/ready" });
    host.postMessage.mockClear();
    host.colorThemeMatched = false;

    view.notifySettingsDidChange();

    expect(host.postMessage).toHaveBeenCalledWith({
      method: "settings/didChange",
      params: { motionPolicy: "reduce", isResourceEditingEnabled: true, isColorThemeMatched: false },
    });
  });

  it("tells the webview about every document change right away", async () => {
    host.receiveMessage?.({ method: "webview/ready" });
    await vi.waitFor(() =>
      expect(host.postMessage).toHaveBeenCalledWith(expect.objectContaining({ method: "document/didChange" })),
    );
    host.postMessage.mockClear();

    view.render();

    await vi.waitFor(() =>
      expect(host.postMessage).toHaveBeenCalledWith({
        method: "document/didChange",
        params: { documentUri: "main.bicep" },
      }),
    );
  });

  it("reports with each graph update which designer source steps can be replayed exactly", async () => {
    await createTrackedResource();

    request("graph/get", {
      sourceSteps: [
        { operationId: "create-1", direction: "undo" },
        { operationId: "create-1", direction: "redo" },
        { operationId: "unknown", direction: "undo" },
        { operationId: 42, direction: "undo" },
      ],
    });

    await vi.waitFor(() =>
      expect(host.postMessage).toHaveBeenCalledWith({
        id: "request-1",
        result: {
          graph: null,
          targetScope: null,
          errorCount: 0,
          replayableSourceSteps: [{ operationId: "create-1", direction: "undo" }],
        },
      }),
    );
    expect(host.sendRequest).toHaveBeenCalledWith(prepareVisualResourceReplayRequestType, {
      textDocument: { uri: "file:///main.bicep" },
      replays: [
        { operationId: "create-1", direction: "undo", nodeId: "storage", insertedText: "\nresource storage" },
        { operationId: "create-1", direction: "redo", nodeId: "storage", insertedText: "\nresource storage" },
      ],
    });
  });

  it("reports no replayable steps once the created declaration is edited", async () => {
    await createTrackedResource();
    host.document.text = "base\nresource renamed";
    host.document.version++;

    request("graph/get", { sourceSteps: [{ operationId: "create-1", direction: "undo" }] });

    await vi.waitFor(() =>
      expect(host.postMessage).toHaveBeenCalledWith({
        id: "request-1",
        result: expect.objectContaining({ replayableSourceSteps: [] }),
      }),
    );
  });

  it("reports replay availability as unknown when the language server cannot answer", async () => {
    await createTrackedResource();
    host.sendRequest.mockImplementation((type: unknown, params: unknown) =>
      type === prepareVisualResourceReplayRequestType
        ? Promise.reject(new Error("server unavailable"))
        : answerLikeLanguageServer(type, params),
    );

    request("graph/get", { sourceSteps: [{ operationId: "create-1", direction: "undo" }] });

    await vi.waitFor(() =>
      expect(host.postMessage).toHaveBeenCalledWith({
        id: "request-1",
        result: { graph: null, targetScope: null, errorCount: 0, replayableSourceSteps: null },
      }),
    );
  });

  it("undoes and redoes a creation with the language server's edits", async () => {
    await createTrackedResource();

    request("history/replaySourceStep", replayRequest("undo"));
    await vi.waitFor(() => expect(host.postMessage).toHaveBeenCalledWith(replayResponse("undo")));
    expect(host.document.text).toBe("base");

    request("history/replaySourceStep", replayRequest("redo"));
    await vi.waitFor(() => expect(host.postMessage).toHaveBeenCalledWith(replayResponse("redo")));
    expect(host.document.text).toBe("base\nresource storage");
    expect(host.applyEdit).toHaveBeenCalledTimes(3);
  });

  it("undoes a creation after unrelated changes to the Bicep file", async () => {
    await createTrackedResource();
    host.document.text = `\n${host.document.text}\n`;
    host.document.version++;

    request("history/replaySourceStep", replayRequest("undo"));

    await vi.waitFor(() => expect(host.postMessage).toHaveBeenCalledWith(replayResponse("undo")));
    expect(host.document.text).toBe("\nbase\n");
  });

  it("refuses a replay the language server can no longer prepare", async () => {
    await createTrackedResource();
    host.document.text = "base\nresource renamed";
    host.document.version++;

    request("history/replaySourceStep", replayRequest("undo"));

    await vi.waitFor(() =>
      expect(host.postMessage).toHaveBeenCalledWith({
        id: "request-1",
        error: expect.objectContaining({ code: "replayUnavailable", retryable: false }),
      }),
    );
    expect(host.document.text).toBe("base\nresource renamed");
    expect(host.applyEdit).toHaveBeenCalledOnce();
  });

  it("refuses a replay if the file changes while its edit is being prepared", async () => {
    await createTrackedResource();
    host.sendRequest.mockImplementation(async (type: unknown, params: unknown) => {
      const result = await answerLikeLanguageServer(type, params);
      host.document.version++;
      return result;
    });

    request("history/replaySourceStep", replayRequest("undo"));

    await vi.waitFor(() =>
      expect(host.postMessage).toHaveBeenCalledWith({
        id: "request-1",
        error: expect.objectContaining({ code: "documentChanged", retryable: true }),
      }),
    );
    expect(host.applyEdit).toHaveBeenCalledOnce();
  });

  it("refuses to track a replay that leaves the file in an unexpected state", async () => {
    await createTrackedResource();
    host.applyEdit.mockImplementationOnce(async () => {
      host.document.text = "unexpected";
      host.document.version++;
      return true;
    });

    request("history/replaySourceStep", replayRequest("undo"));
    await vi.waitFor(() =>
      expect(host.postMessage).toHaveBeenCalledWith({
        id: "request-1",
        error: expect.objectContaining({ code: "replayUnavailable", retryable: false }),
      }),
    );

    host.postMessage.mockClear();
    request("graph/get", { sourceSteps: [{ operationId: "create-1", direction: "redo" }] });
    await vi.waitFor(() =>
      expect(host.postMessage).toHaveBeenCalledWith({
        id: "request-1",
        result: expect.objectContaining({ replayableSourceSteps: [] }),
      }),
    );
  });

  it("gates source undo when resource editing is turned off", async () => {
    await createTrackedResource();
    host.resourceEditingEnabled = false;

    request("history/replaySourceStep", replayRequest("undo"));

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

    request("history/replaySourceStep", replayRequest("undo"));
    await vi.waitFor(() =>
      expect(host.postMessage).toHaveBeenCalledWith({
        id: "request-1",
        error: expect.objectContaining({ code: "editRejected", retryable: true }),
      }),
    );
    expect(host.document.text).toBe("base\nresource storage");

    request("history/replaySourceStep", replayRequest("undo"));
    await vi.waitFor(() => expect(host.postMessage).toHaveBeenCalledWith(replayResponse("undo")));
    expect(host.document.text).toBe("base");
  });

  it.each([undefined, null, { version: 1, operationId: "create-1" }, { ...replayRequest("undo"), version: 2 }])(
    "rejects malformed designer replay requests: %j",
    async (params) => {
      request("history/replaySourceStep", params);

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
