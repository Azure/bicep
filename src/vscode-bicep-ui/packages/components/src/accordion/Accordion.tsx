// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PropsWithChildren } from "react";
import type { AccordionItemId } from "./types";

import { useCallback, useMemo, useRef, useState } from "react";
import { AccordionProvider } from "./AccordionProvider";

export interface AccordionProps extends PropsWithChildren {
  value?: readonly AccordionItemId[];
  defaultValue?: readonly AccordionItemId[];
  multiple?: boolean;
  onValueChange?: (value: readonly AccordionItemId[]) => void;
}

export function Accordion({ children, value, defaultValue = [], multiple = false, onValueChange }: AccordionProps) {
  const [uncontrolledValue, setUncontrolledValue] = useState<readonly AccordionItemId[]>(defaultValue);
  const requestedValue = value ?? uncontrolledValue;
  const activeValue = multiple ? requestedValue : requestedValue.slice(0, 1);
  const activeItemIds = useMemo(() => new Set(activeValue), [activeValue]);
  const headersRef = useRef(new Map<AccordionItemId, HTMLButtonElement>());

  const updateValue = useCallback(
    (nextValue: readonly AccordionItemId[]) => {
      if (value === undefined) {
        setUncontrolledValue(nextValue);
      }
      onValueChange?.(nextValue);
    },
    [onValueChange, value],
  );

  const toggleItem = useCallback(
    (itemId: AccordionItemId) => {
      if (activeItemIds.has(itemId)) {
        updateValue(activeValue.filter((activeItemId) => activeItemId !== itemId));
      } else {
        updateValue(multiple ? [...activeValue, itemId] : [itemId]);
      }
    },
    [activeItemIds, activeValue, multiple, updateValue],
  );

  const registerHeader = useCallback((itemId: AccordionItemId, element: HTMLButtonElement | null) => {
    if (element) {
      headersRef.current.set(itemId, element);
    } else {
      headersRef.current.delete(itemId);
    }
  }, []);

  const focusHeader = useCallback((itemId: AccordionItemId, direction: "first" | "last" | "next" | "previous") => {
    const headerEntries = [...headersRef.current.entries()]
      .filter(([, element]) => element.isConnected)
      .sort(([, left], [, right]) => {
        if (left === right) {
          return 0;
        }
        return left.compareDocumentPosition(right) & Node.DOCUMENT_POSITION_FOLLOWING ? -1 : 1;
      });
    if (headerEntries.length === 0) {
      return;
    }

    const currentIndex = headerEntries.findIndex(([registeredId]) => registeredId === itemId);
    const targetIndex =
      direction === "first"
        ? 0
        : direction === "last"
          ? headerEntries.length - 1
          : direction === "next"
            ? (currentIndex + 1) % headerEntries.length
            : (currentIndex - 1 + headerEntries.length) % headerEntries.length;

    headerEntries[targetIndex]?.[1].focus();
  }, []);

  const context = useMemo(
    () => ({ activeItemIds, toggleItem, registerHeader, focusHeader }),
    [activeItemIds, focusHeader, registerHeader, toggleItem],
  );

  return <AccordionProvider value={context}>{children}</AccordionProvider>;
}
