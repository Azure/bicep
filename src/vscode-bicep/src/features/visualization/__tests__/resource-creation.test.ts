// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { getApplyEditFailureCode, hasDocumentChanged } from "../resource-creation";

describe("hasDocumentChanged", () => {
  it("detects version drift and closed documents", () => {
    expect(hasDocumentChanged(4, 5, false)).toBe(true);
    expect(hasDocumentChanged(4, 4, true)).toBe(true);
    expect(hasDocumentChanged(4, 4, false)).toBe(false);
  });
});

describe("getApplyEditFailureCode", () => {
  it("reports a concurrent document version change", () => {
    expect(getApplyEditFailureCode(4, 5, false)).toBe("documentChanged");
  });

  it("reports a closed document as changed", () => {
    expect(getApplyEditFailureCode(4, 4, true)).toBe("documentChanged");
  });

  it("reports an unchanged edit rejection", () => {
    expect(getApplyEditFailureCode(4, 4, false)).toBe("editRejected");
  });
});
