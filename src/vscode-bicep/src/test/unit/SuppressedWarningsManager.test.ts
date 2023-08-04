// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/* eslint-disable @typescript-eslint/no-unused-vars */

import { ConfigurationTarget, Uri } from "vscode";
import { SuppressedWarningsManager } from "../../commands/SuppressedWarningsManager";
import { WorkspaceConfigurationFake } from "../fakes/workspaceConfigurationFake";
import { BicepCacheContentProvider } from "../../language/bicepCacheContentProvider";

// class BicepCacheContentProvider {
//   //extends Disposable
//   //implements vscode.TextDocumentContentProvider
//   //constructor(private readonly languageClient: LanguageClient) {}

//   onDidChange?: vscode.Event<vscode.Uri> | undefined;

//   async provideTextDocumentContent(
//     uri: vscode.Uri,
//     token: vscode.CancellationToken,
//   ): Promise<void> {
//     // Ask the language server for the sources for the cached module
//     // const response = await this.languageClient.sendRequest(
//     //   bicepCacheRequestType,
//     //   this.getBicepCacheRequest(uri),
//     //   token,
//     // );
//     // return response.content;
//   }

//   // private getBicepCacheRequest(uri: vscode.Uri) {
//   //   const [moduleReference, cachePath] = this.decodeBicepCacheUri(uri);
//   //   return {
//   //     textDocument: TextDocumentIdentifier.create(cachePath),
//   //     target: moduleReference,
//   //   };
//   // }

//   // private decodeBicepCacheUri(
//   //   uri: vscode.Uri,
//   // ): [moduleReference: string, cachePath: string] {
//   //   // The uri passed in has this format:
//   //   //   bicep-cache:module-reference#cache-file-path
//   //   //
//   //   // Example decoded URI:
//   //   //   bicep-cache:br:myregistry.azurecr.io/myrepo:v1#/Users/MyUserName/.bicep/br/registry.azurecr.io/myrepo/v1$/main.json
//   //   const registry = decodeURIComponent(uri.path); // e.g. br:myregistry.azurecr.io/myrepo:v1
//   //   const cachePath = decodeURIComponent(uri.fragment); // e.g. eg /Users/MyUserName/.bicep/br/myregistry.azurecr.io/myrepo/v1$/main.json

//   //   return [registry, cachePath];
//   // }

//   // Returns "br" or "ts"
//   public static getModuleReferenceScheme(uri: Uri) {
//     const moduleReferenceWithLeadingSeparator = uri.path;
//     const colonIndex = moduleReferenceWithLeadingSeparator.indexOf(":");
//     if (colonIndex < 0) {
//       throw new Error(
//         `The document URI '${uri.toString()}' has an unexpected format.`,
//       );
//     }

//     return moduleReferenceWithLeadingSeparator.substring(0, colonIndex);
//   }
// }

describe("suppressedWarningsManager", () => {
  it("should not suppress by default", () => {
    let a = BicepCacheContentProvider.getModuleReferenceScheme(
      Uri.parse("https://abc/def.txt"),
    );
    const b = a;
    a = b;

    const config = new WorkspaceConfigurationFake();
    const manager = new SuppressedWarningsManager(() => config);

    expect(manager.isWarningSuppressed("test")).toBeFalsy();
  });

  it("should suppress when requested", async () => {
    const config = new WorkspaceConfigurationFake();
    const manager = new SuppressedWarningsManager(() => config);

    await manager.suppressWarning("test");
    expect(manager.isWarningSuppressed("test")).toBeTruthy();
  });

  it("should reset when requested", async () => {
    const config = new WorkspaceConfigurationFake();
    const manager = new SuppressedWarningsManager(() => config);

    await manager.suppressWarning("test");
    expect(manager.isWarningSuppressed("test")).toBeTruthy();

    await manager.resetWarning("test");
    expect(manager.isWarningSuppressed("test")).toBeFalsy();
  });

  it("should handle bad configuration", async () => {
    const config = new WorkspaceConfigurationFake();
    const manager = new SuppressedWarningsManager(() => config);

    await config.update(
      SuppressedWarningsManager.suppressedWarningsConfigurationKey,
      123456,
      ConfigurationTarget.Global,
    );

    expect(manager.isWarningSuppressed("test")).toBeFalsy();

    await manager.suppressWarning("test");
    expect(manager.isWarningSuppressed("test")).toBeTruthy();
  });
});
