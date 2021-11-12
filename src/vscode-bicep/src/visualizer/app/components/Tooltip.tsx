// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { ReactNode, useRef, useState, VFC } from "react";
import styled from "styled-components";

interface TooltipHostProps {
  content: string;
  children: ReactNode;
}

const TooltipHostContainer = styled.div`
  display: relative;
  user-select: none;
`;

const TooltipBox = styled.div<{ active: boolean }>`
  position: absolute;
  color: white;
  background: black;
  border-radius: 4px;
  padding: 4px 8px;
  margin-top: 2px;
  right: 44px;
  width: max-content;
  display: ${({ active }) => (active ? "block" : "none")};
`;

export const TooltipHost: VFC<TooltipHostProps> = ({ content, children }) => {
  const [active, setActive] = useState(false);
  const timeoutRef = useRef<NodeJS.Timeout | null>(null);

  const showTooltip = () => {
    timeoutRef.current = setTimeout(() => setActive(true), 600);
  };

  const hideTooltip = () => {
    if (timeoutRef.current) {
      clearTimeout(timeoutRef.current);
      setActive(false);
    }
  };

  return (
    <TooltipHostContainer>
      <TooltipBox active={active}>{content}</TooltipBox>
      <div
        onMouseEnter={showTooltip}
        onMouseLeave={hideTooltip}
        onMouseDown={hideTooltip}
      >
        {children}
      </div>
    </TooltipHostContainer>
  );
};
