// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { callWithTelemetryAndErrorHandling, IActionContext, parseError } from "@microsoft/vscode-azext-utils";
import {
  ConfigurationTarget,
  env,
  MessageItem,
  Position,
  ProgressLocation,
  Range,
  TextDocumentChangeEvent,
  TextDocumentChangeReason,
  Uri,
  window,
  workspace,
  WorkspaceEdit,
} from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { BicepDecompileForPasteCommandParams, BicepDecompileForPasteCommandResult } from "../language";
import { bicepConfigurationKeys, bicepLanguageId, bicepParamLanguageId } from "../language/constants";
import { getBicepConfiguration } from "../language/getBicepConfiguration";
import { areEqualIgnoringWhitespace } from "../utils/areEqualIgnoringWhitespace";
import { Disposable } from "../utils/disposable";
import { getTextAfterFormattingChanges } from "../utils/getTextAfterFormattingChanges";
import { isEmptyOrWhitespace } from "../utils/isEmptyOrWhitespace";
import { getLogger } from "../utils/logger";
import { OutputChannelManager } from "../utils/OutputChannelManager";
import { callWithTelemetryAndErrorHandlingOnlyOnErrors } from "../utils/telemetry";
import { withProgressAfterDelay } from "../utils/withProgressAfterDelay";
import { findOrCreateActiveBicepFile } from "./findOrCreateActiveBicepFile";
import { SuppressedWarningsManager } from "./SuppressedWarningsManager";
import { Command } from "./types";

export class PasteAsBicepCommand implements Command {
  public readonly id = "bicep.pasteAsBicep";
  public disclaimerShownThisSession = false;

  public constructor(
    private readonly client: LanguageClient,
    private readonly outputChannelManager: OutputChannelManager,
    private readonly suppressedWarningsManager: SuppressedWarningsManager,
  ) {
    // Nothing to do
  }

  public async execute(context: IActionContext, documentUri?: Uri): Promise<void> {
    const logPrefix = "PasteAsBicep (command)";
    let clipboardText: string | undefined;
    let finalPastedBicep: string | undefined;

    try {
      documentUri = await findOrCreateActiveBicepFile(
        context,
        documentUri,
        "Choose which Bicep file to paste into",
        true,
      );

      const document = await workspace.openTextDocument(documentUri);
      const editor = await window.showTextDocument(document);

      if (editor?.document.languageId !== bicepLanguageId && editor?.document.languageId !== bicepParamLanguageId) {
        throw new Error("Cannot paste as Bicep: Editor is not editing a Bicep or BicepParam document.");
      }

      clipboardText = await env.clipboard.readText();

      let rangeStart =
        documentUri.toString() === editor.document.uri.toString()
          ? editor.document.offsetAt(editor.selection.active)
          : editor.document.offsetAt(new Position(Number.MAX_SAFE_INTEGER, Number.MAX_SAFE_INTEGER));
      let rangeEnd =
        documentUri.toString() === editor.document.uri.toString()
          ? editor.document.offsetAt(editor.selection.anchor)
          : editor.document.offsetAt(new Position(Number.MAX_SAFE_INTEGER, Number.MAX_SAFE_INTEGER));
      if (rangeEnd < rangeStart) {
        [rangeStart, rangeEnd] = [rangeEnd, rangeStart];
      }

      const result = await this.callDecompileForPaste(
        context,
        documentUri,
        document.getText(),
        rangeStart,
        rangeEnd - rangeStart,
        clipboardText,
        false /* queryCanPaste */,
        editor.document.languageId,
      );

      context.telemetry.properties.pasteType = result.pasteType;

      if (result.pasteContext === "string") {
        throw new Error(
          `Cannot paste JSON as Bicep inside of a string. First paste it outside of a string and then copy/paste into the string.`,
        );
      }

      if (!result.pasteType) {
        throw new Error(
          `The clipboard text does not appear to be valid JSON or is not in a format that can be pasted as Bicep.`,
        );
      }

      if (result.errorMessage) {
        context.errorHandling.issueProperties.clipboardText = clipboardText;
        throw new Error(`Could not paste clipboard text as Bicep: ${result.errorMessage}`);
      }

      // Note that unlike the copy/paste case, we *do* want to paste the Bicep even if the original
      //   clipboard text was already valid Bicep (pasteType == "bicep"), because the user is explicitly asking
      //   to paste as Bicep

      finalPastedBicep = result.bicep;
      await editor.edit((builder) => {
        builder.replace(editor.selection, finalPastedBicep ?? "");
      });
    } catch (err) {
      getLogger().debug(`${logPrefix}: Exception occurred: ${parseError(err).message}"`);
      throw err;
    } finally {
      this.logPasteCompletion(logPrefix, clipboardText, finalPastedBicep);
    }
  }

  public registerForPasteEvents(extension: Disposable) {
    extension.register(workspace.onDidChangeTextDocument(this.onDidChangeTextDocument.bind(this)));
  }

  private async callDecompileForPaste(
    context: IActionContext,
    uri: Uri,
    bicepContent: string,
    rangeOffset: number,
    rangeLength: number,
    jsonContent: string,
    queryCanPaste: boolean,
    languageId: string,
  ): Promise<BicepDecompileForPasteCommandResult> {
    return await withProgressAfterDelay(
      {
        location: ProgressLocation.Notification,
        title: "Decompiling clipboard text into Bicep is taking longer than expected...",
      },
      async () => {
        const decompileParams: BicepDecompileForPasteCommandParams = {
          uri: uri.fsPath,
          bicepContent,
          rangeOffset,
          rangeLength,
          jsonContent,
          queryCanPaste,
          languageId,
        };
        const decompileResult: BicepDecompileForPasteCommandResult = await this.client.sendRequest(
          "workspace/executeCommand",
          {
            command: "decompileForPaste",
            arguments: [decompileParams],
          },
        );

        context.telemetry.properties.pasteType = decompileResult.pasteType;
        context.telemetry.properties.pasteContext = decompileResult.pasteContext;
        context.telemetry.properties.decompileId = decompileResult.decompileId;
        context.telemetry.properties.jsonSize = String(jsonContent.length);
        context.telemetry.properties.queryCanPaste = String(queryCanPaste);
        context.telemetry.properties.languageId = languageId;

        return decompileResult;
      },
    );
  }

  private isAutoConvertOnPasteEnabled(): boolean {
    return getBicepConfiguration().get<boolean>(bicepConfigurationKeys.decompileOnPaste) ?? true;
  }

  // Handle automatically converting to Bicep on paste
  // TODO: refactor
  private async onDidChangeTextDocument(e: TextDocumentChangeEvent): Promise<void> {
    const logPrefix = "PasteAsBicep (copy/paste)";

    await callWithTelemetryAndErrorHandlingOnlyOnErrors("copyPasteInBicepFile", async () => {
      if (!this.isAutoConvertOnPasteEnabled()) {
        return;
      }

      const editor = window.activeTextEditor;
      if (
        e.reason !== TextDocumentChangeReason.Redo &&
        e.reason !== TextDocumentChangeReason.Undo &&
        e.document === editor?.document &&
        (e.document.languageId === bicepLanguageId || e.document.languageId === bicepParamLanguageId) &&
        e.contentChanges.length === 1
      ) {
        const contentChange = e.contentChanges[0];

        // Ignore deletions and trivial changes
        if (contentChange.text.length < 2 || isEmptyOrWhitespace(contentChange.text)) {
          return;
        }

        const clipboardText = await env.clipboard.readText();

        // This edit was a paste if the clipboard text matches the inserted text (ignoring formatting)
        if (
          clipboardText.length > 1 &&
          !isEmptyOrWhitespace(clipboardText) && // ... non-trivial changes only
          areEqualIgnoringWhitespace(clipboardText, contentChange.text)
        ) {
          let finalPastedBicep: string | undefined;

          await callWithTelemetryAndErrorHandling(
            "checkAutoPasteAsBicep",
            async (contextCheckAutoPaste: IActionContext) => {
              try {
                contextCheckAutoPaste.telemetry.properties.autoPasteAsBicep = "false";

                // While we were awaiting async calls, the pasted text may have been formatted in the editor, get the new version
                let formattedPastedText = getTextAfterFormattingChanges(
                  contentChange.text,
                  e.document.getText(),
                  contentChange.rangeOffset,
                );
                if (!formattedPastedText) {
                  getLogger().debug(`${logPrefix}: Couldn't get pasted text after editor format`);
                  return;
                }

                // See if we can paste this text as Bicep
                //   (this call will log telemetry about the result)
                const canPasteResult = await this.callDecompileForPaste(
                  contextCheckAutoPaste,
                  editor.document.uri,
                  editor.document.getText(),
                  contentChange.rangeOffset,
                  formattedPastedText.length,
                  clipboardText,
                  true, // queryCanPaste
                  e.document.languageId,
                );
                if (!canPasteResult.pasteType) {
                  // Nothing we know how to convert, or pasting is not allowed in this context
                  getLogger().debug(`${logPrefix}: pasteType empty`);
                  return;
                }

                if (canPasteResult.pasteType === "bicepValue") {
                  // If the input was already a valid Bicep expression (i.e., the conversion looks the same as the original, once formatting
                  //   changes are ignored), then skip the conversion, otherwise the user will see formatting changes when copying Bicep values
                  //   to Bicep (e.g. [1] would get changed to a multi-line array).
                  // This will mainly happen with single-line arrays and objects, especially since the Newtonsoft parser accepts input that is
                  //   JavaScript but not technically JSON, such as '{ abc: 1, def: 'def' }, but which also happens to be valid Bicep.
                  getLogger().debug(`${logPrefix}: Already bicep`);
                  return;
                }

                // The clipboard contains JSON which we can convert into Bicep
                // Start a new telemetry event to make tracking paste as Bicep more straightforward
                contextCheckAutoPaste.telemetry.properties.autoPasteAsBicep = "true";
                await callWithTelemetryAndErrorHandling("autoPasteAsBicep", async (contextAutoPaste) => {
                  if (canPasteResult.errorMessage || !canPasteResult.bicep) {
                    // If we should be able to convert but there were errors in the JSON, show a message to the output window
                    this.outputChannelManager.appendToOutputChannel(canPasteResult.output);
                    const msg = `Could not convert pasted text into Bicep: ${canPasteResult.errorMessage}`;
                    this.outputChannelManager.appendToOutputChannel(msg);
                    getLogger().debug(`${logPrefix}: ${msg}`);

                    // ... and register telemetry for the failure (don't show the error to the user again)
                    contextAutoPaste.telemetry.maskEntireErrorMessage = true;
                    contextAutoPaste.errorHandling.suppressDisplay = true;
                    throw new Error("Decompile error");
                  }

                  contextCheckAutoPaste.telemetry.properties.bicepSize = String(canPasteResult.bicep.length);

                  formattedPastedText = getTextAfterFormattingChanges(
                    contentChange.text,
                    e.document.getText(),
                    contentChange.rangeOffset,
                  );
                  if (!formattedPastedText) {
                    getLogger().debug(`${logPrefix}: Couldn't get pasted text after editor formatted it`);
                    return;
                  }
                  if (!areEqualIgnoringWhitespace(formattedPastedText, clipboardText)) {
                    // Some other editor change must have happened, abort the conversion to Bicep
                    contextAutoPaste.errorHandling.suppressDisplay = true;
                    throw new Error("Editor changed");
                  }

                  // All systems go - replace pasted JSON with Bicep
                  const edit = new WorkspaceEdit();
                  const rangeOfFormattedPastedText = new Range(
                    e.document.positionAt(contentChange.rangeOffset),
                    e.document.positionAt(contentChange.rangeOffset + formattedPastedText.length),
                  );
                  edit.replace(e.document.uri, rangeOfFormattedPastedText, canPasteResult.bicep);
                  const success = await workspace.applyEdit(edit);
                  if (!success) {
                    throw new Error("Applying edit failed while converting pasted JSON to Bicep");
                  }

                  // Don't wait for disclaimer/warning to be dismissed because our telemetry won't fire until we return
                  void this.showWarning(contextAutoPaste, canPasteResult);
                });

                finalPastedBicep = canPasteResult.bicep;
              } catch (err) {
                getLogger().debug(`${logPrefix}: Exception occurred: ${parseError(err).message}"`);
                throw err;
              } finally {
                this.logPasteCompletion(logPrefix, clipboardText, finalPastedBicep);
              }
            },
          );
        }
      }
    });
  }

  private async showWarning(context: IActionContext, pasteResult: BicepDecompileForPasteCommandResult): Promise<void> {
    // Always show this message
    this.outputChannelManager.appendToOutputChannel(
      "The JSON pasted into the editor was automatically decompiled to Bicep. Use undo to revert.",
      true /*noFocus*/,
    );

    if (!pasteResult.disclaimer) {
      return;
    }

    // Show disclaimer only once per session
    if (this.disclaimerShownThisSession) {
      return;
    }

    // Always show disclaimer in output window
    this.outputChannelManager.appendToOutputChannel(pasteResult.disclaimer, true /*noFocus*/);

    // Show disclaimer in a dialog until disabled
    if (this.suppressedWarningsManager.isWarningSuppressed(SuppressedWarningsManager.keys.decompileOnPasteWarning)) {
      return;
    }

    const dontShowAgain: MessageItem = {
      title: "Never show again",
    };
    const disable: MessageItem = {
      title: "Disable automatic decompile on paste",
    };

    this.disclaimerShownThisSession = true;
    const result = await context.ui.showWarningMessage(pasteResult.disclaimer, dontShowAgain, disable);
    if (result === dontShowAgain) {
      await this.suppressedWarningsManager.suppressWarning(SuppressedWarningsManager.keys.decompileOnPasteWarning);
    } else if (result === disable) {
      await getBicepConfiguration().update(bicepConfigurationKeys.decompileOnPaste, false, ConfigurationTarget.Global);

      // Don't wait for this to finish
      void window.showWarningMessage(
        `Automatic decompile on paste has been disabled. You can turn it back on at any time from VS Code settings (${SuppressedWarningsManager.keys.decompileOnPasteWarning}). You can also still use the "Paste as Bicep" command from the command palette.`,
      );
    }
  }

  // This allows our test code to know when a paste operation has been completed (in the case of handling CTRL+C/V)
  private logPasteCompletion(prefix: string, clipboardText: string | undefined, bicepText: string | undefined): void {
    function format(s: string | undefined): string {
      return typeof s === "string" ? `"${s}"` : "undefined";
    }

    getLogger().debug(`${prefix}: Result: ${format(clipboardText)} -> ${format(bicepText)}`);
  }
}
