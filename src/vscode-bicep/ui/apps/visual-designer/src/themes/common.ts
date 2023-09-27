export const fontFamily = getComputedStyle(document.body)
  .getPropertyValue("--vscode-font-family")
  .replace(/"/g, "");
