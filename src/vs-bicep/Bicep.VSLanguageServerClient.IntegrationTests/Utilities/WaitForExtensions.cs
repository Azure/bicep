// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Globalization;
using Microsoft.Test.Apex.Services;

namespace Bicep.VSLanguageServerClient.IntegrationTests.Utilities
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

        public static void AreEqual<T>(Func<T> getActual, T expected, TimeSpan timeout, Action<T>? onTimeout = null)
        {
            T? actual = default(T);
            bool isUnderDebugger = false;

            bool result = false;

            do
            {
                result = WaitFor.TryIsTrue(() =>
                {
                    actual = getActual();
                    return object.Equals(actual, expected);
                }, timeout);
            }
            while (!result && isUnderDebugger);

            if (!result)
            {
                string actualMessage = string.Format(CultureInfo.InvariantCulture, "Expected value == [{0}]. Last value seen == [{1}].", expected, actual);

                if (onTimeout is not null && actual is not null)
                {
                    // Call the timeout callback. Don't perform the double timeout test as the callback might not expect it.
                    onTimeout(actual);
                }
                else
                {
                    // Determine if doubling the timeout would cause the predicate to succeed
                    result = WaitFor.TryAreEqual(getActual, expected, timeout);

                    if (!result)
                    {
                        actualMessage = actualMessage + " (expired even with doubled timeout length)";
                    }
                    else
                    {
                        actualMessage = actualMessage + " (*didn't expire* with doubled timeout length)";
                    }
                }

                // This will always throw as it's always expired in this codepath
                throw new TimeoutException(string.Format(CultureInfo.InvariantCulture, "Timed out after {0} waiting for {1}", timeout, actualMessage));
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
