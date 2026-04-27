// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Mock cytoscape to avoid canvas/DOM requirements in snapshot tests.
const cytoscapeMock = jest.fn(() => ({
  on: jest.fn(),
  addListener: jest.fn(),
  removeListener: jest.fn(),
  destroy: jest.fn(),
  style: jest.fn(),
  json: jest.fn(),
  layout: jest.fn(() => ({ run: jest.fn(), stop: jest.fn() })),
  zoom: jest.fn(),
  animate: jest.fn(),
  elements: jest.fn(() => []),
  maxZoom: jest.fn(),
  minZoom: jest.fn(),
}));

(cytoscapeMock as jest.Mock & { use: jest.Mock }).use = jest.fn();

module.exports = cytoscapeMock;
