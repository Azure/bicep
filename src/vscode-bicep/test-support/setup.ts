// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

jest.mock(
	"vscode",
	() => ({
		CancellationError: class extends Error {},
		ConfigurationTarget: { Global: 1 },
		ProgressLocation: { Notification: 15 },
		ThemeColor: class {},
		ThemeIcon: class {},
		window: {},
		l10n: { t: (message: string) => message },
	}),
	{ virtual: true },
);
