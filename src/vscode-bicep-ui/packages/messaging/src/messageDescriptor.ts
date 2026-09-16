// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/**
 * Typed descriptions of the messages exchanged with the extension host.
 *
 * A descriptor binds a method name to its parameter and result types in one declaration, so a call
 * site cannot pair the wrong types with a method. Without it, `sendRequest<T>({ method })` takes the
 * method and the result type as independent, unchecked arguments, and nothing stops them from
 * disagreeing.
 *
 * Descriptors carry no runtime behaviour beyond the method name. The phantom members exist only so
 * the compiler can recover the types; they are never present at runtime, and they also stop two
 * descriptors with different types from being structurally interchangeable.
 */

declare const paramsBrand: unique symbol;
declare const resultBrand: unique symbol;

export interface RequestDescriptor<TParams, TResult> {
  readonly method: string;
  readonly [paramsBrand]?: (params: TParams) => void;
  readonly [resultBrand]?: (result: TResult) => void;
}

export interface NotificationDescriptor<TParams> {
  readonly method: string;
  readonly [paramsBrand]?: (params: TParams) => void;
}

/** Arguments a message takes: none when it was declared with `void` parameters. */
export type MessageArgs<TParams> = [TParams] extends [void] ? [] : [params: TParams];

/** Declares a request: `method`, sent with `TParams`, resolving to `TResult`. */
export function defineRequest<TParams = void, TResult = void>(method: string): RequestDescriptor<TParams, TResult> {
  return { method };
}

/** Declares a notification: `method`, sent with `TParams`. */
export function defineNotification<TParams = void>(method: string): NotificationDescriptor<TParams> {
  return { method };
}
