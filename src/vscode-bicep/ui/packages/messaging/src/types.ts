export interface WebviewRequestMessage {
  id: string;
  method: string;
  params?: unknown;
}

export interface WebviewResponseMessage {
  id: string;
  result?: unknown;
  error?: unknown;
}

export interface WebviewNotificationMessage {
  method: string;
  params?: unknown;
}
