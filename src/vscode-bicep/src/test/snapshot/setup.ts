// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Mock HTMLCanvasElement.getContext as jsdom does not implement it,
// but cytoscape requires it during initialization.
const mockContext = {
  canvas: {} as HTMLCanvasElement,
  fillRect: jest.fn(),
  clearRect: jest.fn(),
  getImageData: jest.fn(() => ({ data: new Array(4) })),
  putImageData: jest.fn(),
  createImageData: jest.fn(() => []),
  setTransform: jest.fn(),
  drawImage: jest.fn(),
  save: jest.fn(),
  fillText: jest.fn(),
  restore: jest.fn(),
  beginPath: jest.fn(),
  moveTo: jest.fn(),
  lineTo: jest.fn(),
  closePath: jest.fn(),
  stroke: jest.fn(),
  translate: jest.fn(),
  scale: jest.fn(),
  rotate: jest.fn(),
  arc: jest.fn(),
  fill: jest.fn(),
  measureText: jest.fn(() => ({ width: 0 })),
  transform: jest.fn(),
  rect: jest.fn(),
  clip: jest.fn(),
  bezierCurveTo: jest.fn(),
  quadraticCurveTo: jest.fn(),
  createLinearGradient: jest.fn(() => ({ addColorStop: jest.fn() })),
  createRadialGradient: jest.fn(() => ({ addColorStop: jest.fn() })),
  createPattern: jest.fn(),
  ellipse: jest.fn(),
  roundRect: jest.fn(),
};

HTMLCanvasElement.prototype.getContext = jest.fn(
  () => mockContext,
) as unknown as typeof HTMLCanvasElement.prototype.getContext;
