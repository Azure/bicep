// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { MessageArgs, RequestDescriptor } from "./messageDescriptor";

import { useEffect, useState } from "react";
import { useWebviewMessageChannel } from "./useWebviewMessageChannel";

/**
 * Issues a declared request once on mount and returns `[result, error]`.
 *
 * The result type comes from the descriptor rather than a caller-supplied generic, so it cannot
 * disagree with the method being sent.
 */
export function useRequest<TParams, TResult>(
  descriptor: RequestDescriptor<TParams, TResult>,
  ...args: MessageArgs<TParams>
) {
  const messageChannel = useWebviewMessageChannel();
  const [result, setResult] = useState<TResult | undefined>(undefined);
  const [error, setError] = useState<unknown | undefined>(undefined);
  const params = args[0];

  useEffect(() => {
    const invokeRequest = async () => {
      try {
        setResult(await messageChannel.sendRequest<TResult>({ method: descriptor.method, params }));
      } catch (error) {
        setError(error);
      }
    };

    void invokeRequest();
  }, [descriptor.method, params, messageChannel]);

  return [result, error] as const;
}
