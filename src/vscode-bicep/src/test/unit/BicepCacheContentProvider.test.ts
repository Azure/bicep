// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/* eslint-disable jest/require-hook */

import { Uri } from "vscode";
import { BicepCacheContentProvider } from "../../language";

describe("class BicepCacheContentProvider tests", () => {
  describe("getModuleReferenceScheme tests", () => {
    describe("should return the correct scheme", () => {
      [
        {
          input:
            "bicep-cache:br%3Amyregistry.azurecr.io/myrepo%3Atest#%2FUsers%2Fmyname%2F.bicep%2Fbr%2Fmyregistry.azurecr.io%2Fmyrepo%2Ftest%24%2Fmain.bicep",
          expected: "br",
        },
      ].forEach(({ input, expected }) => {
        it(`uri=${input}`, () => {
          const actual = BicepCacheContentProvider.getModuleReferenceScheme(
            Uri.parse(input),
          );

          expect(actual).toBe(expected);
        });
      });
    });
  });
});
