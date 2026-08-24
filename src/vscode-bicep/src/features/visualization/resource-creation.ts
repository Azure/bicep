// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

export function hasDocumentChanged(
  requestedVersion: number,
  currentVersion: number,
  documentIsClosed: boolean,
): boolean {
  return documentIsClosed || currentVersion !== requestedVersion;
}

export function getApplyEditFailureCode(
  requestedVersion: number,
  currentVersion: number,
  documentIsClosed: boolean,
): "documentChanged" | "editRejected" {
  return hasDocumentChanged(requestedVersion, currentVersion, documentIsClosed) ? "documentChanged" : "editRejected";
}
