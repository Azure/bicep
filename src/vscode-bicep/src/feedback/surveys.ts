// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import {
  commands,
  ConfigurationTarget,
  MessageItem,
  Uri,
  window,
} from "vscode";
import { IAzExtOutputChannel, parseError } from "@microsoft/vscode-azext-utils";
import { IActionContext } from "@microsoft/vscode-azext-utils";
import { callWithTelemetryAndErrorHandling } from "@microsoft/vscode-azext-utils";
import assert from "assert";
import { GlobalState, GlobalStateKeys } from "../globalState";
import https from "https";
import { daysToMs, monthsToDays } from "../utils/time";
import { getBicepConfiguration } from "../language/getBicepConfiguration";
import { bicepConfigurationKeys } from "../language/constants";

// ======================================================
// DEBUGGING
//
// To debug surveys, set the following in your settings.json:
//   "bicep.debug.surveys.debug": true, // Show debugging information in the output window
//   "bicep.debug.surveys.now": "2023-10-01 PDT" // or whatever date you want to pretend the current date/time is
//                                               //   (assumes GMT if you don't specify a time zone)
//   "bicep.debug.surveys.link:<old link>": "<new link>" // Use a different link for a survey
//       // e.g., "bicep.debug.surveys.link:bicep/surveys/annual": "bicep/surveys/testLink"
//
// To reset global state for surveys, add this:
//   "bicep.debug.surveys.clearState": true
// The global state will be cleared the next time the extension is activated (i.e., do a reload)
// You'll need to manually set it back to false if you don't want it to keep clearing on each startup
// ======================================================

// Annual HaTS survey
const hatsAnnualSurveyInfo: ISurveyInfo = {
  akaLinkToSurvey: "bicep/surveys/annual",
  // Enough to be sure we don't ask again before the next survey, but still have flexibility about sending out the next
  //   survey earlier than a year if we want to.
  postponeAfterTakenInDays: monthsToDays(6),
  surveyPrompt:
    "Could you please take 2 minutes to tell us how well Bicep is working for you?",
  postponeForLaterInDays: 2 * 7,
  surveyStateKey: GlobalStateKeys.annualSurveyStateKey,
};

const debugModeKey = "debug.surveys.debug";
const debugClearStateKey = "debug.surveys.clearState";
const debugNowDateKey = "debug.surveys.now";
const debugSurveyLinkKeyPrefix = "debug.surveys.link:";

const debuggingPrefix = "Debugging surveys";

export function showSurveys(
  globalState: GlobalState,
  outputChannel: IAzExtOutputChannel
): void {
  outputChannel?.appendLog("showSurveys start");
  checkShowSurvey(globalState, hatsAnnualSurveyInfo, outputChannel);
  outputChannel?.appendLog("showSurveys end");
}

export function checkShowSurvey(
  globalState: GlobalState,
  surveyInfo: ISurveyInfo,
  outputChannel: IAzExtOutputChannel // Will be used for debug output if debugModeKey setting is true
): void {
  outputChannel?.appendLog("checkShowSurvey start");
  // Don't wait
  callWithTelemetryAndErrorHandling(
    "survey",
    async (context: IActionContext) => {
      let now = new Date();

      // Check debugging settings
      const debugOutputChannel = getBicepConfiguration().get<boolean>(
        debugModeKey
      )
        ? outputChannel
        : undefined;
      debugOutputChannel?.appendLog(
        `Checking survey status for ${surveyInfo.akaLinkToSurvey}...`
      );

      const debugNowDate = getBicepConfiguration().get<string>(debugNowDateKey);
      if (debugNowDate) {
        now = new Date(debugNowDate);
        assert.ok(
          !isNaN(now.valueOf()),
          `Invalid value for ${debugNowDateKey}`
        );
        debugOutputChannel?.appendLog(
          `${debuggingPrefix}: WARNING: Pretending current time is ${now.toLocaleString()} ($via {debugNowDateKey})`
        );
        context.telemetry.properties.debugNowDate = debugNowDate;
        context.telemetry.suppressAll = true;
      }

      const debugTestLink = getBicepConfiguration().get<string>(
        debugSurveyLinkKeyPrefix + surveyInfo.akaLinkToSurvey
      );
      if (debugTestLink) {
        debugOutputChannel?.appendLog(
          `${debuggingPrefix}: WARNING: Replacing survey link ${surveyInfo.akaLinkToSurvey} with ${debugTestLink}`
        );
        surveyInfo.akaLinkToSurvey = debugTestLink;
      }

      const survey = new Survey(globalState, surveyInfo, debugOutputChannel);

      if (getBicepConfiguration().get<boolean>(debugClearStateKey, false)) {
        await survey.clearGlobalState();
      }

      await survey.checkShowSurvey(context, now);
    }
  );
  outputChannel?.appendLog("checkShowSurvey end");
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
    private readonly debugOutputChannel:
      | IAzExtOutputChannel
      | undefined = undefined,
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
      }
  ) {
    // noop
  }

  /**
   * Shows the survey if it's available and timely, and the user doesn't opt out.
   */
  public async checkShowSurvey(
    context: IActionContext,
    now: Date
  ): Promise<void> {
    this.debugOutputChannel?.appendLog("checkShowSurvey start");
    context.errorHandling.suppressDisplay = true;
    context.telemetry.properties.isActivationEvent = "true";
    context.telemetry.properties.akaLink = this.surveyInfo.akaLinkToSurvey;

    const surveyState = this.getPersistedSurveyState(context, now);

    const shouldAsk = await this.shouldAskToTakeSurvey(
      context,
      surveyState,
      now
    );
    context.telemetry.properties.shouldAsk = shouldAsk;
    this.debugOutputChannel?.appendLog(
      `${debuggingPrefix}: Ask to take survey ${this.surveyInfo.akaLinkToSurvey}? ${shouldAsk}`
    );

    if (shouldAsk === "ask") {
      await this.askToTakeSurvey(context, surveyState, now);
    }

    await this.updatePersistedSurveyState(surveyState);
    this.debugOutputChannel?.appendLog("checkShowSurvey end");
  }

  private static getFullSurveyLink(akaLink: string): string {
    return `https://aka.ms/${akaLink}`;
  }

  private async shouldAskToTakeSurvey(
    context: IActionContext,
    state: ISurveyState,
    now: Date
  ): Promise<"ask" | "never" | "postponed" | "unavailable" | "alreadyTaken"> {
    {
      const areSurveysEnabled = this.areSurveysEnabled();
      context.telemetry.properties.areSurveysEnabled =
        String(areSurveysEnabled);
      if (!areSurveysEnabled) {
        return "never";
      }

      context.telemetry.properties.lastTaken = state.lastTaken?.toUTCString();
      context.telemetry.properties.postonedUntil =
        state.postponedUntil?.toUTCString();

      if (
        state.postponedUntil &&
        state.postponedUntil.valueOf() > now.valueOf()
      ) {
        return "postponed";
      }

      if (state.lastTaken) {
        const okayToAskAgainMs =
          state.lastTaken.valueOf() +
          daysToMs(this.surveyInfo.postponeAfterTakenInDays);
        if (okayToAskAgainMs > now.valueOf()) {
          return "alreadyTaken";
        }
      }

      const isAvailable = await this.inject?.getIsSurveyAvailable(
        context,
        Survey.getFullSurveyLink(this.surveyInfo.akaLinkToSurvey),
        this.debugOutputChannel
      );
      context.telemetry.properties.isAvailable = String(isAvailable);
      if (!isAvailable) {
        // Try again next time
        return "unavailable";
      }

      return "ask";
    }
  }

  private getPersistedSurveyState(
    context: IActionContext,
    now: Date
  ): ISurveyState {
    this.debugOutputChannel?.appendLog("getPersistedSurveyState start");
    let retrievedState: ISurveyState;
    const key = this.surveyInfo.surveyStateKey;

    try {
      const persistedState = this.globalState.get<IPersistedSurveyState>(
        key,
        {}
      );

      const state: ISurveyState = {
        lastTaken: persistedState.lastTakenMs
          ? new Date(persistedState.lastTakenMs)
          : undefined,
        postponedUntil: persistedState.postponedUntilMs
          ? new Date(persistedState.postponedUntilMs)
          : undefined,
      };

      if (state.lastTaken && state.lastTaken.valueOf() > now.valueOf()) {
        throw new Error("lastTaken is in the future");
      }
      if (
        isNaN(state.postponedUntil?.valueOf() ?? 0) ||
        isNaN(state.lastTaken?.valueOf() ?? 0)
      ) {
        throw new Error("Persisted survey state is invalid");
      }

      retrievedState = state;
    } catch (err) {
      context.telemetry.properties.depersistStateError =
        parseError(err).message;
      retrievedState = {};
    }

    this.debugOutputChannel?.appendLog(
      `${debuggingPrefix}: Retrieved global state for ${key}: ${JSON.stringify(
        retrievedState
      )}`
    );
    this.debugOutputChannel?.appendLog("getPersistedSurveyState end");
    return retrievedState;
  }

  private async updatePersistedSurveyState(state: ISurveyState): Promise<void> {
    const key = this.surveyInfo.surveyStateKey;
    this.debugOutputChannel?.appendLog(
      `${debuggingPrefix}: Updating global state for ${key}: ${JSON.stringify(
        state
      )}`
    );

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
    now: Date
  ): Promise<void> {
    this.debugOutputChannel?.appendLog("askToTakeSurvey start");
    const neverAskAgain: MessageItem = { title: "Never ask again" };
    const later: MessageItem = { title: "Maybe later" };
    const yes: MessageItem = { title: "Sure" };
    const dismissed: MessageItem = { title: "(dismissed)" };

    const response =
      (await this.inject?.showInformationMessage(
        this.surveyInfo.surveyPrompt,
        neverAskAgain,
        later,
        yes
      )) ?? dismissed;
    context.telemetry.properties.userResponse = String(response.title);

    if (response.title === neverAskAgain.title) {
      await this.disableSurveys();
    } else if (response.title === later.title) {
      await this.postponeSurvey(
        context,
        state,
        now,
        this.surveyInfo.postponeForLaterInDays
      );
    } else if (response.title === yes.title) {
      state.lastTaken = now;
      state.postponedUntil = undefined;
      await this.inject.launchSurvey(
        context,
        this.surveyInfo,
        this.debugOutputChannel
      );
    } else {
      // Try again next time
      assert(
        response.title === dismissed.title,
        `Unexpected response: ${response.title}`
      );
    }
    this.debugOutputChannel?.appendLog("askToTakeSurvey end");
  }

  private static async launchSurvey(
    this: void,
    context: IActionContext,
    surveyInfo: ISurveyInfo,
    outputChannel: IAzExtOutputChannel | undefined
  ): Promise<void> {
    outputChannel?.appendLog("launchSurvey start");
    context.telemetry.properties.launchSurvey = "true";

    await commands.executeCommand(
      "vscode.open",
      Uri.parse(
        Survey.getFullSurveyLink(surveyInfo.akaLinkToSurvey),
        true /*strict*/
      )
    );
    outputChannel?.appendLog("launchSurvey end");
  }

  private async postponeSurvey(
    context: IActionContext,
    state: ISurveyState,
    now: Date,
    days: number
  ): Promise<void> {
    this.debugOutputChannel?.appendLog("postponeSurvey start");
    assert(days > 0, "postponeSurvey: days must be positive");

    let newDateMs = now.valueOf() + daysToMs(days);
    if (state.postponedUntil) {
      newDateMs = Math.max(newDateMs, state.postponedUntil.valueOf());
    }
    const newDate = new Date(newDateMs);

    state.postponedUntil = newDate;
    this.debugOutputChannel?.appendLog("postponeSurvey end");
  }

  public static async getIsSurveyAvailable(
    this: void,
    context: IActionContext,
    fullLink: string,
    outputChannel?: IAzExtOutputChannel
  ): Promise<boolean> {
    outputChannel?.appendLog("getIsSurveyAvailable start");
    let linkStatus = "unknown";

    try {
      const statusCode: number | undefined = await new Promise(
        (resolve, reject) => {
          outputChannel?.appendLog("getIsSurveyAvailable https");
          https
            .get(fullLink, function (res) {
              outputChannel?.appendLog("getIsSurveyAvailable resolve");
              resolve(res.statusCode);
              outputChannel?.appendLog("getIsSurveyAvailable resume");
              res.resume(); // Allow the response to be garbage collected
              outputChannel?.appendLog("getIsSurveyAvailable after resume");
            })
            .on("error", function (err) {
              outputChannel?.appendLog("getIsSurveyAvailable reject");
              // Among other errors, we end up here if the Internet is not available
              reject(err);
              outputChannel?.appendLog("getIsSurveyAvailable after reject");
            });
        }
      );

      outputChannel?.appendLog(
        "getIsSurveyAvailable statuscode=" + statusCode
      );
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
      outputChannel?.appendLog(
        "getIsSurveyAvailable catch: " + parseError(err).message
      );
      return false;
    } finally {
      context.telemetry.properties.surveyLinkStatus = linkStatus;
      outputChannel?.appendLog("getIsSurveyAvailable end");
    }
  }

  public areSurveysEnabled(): boolean {
    this.debugOutputChannel?.appendLog("areSurveysEnabled");
    return this.inject
      .provideBicepConfiguration()
      .get<boolean>(bicepConfigurationKeys.enableSurveys, true);
  }

  private async disableSurveys(): Promise<void> {
    this.debugOutputChannel?.appendLog("disableSurveys start");
    const a = this.inject
      .provideBicepConfiguration()
      .update(
        bicepConfigurationKeys.enableSurveys,
        false,
        ConfigurationTarget.Global
      );
    this.debugOutputChannel?.appendLog("disableSurveys end");
    return a;
  }

  public async clearGlobalState(): Promise<void> {
    this.debugOutputChannel?.appendLog("clearGlobalState start");
    this.debugOutputChannel?.appendLog(
      `${debuggingPrefix} WARNING: Clearing global state for ${this.surveyInfo.surveyStateKey}`
    );
    await this.globalState.update(this.surveyInfo.surveyStateKey, undefined);
    this.debugOutputChannel?.appendLog("clearGlobalState end");
  }
}
