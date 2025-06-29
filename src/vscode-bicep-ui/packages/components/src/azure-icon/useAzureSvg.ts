// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { FunctionComponent, SVGProps } from "react";

import { useEffect, useRef, useState } from "react";

type SvgComponent = FunctionComponent<SVGProps<SVGSVGElement>>;

const svgImportsByPath = import.meta.glob<SvgComponent>(`../../assets/azure-architecture-icons/**/*.svg`, {
  query: "react",
  import: "default",
});

const svgPathsByResourceType: Record<string, string> = {
  // Microsoft.Compute
  "Microsoft.Compute/virtualMachines": "compute/10021-icon-service-Virtual-Machine",
  "Microsoft.Compute/virtualMachineScaleSets": "compute/10034-icon-service-VM-Scale-Sets",

  // Microsoft.Network
  "Microsoft.network/networkInterfaces": "networking/10080-icon-service-Network-Interfaces",
  "Microsoft.network/loadBalancers": "networking/10062-icon-service-Load-Balancers",

  // Microsoft.Web
  "Microsoft.Web/serverfarms": "app-services/00046-icon-service-App-Service-Plans",
  "Microsoft.Web/sites": "app-services/10035-icon-service-App-Services",
};

async function importAzureSvg(resourceType: string): Promise<SvgComponent | undefined> {
  const svgPath = svgPathsByResourceType[resourceType] ?? "custom/resource";
  const svgImport = svgImportsByPath[`../../assets/azure-architecture-icons/${svgPath}.svg`];

  return svgImport();
}

export function useAzureSvg(resourceType: string) {
  const svgRef = useRef<SvgComponent>();
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    setLoading(true);

    const loadIcon = async () => {
      try {
        svgRef.current = await importAzureSvg(resourceType);
      } catch (err) {
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    loadIcon();
  }, [resourceType]);

  return { loading, AzureSvg: svgRef.current };
}
