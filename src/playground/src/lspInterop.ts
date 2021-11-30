// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { editor, languages } from 'monaco-editor/esm/vs/editor/editor.api';

let interop: any; /* eslint-disable-line */

export function initializeInterop(self: any): Promise<boolean> { /* eslint-disable */ /* eslint-disable-line */
  return new Promise<boolean>((resolve, reject) => {
    self['BicepInitialize'] = (newInterop: any) => {
      interop = newInterop;
      resolve(true);
    }

    // this is necessary to invoke the Blazor startup code - do not remove it!
    const test = require('../../Bicep.Wasm/bin/Release/net6.0/wwwroot/_framework/blazor.webassembly.js');
  });
}

export function getSemanticTokensLegend(): languages.SemanticTokensLegend {
  return interop.invokeMethod('GetSemanticTokensLegend');
}

export function getSemanticTokens(content: string): languages.SemanticTokens {
  return interop.invokeMethod('GetSemanticTokens', content);
}

export function compileAndEmitDiagnostics(content: string): {template: string, diagnostics: editor.IMarkerData[]} {
  return interop.invokeMethod('CompileAndEmitDiagnostics', content);
}

export function decompile(jsonContent: string): string {
  const { bicepFile, error } = interop.invokeMethod('Decompile', jsonContent);

  if (error) {
    throw error;
  }

  return bicepFile;
}
