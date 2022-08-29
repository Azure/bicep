// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Globalization;
using Microsoft.Test.Apex.Services;

namespace Bicep.VSLanguageServerClient.TestServices.Utilitites
{
    public static class WaitForExtensions
    {
        public static void IsTrue(Func<bool> predicate, TimeSpan timeout, Func<string>? onTimeout = null, string? conditionDescription = null)
        {
            bool result = WaitFor.TryIsTrue(predicate, timeout);

            if (!result)
            {
                conditionDescription ??= "the condition to become true";

                if (onTimeout != null)
                {
                    // Call the timeout callback.
                    conditionDescription = string.Concat(conditionDescription, "\r\n onTimeout result: ", onTimeout(), "\r\n");
                }

                // Determine if doubling the timeout would cause the predicate to succeed
                result = WaitFor.TryIsTrue(predicate, timeout);

                if (!result)
                {
                    conditionDescription = conditionDescription + " (expired even with doubled timeout length)";
                }
                else
                {
                    conditionDescription = conditionDescription + " (*didn't expire* with doubled timeout length)";
                }

                // This will always throw as it's always expired in this codepath
                throw new TimeoutException(string.Format(CultureInfo.InvariantCulture, "Timed out after {0} waiting for {1}", timeout, conditionDescription));
            }
        }

        public static T? IsNotNull<T>(Func<T> func, int timeout = 5000) where T : class
        {
            T? result = null;
            IsTrue(() =>
            {
                result = func();
                return result != null;
            }, TimeSpan.FromMilliseconds(timeout));

            return result;
        }

        public static T? TryIsNotNull<T>(Func<T> func, int timeout = 5000) where T : class
        {
            T? result = null;
            WaitFor.TryIsTrue(() =>
            {
                result = func();
                return result != null;
            }, TimeSpan.FromMilliseconds(timeout));

            return result;
        }
    }
}
