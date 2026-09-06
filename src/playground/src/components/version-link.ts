// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

export interface BicepVersionLink {
  ariaLabel?: string;
  href?: string;
  label: string;
}

export function getBicepVersionLink(version: string): BicepVersionLink {
  if (version.includes("placeholder")) {
    return { label: "Bicep development" };
  }

  const label = `Bicep ${version}`;
  if (/^[0-9]+\.[0-9]+\.[0-9]+$/.test(version)) {
    return {
      ariaLabel: `${label} release notes (opens in a new tab)`,
      href: `https://github.com/Azure/bicep/releases/tag/v${version}`,
      label,
    };
  }

  const commitHash = /-g([0-9a-f]{7,40})$/i.exec(version)?.[1];
  if (commitHash) {
    return {
      ariaLabel: `${label} source commit (opens in a new tab)`,
      href: `https://github.com/Azure/bicep/commit/${commitHash}`,
      label,
    };
  }

  return { label };
}
