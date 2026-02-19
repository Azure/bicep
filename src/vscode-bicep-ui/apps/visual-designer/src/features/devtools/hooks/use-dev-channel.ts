// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useEffect, useRef, useState } from "react";
import { FakeMessageChannel } from "../fake-message-channel";

/**
 * Lazily create a {@link FakeMessageChannel} for use in the
 * dev-playground (standalone `npm run dev` outside VS Code).
 *
 * Returns `undefined` until the channel is ready, so callers
 * can gate rendering on it.
 */
export function useDevChannel(): FakeMessageChannel | undefined {
  const [channel, setChannel] = useState<FakeMessageChannel | undefined>(undefined);
  const loadedRef = useRef(false);

  useEffect(() => {
    if (loadedRef.current) return;
    loadedRef.current = true; // prevent double-creation in StrictMode
    setChannel(new FakeMessageChannel());
  }, []);

  return channel;
}
