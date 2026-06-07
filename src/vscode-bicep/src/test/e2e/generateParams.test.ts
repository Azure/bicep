// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import fs from "fs";
import path from "path";
import { afterEach, describe, expect, it } from "@jest/globals";
import vscode from "vscode";
import { sleep } from "../../utils/time";
import { createUniqueTempFolder } from "../utils/createUniqueTempFolder";
import { executeCloseAllEditors, executeGenerateParamsCommand } from "./commands";

type ShowStringQuickPick = (
  items: readonly string[],
  options?: vscode.QuickPickOptions,
  token?: vscode.CancellationToken,
) => Thenable<string | undefined>;

describe("generateParams", (): void => {
  afterEach(async () => {
    jest.restoreAllMocks();
    await executeCloseAllEditors();
  });

  it("should generate parameters file if the compiled template already exists", async () => {
    const tempFolder = createUniqueTempFolder("bicep-generate-params-");

    try {
      const bicepFilePath = path.join(tempFolder, "main.bicep");
      const templateJsonPath = path.join(tempFolder, "main.json");
      const parametersJsonPath = path.join(tempFolder, "main.parameters.json");

      fs.writeFileSync(bicepFilePath, "param name string\noutput used string = name\n");
      fs.writeFileSync(
        templateJsonPath,
        '{ "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#" }',
      );

      const textDocument = await vscode.workspace.openTextDocument(bicepFilePath);

      // Give the language server some time to finish compilation.
      await sleep(2000);

      const showQuickPick = jest.spyOn(
        vscode.window,
        "showQuickPick",
      ) as unknown as jest.SpiedFunction<ShowStringQuickPick>;
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
    } finally {
      await executeCloseAllEditors();
      try {
        fs.rmSync(tempFolder, {
          recursive: true,
          maxRetries: 5,
          retryDelay: 1000,
        });
      } catch {
        // post-test cleanup is strictly best-effort only
      }
    }
  });
});
