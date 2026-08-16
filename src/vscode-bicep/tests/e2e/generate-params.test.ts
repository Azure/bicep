// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import type { MockInstance } from "vitest" with { "resolution-mode": "import" };

import fs from "fs";
import path from "path";
import vscode from "vscode";
import { executeCloseAllEditors, executeGenerateParamsCommand } from "./utils/commands";
import { withTempDirectory } from "./utils/temp-directory";

type ShowStringQuickPick = (
  items: readonly string[],
  options?: vscode.QuickPickOptions,
  token?: vscode.CancellationToken,
) => Thenable<string | undefined>;

describe("generateParams", (): void => {
  afterEach(async () => {
    vi.restoreAllMocks();
    await executeCloseAllEditors();
  });

  it("should generate parameters file if the compiled template already exists", async () => {
    await withTempDirectory("bicep-generate-params-", async (tempFolder) => {
      const bicepFilePath = path.join(tempFolder, "main.bicep");
      const templateJsonPath = path.join(tempFolder, "main.json");
      const parametersJsonPath = path.join(tempFolder, "main.parameters.json");

      fs.writeFileSync(bicepFilePath, "param name string\noutput used string = name\n");
      fs.writeFileSync(
        templateJsonPath,
        '{ "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#" }',
      );

      const textDocument = await vscode.workspace.openTextDocument(bicepFilePath);

      const showQuickPick = vi.spyOn(vscode.window, "showQuickPick") as unknown as MockInstance<ShowStringQuickPick>;
      showQuickPick.mockResolvedValueOnce("json").mockResolvedValueOnce("requiredonly");

      await executeGenerateParamsCommand(textDocument.uri);

      expect(fs.existsSync(parametersJsonPath)).toBe(true);
      expect(fs.existsSync(templateJsonPath)).toBe(true);
      expect(showQuickPick).toHaveBeenNthCalledWith(
        1,
        ["json", "bicepparam"],
        expect.objectContaining({ title: "Please select the output format" }),
      );
      expect(showQuickPick).toHaveBeenNthCalledWith(
        2,
        ["requiredonly", "all"],
        expect.objectContaining({ title: "Please select which parameters to include" }),
      );
      await executeCloseAllEditors();
    });
  });
});
