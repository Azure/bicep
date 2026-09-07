// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

export const EPSILON = 1.0e-8;

export function equalsZero(x: number, epsilon = EPSILON): boolean {
  return Math.abs(x) < epsilon;
}

export function equal(x: number, y: number, epsilon = EPSILON): boolean {
  return Math.abs(x - y) < epsilon;
}

export function greaterThan(x: number, y: number, epsilon = EPSILON): boolean {
  return x - y > epsilon;
}

export function greaterThanOrEqual(x: number, y: number, epsilon = EPSILON): boolean {
  return x - y >= -epsilon;
}

export function lessThan(x: number, y: number, epsilon = EPSILON): boolean {
  return x - y < -epsilon;
}

export function lessThanOrEqual(x: number, y: number, epsilon = EPSILON): boolean {
  return x - y < epsilon;
}
