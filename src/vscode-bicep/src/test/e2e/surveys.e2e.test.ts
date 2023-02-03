// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Survey } from "../../feedback/surveys";
import { createActionContextMock } from "../mocks/actionContextMock";

describe("surveys-e2etests", () => {
  describe("getIsSurveyAvailable", () => {
    it("active aka survey link", async () => {
      const context = createActionContextMock();

      const isAvailable = await Survey.getIsSurveyAvailable(
        context,
        // This test depends on this aka.ms link existing and being active (can point to https://github.com/Azure/bicep/)
        "https://aka.ms/bicep/tests/surveytests/active"
      );

      expect(isAvailable).toBeTruthy();
      expect(context.telemetry.properties.surveyLinkStatus).toBe("available");
    });

    it("inactive aka survey link", async () => {
      const context = createActionContextMock();

      const isAvailable = await Survey.getIsSurveyAvailable(
        context,
        // This test depends on this aka.ms link existing and being set to inactive (can point to https://github.com/Azure/bicep/)
        "https://aka.ms/bicep/tests/surveytests/inactive"
      );

      expect(isAvailable).toBeFalsy();
      expect(context.telemetry.properties.surveyLinkStatus).toBe("unavailable");
    });

    it("invalid aka link (indicating there's no survey link)", async () => {
      const context = createActionContextMock();

      const isAvailable = await Survey.getIsSurveyAvailable(
        context,
        "https://aka.ms/bicep/tests/surveytests/this/link/does/not/exist"
      );

      expect(isAvailable).toBeFalsy();
      expect(context.telemetry.properties.surveyLinkStatus).toBe("unavailable");
    });

    it("host not found (or Internet not available)", async () => {
      const context = createActionContextMock();

      // This test depends on this aka.ms link existing and being active
      const isAvailable = await Survey.getIsSurveyAvailable(
        context,
        "https://whoops.misspelled.aka.ms/bicep/tests/surveytests/active"
      );

      expect(isAvailable).toBeFalsy();
      expect(context.telemetry.properties.surveyLinkStatus).toBe("ENOTFOUND");
    });

    // eslint-disable-next-line jest/prefer-lowercase-title
    it("Other errors", async () => {
      const context = createActionContextMock();

      const isAvailable = await Survey.getIsSurveyAvailable(
        context,
        "foo://aka.ms/bicep/tests/surveytests/active"
      );

      expect(isAvailable).toBeFalsy();
      expect(context.telemetry.properties.surveyLinkStatus).toBe(
        "ERR_INVALID_PROTOCOL"
      );
    });

    it("other status code returned", async () => {
      const context = createActionContextMock();

      const isAvailable = await Survey.getIsSurveyAvailable(
        context,
        "https://github.com/Azure/bicep"
      );

      expect(isAvailable).toBeFalsy();
      expect(context.telemetry.properties.surveyLinkStatus).toBe("200");
    });
  });
});
