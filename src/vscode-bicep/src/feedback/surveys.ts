// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { commands, MessageItem, Uri, window } from "vscode";
import { parseError } from "@microsoft/vscode-azext-utils";
import { IActionContext } from "@microsoft/vscode-azext-utils";
import { callWithTelemetryAndErrorHandling } from "@microsoft/vscode-azext-utils";
import assert from "assert";
import { GlobalState, GlobalStateKeys } from "../globalState";
import https from "https";
import { daysToMs, monthsToDays } from "../utils/time";

// Annual HaTS survey
const hatsAnnualSurveyInfo: ISurveyInfo = {
  akaLinkToSurvey: "bicep/surveys/annual",
  // Enough to be sure we don't ask again before the next survey, but still have flexibility about sending out the next
  //   survey earlier in the year if we want to.
  postponeAfterTakenInDays: monthsToDays(6),
  surveyPrompt:
    "Could you please take 2 minutes to tell us how well Bicep is working for you?",
  postponeForLaterInDays: 1 * 7,
  surveyStateKey: GlobalStateKeys.annualSurveyStateKey,
};

export function showSurveys(globalState: GlobalState): void {
  checkShowSurvey(new Survey(globalState, hatsAnnualSurveyInfo));
}

export function checkShowSurvey(survey: Survey): void {
  // Don't wait
  callWithTelemetryAndErrorHandling(
    "survey",
    async (context: IActionContext) => {
      await survey.checkShowSurvey(context, new Date());
    }
  );
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
    } = {
        showInformationMessage: window.showInformationMessage,
        getIsSurveyAvailable: Survey.getIsSurveyAvailable,
        launchSurvey: Survey.launchSurvey,
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
    context.errorHandling.suppressDisplay = true;
    context.telemetry.properties.isActivationEvent = "true";
    context.telemetry.properties.akaLink = this.surveyInfo.akaLinkToSurvey;

    const surveyState = this.getPersistedSurveyState(context, now);

    const shouldShowSurvey = await this.shouldAskToTakeSurvey(
      context,
      surveyState,
      now
    );

    if (shouldShowSurvey) {
      await this.askToTakeSurvey(context, surveyState, now);
    }

    await this.updatePersistedSurveyState(context, surveyState);
  }

  private static getFullSurveyLink(akaLink: string): string {
    return `https://aka.ms/${akaLink}`;
  }

  private async shouldAskToTakeSurvey(
    context: IActionContext,
    state: ISurveyState,
    now: Date
  ): Promise<boolean> {
    {
      const neverShowSurveys = this.getShouldNeverShowSurveys();
      context.telemetry.properties.neverShowSurvey = String(neverShowSurveys);
      if (neverShowSurveys) {
        context.telemetry.properties.shouldAsk = "never";
        return false;
      }

      context.telemetry.properties.lastTaken = state.lastTaken?.toUTCString();
      context.telemetry.properties.postonedUntil =
        state.postponedUntil?.toUTCString();

      if (
        state.postponedUntil &&
        state.postponedUntil.valueOf() > now.valueOf()
      ) {
        context.telemetry.properties.shouldAsk = "postponed";
        return false;
      }

      if (state.lastTaken) {
        const okayToAskAgainMs =
          state.lastTaken.valueOf() +
          daysToMs(this.surveyInfo.postponeAfterTakenInDays);
        if (okayToAskAgainMs > now.valueOf()) {
          context.telemetry.properties.shouldAsk = "alreadyTaken";
          return false;
        }
      }

      const isAvailable = await this.inject?.getIsSurveyAvailable(
        context,
        Survey.getFullSurveyLink(this.surveyInfo.akaLinkToSurvey)
      );
      context.telemetry.properties.isAvailable = String(isAvailable);
      if (!isAvailable) {
        // Try again next time
        context.telemetry.properties.shouldAsk = "unavailable";
        return false;
      }

      context.telemetry.properties.shouldAsk = "true";
      return true;
    }
  }

  private getPersistedSurveyState(
    context: IActionContext,
    now: Date
  ): ISurveyState {
    try {
      const persistedState = this.globalState.get<IPersistedSurveyState>(
        this.surveyInfo.surveyStateKey,
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

      return state;
    } catch (err) {
      context.telemetry.properties.depersistStateError =
        parseError(err).message;
      return {};
    }
  }

  private async updatePersistedSurveyState(
    context: IActionContext,
    state: ISurveyState
  ): Promise<void> {
    const persistedState: IPersistedSurveyState = {
      lastTakenMs: state.lastTaken?.valueOf(),
      postponedUntilMs: state.postponedUntil?.valueOf(),
    };
    await this.globalState.update(
      this.surveyInfo.surveyStateKey,
      persistedState
    );
  }

  // TODO: If the user never responds, the telemetry event isn't sent - can fix this with a different event
  private async askToTakeSurvey(
    context: IActionContext,
    state: ISurveyState, // this is modified
    now: Date
  ): Promise<void> {
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
      await this.updateShouldNeverShowSurveys(context, true);
    } else if (response.title === later.title) {
      await this.postponeSurvey(
        context,
        state,
        now,
        this.surveyInfo.postponeForLaterInDays
      );
    } else if (response.title === yes.title) {
      state.lastTaken = now;
      await this.inject.launchSurvey(context, this.surveyInfo);
    } else {
      // Try again next time
      assert(
        response.title === dismissed.title,
        `Unexpected response: ${response.title}`
      );
    }
  }

  private static async launchSurvey(
    this: void,
    context: IActionContext,
    surveyInfo: ISurveyInfo
  ): Promise<void> {
    context.telemetry.properties.launchSurvey = "true";

    await commands.executeCommand(
      "vscode.open",
      Uri.parse(
        Survey.getFullSurveyLink(surveyInfo.akaLinkToSurvey),
        true /*strict*/
      )
    );
  }

  private async postponeSurvey(
    context: IActionContext,
    state: ISurveyState,
    now: Date,
    days: number
  ): Promise<void> {
    assert(days > 0);

    let newDateMs = now.valueOf() + daysToMs(days);
    if (state.postponedUntil) {
      newDateMs = Math.max(newDateMs, state.postponedUntil.valueOf());
    }
    const newDate = new Date(newDateMs);

    state.postponedUntil = newDate;
  }

  public static async getIsSurveyAvailable(
    this: void,
    context: IActionContext,
    fullLink: string
  ): Promise<boolean> {
    let linkStatus = "unknown";

    try {
      const statusCode: number | undefined = await new Promise(
        (resolve, reject) => {
          https
            .get(fullLink, function (res) {
              resolve(res.statusCode);
              res.resume(); // Allow the response to be garbage collected
            })
            .on("error", function (err) {
              // Among other errors, we end up here if the Internet is not available
              reject(err);
            });
        }
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
      return false;
    } finally {
      context.telemetry.properties.linkStatus = linkStatus;
    }
  }

  public getShouldNeverShowSurveys(): boolean {
    return this.globalState.get<boolean>(
      GlobalStateKeys.neverShowSurveyKey,
      false
    );
  }

  private async updateShouldNeverShowSurveys(
    context: IActionContext,
    neverShowSurveys: boolean
  ): Promise<void> {
    await this.globalState.update(
      GlobalStateKeys.neverShowSurveyKey,
      neverShowSurveys
    );
  }
}
