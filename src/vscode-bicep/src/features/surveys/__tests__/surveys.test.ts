// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Memento, MessageItem, WorkspaceConfiguration } from "vscode";

import { daysToMs, monthsToDays, weeksToDays } from "../../../infrastructure/timing";
import { IPersistedSurveyState, ISurveyInfo, Survey } from "../surveys";

type SurveyResponse = MessageItem & { id: string };

describe("Survey", () => {
  function createEnabledConfiguration(): Pick<WorkspaceConfiguration, "get"> {
    function get<T>(_section: string): T | undefined;
    function get<T>(_section: string, defaultValue: T): T;
    function get<T>(_section: string, defaultValue?: T): T | undefined {
      return defaultValue;
    }

    return { get };
  }

  function createMemento(): Memento {
    const values = new Map<string, unknown>();

    function get<T>(key: string): T | undefined;
    function get<T>(key: string, defaultValue: T): T;
    function get<T>(key: string, defaultValue?: T): T | undefined {
      return values.has(key) ? (values.get(key) as T) : defaultValue;
    }

    return {
      keys: () => [...values.keys()],
      get,
      update: async (key: string, value: unknown): Promise<void> => {
        if (value === undefined) {
          values.delete(key);
        } else {
          values.set(key, value);
        }
      },
    };
  }

  function createMocks(options: { surveyInfo?: ISurveyInfo; isSurveyAvailable: boolean }) {
    const globalStorage = createMemento();
    const surveyInfo =
      options.surveyInfo ??
      <ISurveyInfo>{
        akaLinkToSurvey: "link",
        postponeAfterTakenInDays: monthsToDays(1),
        postponeForLaterInDays: weeksToDays(1),
        surveyPrompt: "prompt",
        surveyStateKey: "testSurvey",
      };
    const configuration = createEnabledConfiguration();
    let nextResponse: SurveyResponse | undefined;
    let promptCount = 0;

    const survey = new Survey(globalStorage, surveyInfo, {
      showInformationMessage: async () => {
        promptCount++;
        const response = nextResponse;
        nextResponse = undefined;
        return response;
      },
      getIsSurveyAvailable: async () => options.isSurveyAvailable,
      launchSurvey: async () => undefined,
      provideBicepConfiguration: () => configuration,
    });

    return {
      globalStorage,
      get promptCount() {
        return promptCount;
      },
      resetPrompts: () => {
        nextResponse = undefined;
        promptCount = 0;
      },
      respondWith: (response: SurveyResponse | undefined) => {
        nextResponse = response;
      },
      survey,
    };
  }

  test("postpones for 180 days when the user chooses don't ask again", async () => {
    const mocks = createMocks({ isSurveyAvailable: true });
    const start = new Date();
    let now = start;

    // Show and respond with "Don't ask again"
    mocks.respondWith({
      title: "Don't ask again",
      id: "dontAskAgain",
    });
    await mocks.survey.checkShowSurvey(now);

    expect(mocks.promptCount).toBe(1);
    expect(mocks.globalStorage.get<IPersistedSurveyState>("testSurvey")?.postponedUntilMs).toBe(
      now.valueOf() + daysToMs(180),
    );

    // Try again at 179 days, should not show
    now = new Date(start.valueOf() + daysToMs(179));
    mocks.resetPrompts();
    await mocks.survey.checkShowSurvey(now);
    expect(mocks.promptCount).toBe(0);

    // Try again at 181 days, should show
    now = new Date(start.valueOf() + daysToMs(181));
    mocks.resetPrompts();
    await mocks.survey.checkShowSurvey(now);
    expect(mocks.promptCount).toBe(1);
  });

  test("doesn't prompt when the survey is unavailable", async () => {
    const mocks = createMocks({
      isSurveyAvailable: false,
    });

    // Try to show, should not ask
    await mocks.survey.checkShowSurvey(new Date());

    expect(mocks.promptCount).toBe(0);
  });

  test("prompts when an available survey hasn't been taken", async () => {
    const mocks = createMocks({
      isSurveyAvailable: true,
    });

    // Should show
    await mocks.survey.checkShowSurvey(new Date());

    expect(mocks.promptCount).toBe(1);
  });

  test("postpones after taking the survey", async () => {
    const postponeAfterYes = 100;
    const mocks = createMocks({
      isSurveyAvailable: true,
      surveyInfo: {
        akaLinkToSurvey: "link",
        postponeAfterTakenInDays: postponeAfterYes,
        postponeForLaterInDays: 1,
        surveyPrompt: "prompt",
        surveyStateKey: "testSurvey",
      },
    });
    const start = new Date();
    let now = start;

    // Show and respond with yes
    mocks.respondWith({
      title: "Jawohl",
      id: "yes",
    });
    await mocks.survey.checkShowSurvey(now);

    expect(mocks.promptCount).toBe(1);
    expect(mocks.globalStorage.get<IPersistedSurveyState>("testSurvey")?.postponedUntilMs).toBeUndefined();
    expect(mocks.globalStorage.get<IPersistedSurveyState>("testSurvey")?.lastTakenMs).toBe(now.valueOf());

    // Try again, right before the postponement date
    now = new Date(start.valueOf() + daysToMs(postponeAfterYes) - 1);
    mocks.resetPrompts();
    await mocks.survey.checkShowSurvey(now);

    // Shouldn't have asked
    expect(mocks.promptCount).toBe(0);

    // Try again, on the postponement date
    now = new Date(start.valueOf() + daysToMs(postponeAfterYes));
    mocks.resetPrompts();
    await mocks.survey.checkShowSurvey(now);

    expect(mocks.promptCount).toBe(1);

    // Try again, the day after the postponement date
    now = new Date(start.valueOf() + daysToMs(postponeAfterYes + 1));
    mocks.resetPrompts();
    await mocks.survey.checkShowSurvey(now);

    // Should have been shown
    expect(mocks.promptCount).toBe(1);
  });

  test("postpones when the user chooses later", async () => {
    const postponeLaterDays = 10;
    const mocks = createMocks({
      isSurveyAvailable: true,
      surveyInfo: {
        akaLinkToSurvey: "link",
        postponeAfterTakenInDays: postponeLaterDays * 2,
        postponeForLaterInDays: postponeLaterDays,
        surveyPrompt: "prompt",
        surveyStateKey: "testSurvey",
      },
    });
    const start = new Date();
    let now = start;

    // Ask and respond with "Later"
    mocks.respondWith({
      title: "Maybe later",
      id: "later",
    });
    await mocks.survey.checkShowSurvey(now);

    expect(mocks.promptCount).toBe(1);
    expect(mocks.globalStorage.get<IPersistedSurveyState>("testSurvey")?.postponedUntilMs).toBe(
      now.valueOf() + daysToMs(postponeLaterDays),
    );

    // Try again, a day before the postponement date
    now = new Date(start.valueOf() + daysToMs(postponeLaterDays - 1));
    mocks.resetPrompts();
    await mocks.survey.checkShowSurvey(now);

    // Shouldn't have asked
    expect(mocks.promptCount).toBe(0);

    // Try again, a day after the postponement date
    now = new Date(start.valueOf() + daysToMs(postponeLaterDays + 1));
    mocks.resetPrompts();
    await mocks.survey.checkShowSurvey(now);

    // Should have asked
    expect(mocks.promptCount).toBe(1);
  });

  test("postpones when the user dismisses the prompt", async () => {
    const postponeLaterDays = 7;
    const mocks = createMocks({
      isSurveyAvailable: true,
      surveyInfo: {
        akaLinkToSurvey: "link",
        postponeAfterTakenInDays: postponeLaterDays * 2,
        postponeForLaterInDays: postponeLaterDays,
        surveyPrompt: "prompt",
        surveyStateKey: "testSurvey",
      },
    });
    const start = new Date();
    let now = start;

    // Show and dismiss
    mocks.respondWith(undefined);
    await mocks.survey.checkShowSurvey(now);

    expect(mocks.promptCount).toBe(1);
    // Should postpone same as "later"
    expect(mocks.globalStorage.get<IPersistedSurveyState>("testSurvey")?.postponedUntilMs).toBe(
      now.valueOf() + daysToMs(postponeLaterDays),
    );

    // Try again, a day before the postponement date
    now = new Date(start.valueOf() + daysToMs(postponeLaterDays - 1));
    mocks.resetPrompts();
    await mocks.survey.checkShowSurvey(now);

    // Shouldn't have asked
    expect(mocks.promptCount).toBe(0);

    // Try again, a day after the postponement date
    now = new Date(start.valueOf() + daysToMs(postponeLaterDays + 1));
    mocks.resetPrompts();
    await mocks.survey.checkShowSurvey(now);

    // Should have asked
    expect(mocks.promptCount).toBe(1);
  });

  test("recovers from corrupt persisted state", async () => {
    const mocks = createMocks({
      isSurveyAvailable: true,
    });

    // Should show in spite of corrupt state
    await mocks.globalStorage.update("testSurvey", {
      lastTakenMs: "whoops",
      postponedUntilMs: -1,
    });
    await mocks.survey.checkShowSurvey(new Date());

    expect(mocks.promptCount).toBe(1);
  });

  test.each([
    [301, true],
    [302, false],
    [200, false],
    [undefined, false],
  ] as const)("maps survey link status %s to availability %s", async (statusCode, expected) => {
    const actual = await Survey.getIsSurveyAvailable("https://example.test", async () => statusCode);

    expect(actual).toBe(expected);
  });

  test("returns false when checking survey availability fails", async () => {
    const actual = await Survey.getIsSurveyAvailable("https://example.test", async () => {
      throw new Error("Host not found");
    });

    expect(actual).toBe(false);
  });
});
