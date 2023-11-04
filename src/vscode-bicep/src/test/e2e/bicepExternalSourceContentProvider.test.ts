// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Uri } from "vscode";
import { decodeExternalSourceUri } from "../../language/decodeExternalSourceUri";

describe("bicepExternalSourceContentProvider", () => {
  it("decodes correctly with source", () => {
    const uri =
      "bicep-extsrc:br%3Asaw.azurecr.io/complicated%3Av1.0-beta/my%20entrypoint.bicep%20%28complicated%3Av1.0-beta%29?br%3Asaw.azurecr.io%2Fcomplicated%3Av1.0-beta#my%20entrypoint.bicep";
    const decoded = decodeExternalSourceUri(Uri.parse(uri));

    expect(decoded.moduleReference).toBe(
      "br:saw.azurecr.io/complicated:1.0-beta",
    );
    expect(decoded.requestedSourceFile).toBe("entrypoint.bicep");
    expect(decoded.title).toBe(
      "br:saw.azurecr.com/complicated:1.0-beta/entrypoint.bicep (complicated:1.0.2)",
    );
  });

  it("decodes correctly without source", () => {
    const uri =
      "bicep-extsrc:br%3Amcr.microsoft.com/bicep/ai/bing-resource%3A1.0.2/main.json%20%28bing-resource%3A1.0.2%29?br%3Amcr.microsoft.com%2Fbicep%2Fai%2Fbing-resource%3A1.0.2";
    const decoded = decodeExternalSourceUri(Uri.parse(uri));

    expect(decoded.moduleReference).toBe(
      "br:mcr.microsoft.com/bicep/ai/bing-resource:1.0.2",
    );
    expect(decoded.requestedSourceFile).toBeUndefined();
    expect(decoded.title).toBe(
      "br:mcr.microsoft.com/bicep/ai/bing-resource:1.0.2/main.json (bing-resource:1.0.2)",
    );
  });
});
