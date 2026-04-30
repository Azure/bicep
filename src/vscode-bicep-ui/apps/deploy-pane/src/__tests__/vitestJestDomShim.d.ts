// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Shim to augment @vitest/expect with @testing-library/jest-dom matchers.
// Required because @testing-library/jest-dom augments 'vitest' but
// vitest 4.x moved Assertion to '@vitest/expect'.
// The import below loads the jest-dom augmentation for 'vitest' (side effect).
import "@testing-library/jest-dom/vitest";

// Re-apply the same augmentation to @vitest/expect since vitest 4.x
// defines Assertion in @vitest/expect rather than in 'vitest' directly.
declare module "@vitest/expect" {
  interface Assertion<T = any> {
    toBeInTheDocument(): void;
    toBeVisible(): void;
    toBeChecked(): void;
    toBeDisabled(): void;
    toBeEnabled(): void;
    toBeEmpty(): void;
    toBeEmptyDOMElement(): void;
    toBeInvalid(): void;
    toBeRequired(): void;
    toBeValid(): void;
    toContainElement(element: Element | null): void;
    toContainHTML(htmlText: string): void;
    toHaveAccessibleDescription(text?: string | RegExp): void;
    toHaveAccessibleName(text?: string | RegExp): void;
    toHaveAttribute(attr: string, value?: string | RegExp | null): void;
    toHaveClass(...classNames: string[]): void;
    toHaveFocus(): void;
    toHaveFormValues(values: Record<string, unknown>): void;
    toHaveStyle(css: string | Record<string, unknown>): void;
    toHaveTextContent(text: string | RegExp, options?: { normalizeWhitespace: boolean }): void;
    toHaveValue(value?: string | string[] | number | null): void;
    toHaveDisplayValue(value: string | RegExp | Array<string | RegExp>): void;
    toBePartiallyChecked(): void;
    toHaveErrorMessage(text?: string | RegExp): void;
  }
}
