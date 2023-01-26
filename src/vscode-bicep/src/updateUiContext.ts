// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { commands, env, TextDocument } from "vscode";
import { DecompileCommand } from "./commands/decompile";
import {
  callWithTelemetryAndErrorHandling,
  IActionContext,
} from "@microsoft/vscode-azext-utils";
import { PasteAsBicepCommand } from "./commands/pasteAsBicep";
import { bicepLanguageId } from "./language/constants";

const cachedCanPasteAsBicep = {
  clipboardText: "",
  canPasteAsBicep: false,
};

export async function updateUiContext(
  currentDocument: TextDocument | undefined,
  pasteAsBicepCommand?: PasteAsBicepCommand // Pass this in if you want to check for canPasteAsBicep
): Promise<void> {
  await callWithTelemetryAndErrorHandling(
    "updateUiContext",
    async (context: IActionContext) => {
      context.telemetry.suppressIfSuccessful = true;

      let cannotDecompile: boolean;
      switch (currentDocument?.languageId) {
        case "arm-template":
        case "json":
        case "jsonc":
          cannotDecompile = !(await DecompileCommand.mightBeArmTemplateNoThrow(
            currentDocument.uri
          ));
          break;
        default:
          // Only disable if the current editor is JSON but not an ARM template.
          cannotDecompile = false;
          break;
      }

      await commands.executeCommand(
        "setContext",
        "bicep.cannotDecompile",
        cannotDecompile
      );

      if (pasteAsBicepCommand) {
        let canPasteAsBicep = false;

        if (pasteAsBicepCommand?.isExperimentalPasteAsBicepEnabled()) {
          if (currentDocument?.languageId === bicepLanguageId) {
            const clipboardText = await env.clipboard.readText();

            if (cachedCanPasteAsBicep.clipboardText === clipboardText) {
              canPasteAsBicep = cachedCanPasteAsBicep.canPasteAsBicep;
            } else {
              canPasteAsBicep = await pasteAsBicepCommand.canPasteAsBicep(
                context,
                clipboardText
              );
              cachedCanPasteAsBicep.clipboardText = clipboardText;
              cachedCanPasteAsBicep.canPasteAsBicep = canPasteAsBicep;
            }
          }
        }

        await commands.executeCommand(
          "setContext",
          "bicep.canPasteAsBicep",
          canPasteAsBicep
        );
      }
    }
  );
}
