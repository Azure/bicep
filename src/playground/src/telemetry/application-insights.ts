// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import {
  ApplicationInsights,
  IApplicationInsights,
  ITelemetryItem,
} from "@microsoft/applicationinsights-web";

const urlFieldNames = new Set(["refUri", "uri", "url"]);

export function getSanitizedCurrentUrl(): string {
  return sanitizeUrl(window.location.href);
}

export function configureTelemetry(
  insights: ApplicationInsights,
): IApplicationInsights {
  insights.loadAppInsights();
  insights.addTelemetryInitializer(sanitizeTelemetryItem);

  return insights;
}

export function initializeTelemetry(
  instrumentationKey: string,
): IApplicationInsights {
  const insights = configureTelemetry(
    new ApplicationInsights({
      config: {
        instrumentationKey,
      },
    }),
  );
  insights.trackPageView({ uri: getSanitizedCurrentUrl() });

  return insights;
}

export function sanitizeTelemetryItem(item: ITelemetryItem): void {
  sanitizeUrlFields(item.baseData);
  sanitizeUrlFields(item.data);
}

function sanitizeUrlFields(data: Record<string, unknown> | undefined): void {
  if (!data) {
    return;
  }

  for (const [key, value] of Object.entries(data)) {
    if (urlFieldNames.has(key) && typeof value === "string") {
      data[key] = sanitizeUrl(value);
    }
  }
}

function sanitizeUrl(value: string): string {
  try {
    const url = new URL(value, window.location.origin);
    url.hash = "";
    url.search = "";

    return url.toString();
  } catch {
    return value.split(/[?#]/, 1)[0];
  }
}
