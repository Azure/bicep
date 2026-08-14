// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { describe, expect, it } from "@jest/globals";
import { Uri } from "vscode";
import { decodeExternalSourceUri } from "./external-source-uri";

describe("bicepExternalSourceContentProvider", () => {
  it("decodes correctly with source", () => {
    const uri = {
      path: "br%3Asaw.azurecr.io/complicated%3Av1.0-beta/my%20entrypoint.bicep%20%28very%20complicated%20for%20just%20version%20v1.0-beta%29",
      query: "br%3Asaw.azurecr.io%2Fcomplicated%3Av1.0-beta",
      fragment: "my%20entrypoint.bicep",
    } as Uri;
    const decoded = decodeExternalSourceUri(uri);

    expect(decoded.moduleReference).toBe("br:saw.azurecr.io/complicated:v1.0-beta");
    expect(decoded.requestedSourceFile).toBe("my entrypoint.bicep");
    expect(decoded.title).toBe(
      "br:saw.azurecr.io/complicated:v1.0-beta/my entrypoint.bicep (very complicated for just version v1.0-beta)",
    );
  });

  it("decodes correctly without source", () => {
    const uri = {
      path: "br%3Amcr.microsoft.com/bicep/ai/bing-resource%3A1.0.2/main.json%20%28bing-resource%3A1.0.2%29",
      query: "br%3Amcr.microsoft.com%2Fbicep%2Fai%2Fbing-resource%3A1.0.2",
      fragment: "",
    } as Uri;
    const decoded = decodeExternalSourceUri(uri);

    expect(decoded.moduleReference).toBe("br:mcr.microsoft.com/bicep/ai/bing-resource:1.0.2");
    expect(decoded.requestedSourceFile).toBeUndefined();
    expect(decoded.title).toBe("br:mcr.microsoft.com/bicep/ai/bing-resource:1.0.2/main.json (bing-resource:1.0.2)");
  });
});
