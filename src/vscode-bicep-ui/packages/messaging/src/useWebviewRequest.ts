// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useEffect, useState } from "react";
import { useWebviewMessageChannel } from "./useWebviewMessageChannel";

export function useWebviewRequest<TResult>(method: string, params?: unknown) {
  const messageChannel = useWebviewMessageChannel();
  const [result, setResult] = useState<TResult | undefined>(undefined);
  const [error, setError] = useState<unknown | undefined>(undefined);

  useEffect(() => {
    const invokeRequest = async () => {
      try {
        const result = (await messageChannel.sendRequest({ method, params })) as TResult;
        setResult(result);
      } catch (error) {
        setError(error);
      }
    };

    invokeRequest();
  }, [method, params, messageChannel]);

  return [result, error] as const;
}
