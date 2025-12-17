// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import assert from "assert";
import https from "https";
import { callWithTelemetryAndErrorHandling, IActionContext, parseError } from "@microsoft/vscode-azext-utils";
import { commands, MessageItem, Uri, window } from "vscode";
import { GlobalState, GlobalStateKeys } from "../globalState";
import { bicepConfigurationKeys } from "../language/constants";
import { getBicepConfiguration } from "../language/getBicepConfiguration";
import { daysToMs, monthsToDays } from "../utils/time";

// ======================================================
// DEBUGGING
//
// To debug surveys, set the following in your settings.json:
//   "bicep.debug.surveys.now": "2023-10-01 PDT" // or whatever date you want to pretend the current date/time is
//                                               //   (assumes GMT if you don't specify a time zone)
//   "bicep.debug.surveys.link:<old link>": "<new link>" // Use a different link for a survey
//       // e.g., "bicep.debug.surveys.link:bicep-csat-survey": "bicep/surveys/testLink"
//
// To reset global state for surveys, add this:
//   "bicep.debug.surveys.clearState": true
// The global state will be cleared the next time the extension is activated (i.e., do a reload)
// You'll need to manually set it back to false if you don't want it to keep clearing on each startup
// ======================================================

// Always on HaTS survey
const hatsAlwaysOnSurveyInfo: ISurveyInfo = {
  akaLinkToSurvey: "bicep-csat-survey",
  // Enough to be sure we don't ask again before the next survey, but still have flexibility about sending out the next
  //   survey earlier than a year if we want to.
  postponeAfterTakenInDays: monthsToDays(6),
  surveyPrompt: "Do you have a few minutes to tell us about your experience with Bicep?",
  postponeForLaterInDays: 7,
  surveyStateKey: GlobalStateKeys.annualSurveyStateKey,
};

const debugClearStateKey = "debug.surveys.clearState";
const debugNowDateKey = "debug.surveys.now";
const debugSurveyLinkKeyPrefix = "debug.surveys.link:";

type MessageItemWithId = MessageItem & { id: string };

export function showSurveys(globalState: GlobalState): void {
  checkShowSurvey(globalState, hatsAlwaysOnSurveyInfo);
}

export function checkShowSurvey(globalState: GlobalState, surveyInfo: ISurveyInfo): void {
  // Don't wait, run asynchronously
  void callWithTelemetryAndErrorHandling("survey", async (context: IActionContext) => {
    let now = new Date();

    // Check debugging settings
    const debugNowDate = getBicepConfiguration().get<string>(debugNowDateKey);
    if (debugNowDate) {
      now = new Date(debugNowDate);
      assert.ok(!isNaN(now.valueOf()), `Invalid value for ${debugNowDateKey}`);
      console.warn(`Debugging surveys: Pretending now is ${now.toLocaleString()}`);
      context.telemetry.properties.debugNowDate = debugNowDate;
      context.telemetry.suppressAll = true;
    }

    const debugTestLink = getBicepConfiguration().get<string>(debugSurveyLinkKeyPrefix + surveyInfo.akaLinkToSurvey);
    if (debugTestLink) {
      console.warn(`Debugging surveys: Replacing link ${surveyInfo.akaLinkToSurvey} with ${debugTestLink}`);
      surveyInfo.akaLinkToSurvey = debugTestLink;
    }

    const survey = new Survey(globalState, surveyInfo);

    if (getBicepConfiguration().get<boolean>(debugClearStateKey, false)) {
      await survey.clearGlobalState();
    }

    await survey.checkShowSurvey(context, now);
  });
}

export interface ISurveyInfo {
  akaLinkToSurvey: string; // the part after https://aka.ms/
  surveyStateKey: string; // Key used in global storage to persist the survey state
  postponeAfterTakenInDays: number;
  postponeForLaterInDays: number;
  surveyPrompt: string;
}

export interface ISurveyState {
  lastTaken?: Date;
  postponedUntil?: Date;
}

export interface IPersistedSurveyState {
  lastTakenMs?: number;
  postponedUntilMs?: number;
}

export class Survey {
  public constructor(
    private readonly globalState: GlobalState,
    private readonly surveyInfo: ISurveyInfo,
    private inject: {
      showInformationMessage: typeof window.showInformationMessage;
      getIsSurveyAvailable: typeof Survey.getIsSurveyAvailable;
      launchSurvey: typeof Survey.launchSurvey;
      provideBicepConfiguration: typeof getBicepConfiguration;
    } = {
      showInformationMessage: window.showInformationMessage,
      getIsSurveyAvailable: Survey.getIsSurveyAvailable,
      launchSurvey: Survey.launchSurvey,
      provideBicepConfiguration: getBicepConfiguration,
    },
  ) {
    // noop
  }

  /**
   * Shows the survey if it's available and timely, and the user doesn't opt out.
   */
  public async checkShowSurvey(context: IActionContext, now: Date): Promise<void> {
    context.errorHandling.suppressDisplay = true;
    context.telemetry.properties.isActivationEvent = "true";
    context.telemetry.properties.akaLink = this.surveyInfo.akaLinkToSurvey.replace("/", "-");

    const surveyState = this.getPersistedSurveyState(context, now);

    const shouldAsk = await this.shouldAskToTakeSurvey(context, surveyState, now);
    context.telemetry.properties.shouldAsk = shouldAsk;
    console.info(`Ask to take survey ${this.surveyInfo.akaLinkToSurvey}? ${shouldAsk}`);

    if (shouldAsk === "ask") {
      await this.askToTakeSurvey(context, surveyState, now);
    }

    await this.updatePersistedSurveyState(surveyState);
  }

  private static getFullSurveyLink(akaLink: string): string {
    return `https://aka.ms/${akaLink}`;
  }

  private async shouldAskToTakeSurvey(
    context: IActionContext,
    state: ISurveyState,
    now: Date,
  ): Promise<"ask" | "never" | "postponed" | "unavailable" | "alreadyTaken"> {
    {
      const areSurveysEnabled = this.areSurveysEnabled();
      context.telemetry.properties.areSurveysEnabled = String(areSurveysEnabled);
      if (!areSurveysEnabled) {
        return "never";
      }

      context.telemetry.properties.lastTaken = state.lastTaken?.toUTCString();
      context.telemetry.properties.postonedUntil = state.postponedUntil?.toUTCString();

      if (state.postponedUntil && state.postponedUntil.valueOf() > now.valueOf()) {
        return "postponed";
      }

      if (state.lastTaken) {
        const okayToAskAgainMs = state.lastTaken.valueOf() + daysToMs(this.surveyInfo.postponeAfterTakenInDays);
        if (okayToAskAgainMs > now.valueOf()) {
          return "alreadyTaken";
        }
      }

      const isAvailable = await this.inject?.getIsSurveyAvailable(
        context,
        Survey.getFullSurveyLink(this.surveyInfo.akaLinkToSurvey),
      );
      context.telemetry.properties.isAvailable = String(isAvailable);
      if (!isAvailable) {
        // Try again next time
        return "unavailable";
      }

      return "ask";
    }
  }

  private getPersistedSurveyState(context: IActionContext, now: Date): ISurveyState {
    let retrievedState: ISurveyState;
    const key = this.surveyInfo.surveyStateKey;

    try {
      const persistedState = this.globalState.get<IPersistedSurveyState>(key, {});

      const state: ISurveyState = {
        lastTaken: persistedState.lastTakenMs ? new Date(persistedState.lastTakenMs) : undefined,
        postponedUntil: persistedState.postponedUntilMs ? new Date(persistedState.postponedUntilMs) : undefined,
      };

      if (state.lastTaken && state.lastTaken.valueOf() > now.valueOf()) {
        throw new Error("lastTaken is in the future");
      }
      if (isNaN(state.postponedUntil?.valueOf() ?? 0) || isNaN(state.lastTaken?.valueOf() ?? 0)) {
        throw new Error("Persisted survey state is invalid");
      }

      retrievedState = state;
    } catch (err) {
      context.telemetry.properties.depersistStateError = parseError(err).message;
      retrievedState = {};
    }

    console.info(`Retrieved global state for ${key}:`, retrievedState);
    return retrievedState;
  }

  private async updatePersistedSurveyState(state: ISurveyState): Promise<void> {
    const key = this.surveyInfo.surveyStateKey;
    console.info(`Updating global state for ${key}:`, state);

    const persistedState: IPersistedSurveyState = {
      lastTakenMs: state.lastTaken?.valueOf(),
      postponedUntilMs: state.postponedUntil?.valueOf(),
    };

    await this.globalState.update(key, persistedState);
  }

  // TODO: If the user never responds, the telemetry event isn't sent - can fix this with a different event
  private async askToTakeSurvey(
    context: IActionContext,
    state: ISurveyState, // this is modified
    now: Date,
  ): Promise<void> {
    const dontAskAgain: MessageItemWithId = {
      title: "Don't ask again",
      id: "dontAskAgain",
    };
    const later: MessageItemWithId = { title: "Maybe later", id: "later" };
    const yes: MessageItemWithId = { title: "Yes", id: "yes" };
    const dismissed: MessageItemWithId = {
      title: "(dismissed)",
      id: "dismissed",
    };

    const response =
      (await this.inject?.showInformationMessage(this.surveyInfo.surveyPrompt, yes, later, dontAskAgain)) ?? dismissed;
    context.telemetry.properties.userResponse = String(response.id);

    if (response.id === dontAskAgain.id) {
      await this.postponeSurvey(state, now, 180);
    } else if (response.id === later.id) {
      await this.postponeSurvey(state, now, this.surveyInfo.postponeForLaterInDays);
    } else if (response.id === yes.id) {
      state.lastTaken = now;
      state.postponedUntil = undefined;
      await this.inject.launchSurvey(context, this.surveyInfo);
    } else {
      // Dismissed/ignored - treat same as "later"
      assert(response.id === dismissed.id, `Unexpected response: ${response.id}`);
      await this.postponeSurvey(state, now, this.surveyInfo.postponeForLaterInDays);
    }
  }

  private static async launchSurvey(this: void, context: IActionContext, surveyInfo: ISurveyInfo): Promise<void> {
    context.telemetry.properties.launchSurvey = "true";

    await commands.executeCommand(
      "vscode.open",
      Uri.parse(Survey.getFullSurveyLink(surveyInfo.akaLinkToSurvey), true /*strict*/),
    );
  }

  private async postponeSurvey(state: ISurveyState, now: Date, days: number): Promise<void> {
    assert(days > 0, "postponeSurvey: days must be positive");

    let newDateMs = now.valueOf() + daysToMs(days);
    if (state.postponedUntil) {
      newDateMs = Math.max(newDateMs, state.postponedUntil.valueOf());
    }
    const newDate = new Date(newDateMs);

    state.postponedUntil = newDate;
  }

  public static async getIsSurveyAvailable(this: void, context: IActionContext, fullLink: string): Promise<boolean> {
    let linkStatus = "unknown";

    try {
      const statusCode: number | undefined = await new Promise((resolve, reject) => {
        https
          .get(fullLink, function (res) {
            resolve(res.statusCode);
            res.resume(); // Allow the response to be garbage collected
          })
          .on("error", function (err) {
            // Among other errors, we end up here if the Internet is not available
            reject(err);
          });
      });

      if (statusCode === 301 /* moved permanently */) {
        // The aka link exists and is active
        linkStatus = "available";
        return true;
      } else if (statusCode === 302 /* found/moved temporarily */) {
        // The aka link either exists but is inactive, or does not exist
        linkStatus = "unavailable";
        return false;
      } else {
        linkStatus = String(statusCode);
        return false;
      }
    } catch (err) {
      linkStatus = parseError(err).errorType;
      return false;
    } finally {
      context.telemetry.properties.surveyLinkStatus = linkStatus;
    }
  }

  public areSurveysEnabled(): boolean {
    return this.inject.provideBicepConfiguration().get<boolean>(bicepConfigurationKeys.enableSurveys, true);
  }

  public async clearGlobalState(): Promise<void> {
    console.info(`Clearing global state for ${this.surveyInfo.surveyStateKey}`);
    await this.globalState.update(this.surveyInfo.surveyStateKey, undefined);
  }
}
