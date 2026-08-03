// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

Object.defineProperty(globalThis, "CSS", {
  writable: true,
  value: {
    escape: (value: string) => value.replace(/[^a-zA-Z0-9_\-]/g, (character) => `\\${character}`),
    supports: () => false,
  },
});

Object.defineProperty(globalThis, "matchMedia", {
  writable: true,
  value: vi.fn().mockImplementation((query) => ({
    matches: false,
    media: query,
    onchange: null,
    addListener: vi.fn(),
    removeListener: vi.fn(),
    addEventListener: vi.fn(),
    removeEventListener: vi.fn(),
    dispatchEvent: vi.fn(),
  })),
});