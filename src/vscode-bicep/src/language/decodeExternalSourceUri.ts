// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as vscode from "vscode";

export const BicepExternalSourceScheme = "bicep-extsrc";

export type ExternalSource = {
  // The title to display for the document,
  //   e.g. "br:myregistry.azurecr.io/myrepo/module/main.json:v1/main.json (module:v1)" or similar
  // VSCode will display everything after the last slash in the document's tab, and the full string
  //   on hover.
  title: string;
  // Full module reference, e.g. "myregistry.azurecr.io/myrepo/module:v1"
  moduleReference: string;
  // File being requested from the source, relative to the module root.
  //   e.g. main.bicep or mypath/module.bicep
  // This should be undefined to request the compiled JSON file (can't use "main.json" because there
  //   might actually be a source file called "main.json" in the original module sources).
  requestedSourceFile?: string;
};

// NOTE: This should match the logic in BicepExternalSourceRequestHandler.GetExternalSourceLinkUri and
// also bicep\src\Bicep.LangServer.UnitTests\BicepExternalSourceRequestHandlerTests.cs.DecodeExternalSourceUri
export function decodeExternalSourceUri(uri: vscode.Uri): ExternalSource {
  // The uri passed in has this format (encoded):
  //   bicep-extsrc:{title}?{module-reference}[#{source-file-relative-path}]
  const title = decodeURIComponent(uri.path);
  const moduleReference = decodeURIComponent(uri.query);
  let requestedSourceFile: string | undefined = decodeURIComponent(uri.fragment);

  if (requestedSourceFile === "") {
    requestedSourceFile = undefined;
  }

  return { title, moduleReference, requestedSourceFile };
}
