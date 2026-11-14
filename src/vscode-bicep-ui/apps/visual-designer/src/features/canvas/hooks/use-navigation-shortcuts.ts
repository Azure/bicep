// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useSetAtom } from "jotai";
import { useEffect } from "react";
import { isInTextInput } from "@/utils";
import { setNavigationModeAtom, setSpaceHeldAtom } from "../atoms";

/** Controls where Space activates or toggles the focused element, so it must not switch tools. */
const SPACE_ACTIVATED_SELECTOR =
  "button, a[href], select, summary, [role='button'], [role='tab'], [role='menuitem'], [role='option'], [role='combobox'], [role='checkbox'], [role='switch']";

function hasModifier(event: KeyboardEvent) {
  return event.ctrlKey || event.metaKey || event.altKey || event.shiftKey;
}

/**
 * V selects the select tool and H the hand tool. Holding Space switches to the hand tool until it is released, so a
 * trackpad can pan without changing tools. None of them fire while typing or with a modifier held, and Space keeps its
 * usual meaning on buttons and other controls.
 */
export function useNavigationShortcuts() {
  const setMode = useSetAtom(setNavigationModeAtom);
  const setSpaceHeld = useSetAtom(setSpaceHeldAtom);

  useEffect(() => {
    const handleKeyDown = (event: KeyboardEvent) => {
      if (event.defaultPrevented || event.isComposing || hasModifier(event) || isInTextInput(event.target)) {
        return;
      }

      if (event.key === " ") {
        if (event.target instanceof Element && event.target.closest(SPACE_ACTIVATED_SELECTOR)) {
          return;
        }

        // Also keeps a held Space from scrolling anything.
        event.preventDefault();
        if (!event.repeat) setSpaceHeld(true);
        return;
      }

      const key = event.key.toLowerCase();
      if (key === "v" || key === "h") {
        event.preventDefault();
        event.stopPropagation();
        setMode(key === "v" ? "select" : "hand");
      }
    };

    const handleKeyUp = (event: KeyboardEvent) => {
      if (event.key === " ") setSpaceHeld(false);
    };
    // A release while the designer is not focused never arrives.
    const release = () => setSpaceHeld(false);

    document.addEventListener("keydown", handleKeyDown);
    document.addEventListener("keyup", handleKeyUp);
    window.addEventListener("blur", release);
    return () => {
      document.removeEventListener("keydown", handleKeyDown);
      document.removeEventListener("keyup", handleKeyUp);
      window.removeEventListener("blur", release);
      release();
    };
  }, [setMode, setSpaceHeld]);
}
