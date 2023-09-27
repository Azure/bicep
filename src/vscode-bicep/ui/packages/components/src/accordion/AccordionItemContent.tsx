import { PropsWithChildren } from "react";
import { AnimatePresence, motion } from "framer-motion";
import { useAccordionItem } from "./use-accordion-item";

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
          <motion.div>{children}</motion.div>
        </motion.section>
      )}
    </AnimatePresence>
  );
}
