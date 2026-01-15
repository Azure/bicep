// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { IActionContext } from "@microsoft/vscode-azext-utils";
import { MessageItem, window } from "vscode";
import { IPersistedSurveyState, ISurveyInfo, Survey } from "../../feedback/surveys";
import { daysToMs, monthsToDays, weeksToDays } from "../../utils/time";
import { GlobalStateFake } from "../fakes/globalStateFake";
import { WorkspaceConfigurationFake } from "../fakes/workspaceConfigurationFake";
import { createActionContextMock } from "../mocks/actionContextMock";

describe("surveys-unittests", () => {
  function createMocks(options: {
    surveyInfo?: ISurveyInfo;
    isSurveyAvailable: boolean;
    showInformationMessageMock?: jest.Mock<typeof window.showInformationMessage>;
    getIsSurveyAvailableMock?: jest.Mock<() => Promise<boolean>>;
    launchSurveyMock?: jest.Mock<(context: IActionContext, surveyInfo: ISurveyInfo) => Promise<void>>;
  }) {
    const globalStorageFake = new GlobalStateFake();
    const surveyInfo =
      options.surveyInfo ??
      <ISurveyInfo>{
        akaLinkToSurvey: "link",
        postponeAfterTakenInDays: monthsToDays(1),
        postponeForLaterInDays: weeksToDays(1),
        surveyPrompt: "prompt",
        surveyStateKey: "testSurvey",
      };
    const showInformationMessageMock = options.showInformationMessageMock ?? jest.fn();
    const getIsSurveyAvailableMock = options.getIsSurveyAvailableMock ?? jest.fn();
    const launchSurveyMock = options.launchSurveyMock ?? jest.fn();
    const workspaceConfigurationFake = new WorkspaceConfigurationFake();

    const survey = new Survey(globalStorageFake, surveyInfo, {
      showInformationMessage: showInformationMessageMock,
      getIsSurveyAvailable: jest.fn().mockImplementation(async () => options.isSurveyAvailable),
      launchSurvey: launchSurveyMock,
      provideBicepConfiguration: () => workspaceConfigurationFake,
    });

    return {
      globalStorageFake,
      survey,
      showInformationMessageMock,
      getIsSurveyAvailableMock,
      workspaceConfigurationFake,
    };
  }

  it("postpones for 180 days if user responds don't ask again", async () => {
    const mocks = createMocks({ isSurveyAvailable: true });
    const start = new Date();
    let now = start;

    // Show and respond with "Don't ask again"
    mocks.showInformationMessageMock.mockResolvedValueOnce(<MessageItem>{
      title: "Don't ask again",
      id: "dontAskAgain",
    });
    await mocks.survey.checkShowSurvey(createActionContextMock(), now);

    expect(mocks.showInformationMessageMock).toHaveBeenCalledTimes(1);
    expect(mocks.globalStorageFake.get<IPersistedSurveyState>("testSurvey")?.postponedUntilMs).toBe(
      now.valueOf() + daysToMs(180),
    );

    // Try again at 179 days, should not show
    now = new Date(start.valueOf() + daysToMs(179));
    mocks.showInformationMessageMock.mockClear();
    await mocks.survey.checkShowSurvey(createActionContextMock(), now);
    expect(mocks.showInformationMessageMock).not.toHaveBeenCalled();

    // Try again at 181 days, should show
    now = new Date(start.valueOf() + daysToMs(181));
    mocks.showInformationMessageMock.mockClear();
    await mocks.survey.checkShowSurvey(createActionContextMock(), now);
    expect(mocks.showInformationMessageMock).toHaveBeenCalledTimes(1);
  });

  it("doesn't prompt if survey is not available", async () => {
    const mocks = createMocks({
      isSurveyAvailable: false,
    });

    // Try to show, should not ask
    await mocks.survey.checkShowSurvey(createActionContextMock(), new Date());

    expect(mocks.showInformationMessageMock).not.toHaveBeenCalled();
  });

  it("prompts for survey if available and never taken before", async () => {
    const mocks = createMocks({
      isSurveyAvailable: true,
    });

    // Should show
    await mocks.survey.checkShowSurvey(createActionContextMock(), new Date());

    expect(mocks.showInformationMessageMock).toHaveBeenCalledTimes(1);
  });

  it("postpones after taking survey", async () => {
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
    mocks.showInformationMessageMock.mockResolvedValueOnce(<MessageItem>{
      title: "Jawohl",
      id: "yes",
    });
    let context = createActionContextMock();
    await mocks.survey.checkShowSurvey(context, now);

    expect(mocks.showInformationMessageMock).toHaveBeenCalledTimes(1);
    expect(mocks.globalStorageFake.get<IPersistedSurveyState>("testSurvey")?.postponedUntilMs).toBeFalsy();
    expect(mocks.globalStorageFake.get<IPersistedSurveyState>("testSurvey")?.lastTakenMs).toBe(now.valueOf());

    // Try again, right before the postponement date
    now = new Date(start.valueOf() + daysToMs(postponeAfterYes) - 1);
    mocks.showInformationMessageMock.mockReset();
    context = createActionContextMock();
    await mocks.survey.checkShowSurvey(context, now);

    // Shouldn't have asked
    expect(mocks.showInformationMessageMock).not.toHaveBeenCalled();
    expect(context.telemetry.properties.shouldAsk).toBe("alreadyTaken");

    // Try again, on the postponement date
    now = new Date(start.valueOf() + daysToMs(postponeAfterYes));
    mocks.showInformationMessageMock.mockReset();
    context = createActionContextMock();
    await mocks.survey.checkShowSurvey(context, now);

    // Try again, the day after the postponement date
    now = new Date(start.valueOf() + daysToMs(postponeAfterYes + 1));
    mocks.showInformationMessageMock.mockReset();
    context = createActionContextMock();
    await mocks.survey.checkShowSurvey(context, now);

    // Should have been shown
    expect(mocks.showInformationMessageMock).toHaveBeenCalledTimes(1);
  });

  it("postpones after saying later", async () => {
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
    mocks.showInformationMessageMock.mockResolvedValueOnce(<MessageItem>{
      title: "Maybe later",
      id: "later",
    });
    let context = createActionContextMock();
    await mocks.survey.checkShowSurvey(context, now);

    expect(mocks.showInformationMessageMock).toHaveBeenCalledTimes(1);
    expect(mocks.globalStorageFake.get<IPersistedSurveyState>("testSurvey")?.postponedUntilMs).toBe(
      now.valueOf() + daysToMs(postponeLaterDays),
    );

    // Try again, a day before the postponement date
    now = new Date(start.valueOf() + daysToMs(postponeLaterDays - 1));
    mocks.showInformationMessageMock.mockReset();
    context = createActionContextMock();
    await mocks.survey.checkShowSurvey(context, now);

    // Shouldn't have asked
    expect(mocks.showInformationMessageMock).not.toHaveBeenCalled();
    expect(context.telemetry.properties.shouldAsk).toBe("postponed");

    // Try again, a day after the postponement date
    now = new Date(start.valueOf() + daysToMs(postponeLaterDays + 1));
    mocks.showInformationMessageMock.mockReset();
    context = createActionContextMock();
    await mocks.survey.checkShowSurvey(context, now);

    // Should have asked
    expect(mocks.showInformationMessageMock).toHaveBeenCalledTimes(1);
  });

  it("postpones after dismissing/ignoring prompt", async () => {
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
    mocks.showInformationMessageMock.mockResolvedValueOnce(undefined);
    let context = createActionContextMock();
    await mocks.survey.checkShowSurvey(context, now);

    expect(mocks.showInformationMessageMock).toHaveBeenCalledTimes(1);
    // Should postpone same as "later"
    expect(mocks.globalStorageFake.get<IPersistedSurveyState>("testSurvey")?.postponedUntilMs).toBe(
      now.valueOf() + daysToMs(postponeLaterDays),
    );

    // Try again, a day before the postponement date
    now = new Date(start.valueOf() + daysToMs(postponeLaterDays - 1));
    mocks.showInformationMessageMock.mockReset();
    context = createActionContextMock();
    await mocks.survey.checkShowSurvey(context, now);

    // Shouldn't have asked
    expect(mocks.showInformationMessageMock).not.toHaveBeenCalled();
    expect(context.telemetry.properties.shouldAsk).toBe("postponed");

    // Try again, a day after the postponement date
    now = new Date(start.valueOf() + daysToMs(postponeLaterDays + 1));
    mocks.showInformationMessageMock.mockReset();
    context = createActionContextMock();
    await mocks.survey.checkShowSurvey(context, now);

    // Should have asked
    expect(mocks.showInformationMessageMock).toHaveBeenCalledTimes(1);
  });

  it("resets state if corrupt", async () => {
    const mocks = createMocks({
      isSurveyAvailable: true,
    });

    // Should show in spite of corrupt state
    await mocks.globalStorageFake.update<IPersistedSurveyState>("testSurvey", {
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      lastTakenMs: <any>"whoops",
      postponedUntilMs: -1,
    });
    const context = createActionContextMock();
    await mocks.survey.checkShowSurvey(context, new Date());

    expect(mocks.showInformationMessageMock).toHaveBeenCalledTimes(1);
    expect(context.telemetry.properties.depersistStateError).toBeTruthy();
  });
});
