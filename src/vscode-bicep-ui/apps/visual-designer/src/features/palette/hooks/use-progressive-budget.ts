// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { createContext, startTransition, useContext, useEffect, useState } from "react";

/** Headers and rows rendered at first: several viewports' worth, while keeping the first paint cheap. */
export const PROGRESSIVE_INITIAL_BUDGET = 50;

/** Headers and rows added each time the end of the rendered list comes within reach. */
export const PROGRESSIVE_BUDGET_STEP = 100;

/** The palette's scroll container, used as the root for detecting when the end of the list is near. */
export const PaletteScrollRootContext = createContext<HTMLElement | null>(null);

/**
 * A render budget that starts at {@link PROGRESSIVE_INITIAL_BUDGET} and grows by {@link PROGRESSIVE_BUDGET_STEP}
 * whenever the returned sentinel comes within reach of the scroll viewport. A new `resetKey` (for example a new
 * search query) starts over, so typing never re-renders an already grown list.
 */
export function useProgressiveBudget(resetKey: string) {
  const root = useContext(PaletteScrollRootContext);
  const [state, setState] = useState({ resetKey, budget: PROGRESSIVE_INITIAL_BUDGET });
  const [sentinel, setSentinel] = useState<HTMLElement | null>(null);
  const budget = state.resetKey === resetKey ? state.budget : PROGRESSIVE_INITIAL_BUDGET;

  useEffect(() => {
    if (!sentinel) {
      return;
    }

    // Re-observing after every growth reports the sentinel again if it is still in reach. Growing is a transition,
    // so rendering the next batch yields to scrolling and typing.
    const observer = new IntersectionObserver(
      (entries) => {
        if (entries.some((entry) => entry.isIntersecting)) {
          startTransition(() =>
            setState((current) => ({
              resetKey,
              budget:
                (current.resetKey === resetKey ? current.budget : PROGRESSIVE_INITIAL_BUDGET) + PROGRESSIVE_BUDGET_STEP,
            })),
          );
        }
      },
      { root, rootMargin: "0px 0px 400px 0px" },
    );
    observer.observe(sentinel);
    return () => observer.disconnect();
  }, [budget, resetKey, root, sentinel]);

  return { budget, sentinelRef: setSentinel };
}
