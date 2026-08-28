// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

export function getErrorMessage(error: unknown, fallback: string): string {
  return typeof error === "object" && error !== null && "message" in error ? String(error.message) : fallback;
}
