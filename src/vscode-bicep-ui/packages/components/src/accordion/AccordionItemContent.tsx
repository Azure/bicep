// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PropsWithChildren } from "react";

import { AnimatePresence, motion } from "motion/react";
import { useAccordionItem } from "./useAccordionItem";

export function AccordionItemContent({ children }: PropsWithChildren) {
  const { active } = useAccordionItem();

  return (
    <AnimatePresence initial={false}>
      {active && (
        <motion.section
          key="content"
          initial="collapsed"
          animate="open"
          exit="collapsed"
          variants={{
            open: { height: "auto" },
            collapsed: { height: 0 },
          }}
          transition={{ type: "spring", duration: 0.4, bounce: 0 }}
        >
          {children}
        </motion.section>
      )}
    </AnimatePresence>
  );
}
