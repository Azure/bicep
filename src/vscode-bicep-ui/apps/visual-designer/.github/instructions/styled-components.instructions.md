---
applyTo: "src/**/*.tsx"
description: "Use when writing or modifying styled-components, CSS-in-JS, or component styling in TSX files."
---

# Styled-Components Conventions

## Naming

- Prefix styled components with `$`: `const $Wrapper = styled.div\`...\`;`.
- Name describes purpose, not HTML element: `$Toolbar`, `$Separator`, not `$StyledDiv`.

## Rules

- Use styled-components for **all** static styles. Never use inline `style={{}}` for values known at build time.
- For dynamic values computed at render time (positions, transforms, dimensions), use `.attrs()` to keep styles co-located and avoid CSS class regeneration:
  ```tsx
  const $Panel = styled.div.attrs<{ $x: number; $y: number }>(({ $x, $y }) => ({
    style: { transform: `translate(${$x}px, ${$y}px)` },
  }))`
    position: absolute;
  `;
  ```
- Do not use bare inline `style={{}}` in JSX. Prefer `.attrs()` to keep all styling within the styled component definition.
- Access theme values via the `${({ theme }) => ...}` interpolation, not `useTheme()` + inline style.
- Extend existing styled components with `styled(Base)` instead of duplicating CSS.
- Keep styled component definitions in the same file as the React component that uses them, above the component function.
- Do not use `className` props or external CSS files.
