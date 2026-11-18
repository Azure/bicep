// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PromptItem } from "../../../infrastructure/prompts";

import { Uri } from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { UserCancelledError } from "../../../infrastructure/errors";
import { OutputChannelManager } from "../../../infrastructure/logging";
import { Prompts } from "../../../infrastructure/prompts";
import { ForceModulesRestoreCommand } from "../commands";

const host = vi.hoisted(() => ({
  activeTextEditor: undefined as { document: { languageId: string; uri: Uri } } | undefined,
  visibleTextEditors: [] as { document: { languageId: string; uri: Uri } }[],
  findFiles: vi.fn(),
  showQuickPick: vi.fn(),
  sendRequest: vi.fn(),
  error: vi.fn(),
  appendToOutputChannel: vi.fn(),
}));

vi.mock("vscode", () => ({
  Uri: {
    file: (path: string) => ({ scheme: "file", path, fsPath: path, toString: () => `file://${path}` }),
  },
  window: {
    get activeTextEditor() {
      return host.activeTextEditor;
    },
    get visibleTextEditors() {
      return host.visibleTextEditors;
    },
    showQuickPick: host.showQuickPick,
  },
  workspace: {
    findFiles: host.findFiles,
    getWorkspaceFolder: () => undefined,
  },
}));

vi.mock("vscode-languageclient/node", () => ({
  LanguageClient: class {
    sendRequest = host.sendRequest;
    error = host.error;
  },
}));

vi.mock("../../../infrastructure/logging", () => ({
  OutputChannelManager: class {
    appendToOutputChannel = host.appendToOutputChannel;
  },
}));

describe("ForceModulesRestoreCommand", () => {
  const bicepUri = Uri.file("/workspace/main.bicep");
  const paramUri = Uri.file("/workspace/environments/prod.bicepparam");
  let command: ForceModulesRestoreCommand;

  beforeEach(() => {
    vi.resetAllMocks();
    host.activeTextEditor = undefined;
    host.visibleTextEditors = [];
    host.findFiles.mockResolvedValue([bicepUri]);
    host.sendRequest.mockResolvedValue("Restore (force) succeeded.");
    command = new ForceModulesRestoreCommand(
      new Prompts(),
      new LanguageClient("test", "test", { command: "unused" }, {}),
      new OutputChannelManager("test"),
    );
  });

  it.each([
    { languageId: "bicep", uri: bicepUri },
    { languageId: "bicep-params", uri: paramUri },
  ])("restores the active $languageId file without searching the workspace", async ({ languageId, uri }) => {
    host.activeTextEditor = { document: { languageId, uri } };

    await command.execute();

    expect(host.sendRequest).toHaveBeenCalledExactlyOnceWith("workspace/executeCommand", {
      command: "forceModulesRestore",
      arguments: [uri.toString()],
    });
    expect(host.findFiles).not.toHaveBeenCalled();
    expect(host.showQuickPick).not.toHaveBeenCalled();
  });

  it("prefers an explicit parameter-file URI over the active editor", async () => {
    host.activeTextEditor = { document: { languageId: "bicep", uri: bicepUri } };

    await command.execute(paramUri);

    expect(host.sendRequest).toHaveBeenCalledExactlyOnceWith("workspace/executeCommand", {
      command: "forceModulesRestore",
      arguments: [paramUri.toString()],
    });
    expect(host.findFiles).not.toHaveBeenCalled();
  });

  it("searches for both file types and restores the selected parameter file", async () => {
    host.activeTextEditor = {
      document: { languageId: "plaintext", uri: Uri.file("/workspace/readme.txt") },
    };
    host.findFiles.mockResolvedValue([bicepUri, paramUri]);
    host.showQuickPick.mockImplementation(async (items: PromptItem<Uri>[]) =>
      items.find((item) => item.data === paramUri),
    );

    await command.execute();

    expect(host.findFiles).toHaveBeenCalledExactlyOnceWith("**/*.{bicep,bicepparam}", undefined);
    expect(host.showQuickPick).toHaveBeenCalledWith(
      expect.arrayContaining([
        expect.objectContaining({ data: bicepUri }),
        expect.objectContaining({ data: paramUri }),
      ]),
      expect.objectContaining({
        placeHolder: "Choose which .bicep or .bicepparam file to restore modules for",
      }),
    );
    expect(host.sendRequest).toHaveBeenCalledExactlyOnceWith("workspace/executeCommand", {
      command: "forceModulesRestore",
      arguments: [paramUri.toString()],
    });
  });

  it("restores a sole workspace parameter file without prompting", async () => {
    host.findFiles.mockResolvedValue([paramUri]);

    await command.execute();

    expect(host.sendRequest).toHaveBeenCalledExactlyOnceWith("workspace/executeCommand", {
      command: "forceModulesRestore",
      arguments: [paramUri.toString()],
    });
    expect(host.showQuickPick).not.toHaveBeenCalled();
  });

  it("includes visible parameter files outside the workspace without duplicating files", async () => {
    host.visibleTextEditors = [
      { document: { languageId: "bicep", uri: bicepUri } },
      { document: { languageId: "bicep-params", uri: paramUri } },
    ];
    host.showQuickPick.mockImplementation(async (items: PromptItem<Uri>[]) => items[0]);

    await command.execute();

    expect(host.showQuickPick.mock.calls[0][0]).toHaveLength(2);
    expect(host.showQuickPick.mock.calls[0][0]).toEqual(
      expect.arrayContaining([expect.objectContaining({ data: paramUri })]),
    );
  });

  it("does not restore when the file picker is cancelled", async () => {
    host.findFiles.mockResolvedValue([bicepUri, paramUri]);
    host.showQuickPick.mockResolvedValue(undefined);

    await expect(command.execute()).rejects.toBeInstanceOf(UserCancelledError);

    expect(host.sendRequest).not.toHaveBeenCalled();
  });
});
