using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bicep.Core.Errors;
using Bicep.Core.Parser;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Errors
{
    [TestClass]
    public class ErrorBuilderTests
    {
        [TestMethod]
        public void ErrorBuilder_ErrorCodesAreUnique()
        {
            var errorMethods = typeof(ErrorBuilder.ErrorBuilderInternal)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(m => m.ReturnType == typeof(Error));

            // verify the above Linq is actually working
            errorMethods.Should().HaveCountGreaterThan(40);

            var builder = ErrorBuilder.ForPosition(new TextSpan(0, 10));

            var definedCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var errorMethod in errorMethods)
            {
                var mockParams = errorMethod.GetParameters().Select((p, index) => $"<param_{index}>");
                
                var error = errorMethod.Invoke(builder, mockParams.ToArray()) as Error;

                if (mockParams.Any())
                {
                    // verify that all the params are actually being written in the message
                    error!.Message.Should().ContainAll(mockParams, $"method {errorMethod.Name} should use all of its parameters in the format string.");
                }

                // verify that the ErrorCode is unique
                definedCodes.Should().NotContain(error!.ErrorCode, $"Method {errorMethod.Name} should be assigned a unique error code.");
                definedCodes.Add(error!.ErrorCode);
            }
        }
    }
}