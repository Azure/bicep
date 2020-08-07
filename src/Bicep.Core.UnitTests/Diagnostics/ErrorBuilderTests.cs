using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parser;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics
{
    [TestClass]
    public class DiagnosticBuilderTests
    {
        [TestMethod]
        public void DiagnosticBuilder_CodesAreUnique()
        {
            var errorMethods = typeof(DiagnosticBuilder.DiagnosticBuilderInternal)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(m => m.ReturnType == typeof(Diagnostic));

            // verify the above Linq is actually working
            errorMethods.Should().HaveCountGreaterThan(40);

            var builder = DiagnosticBuilder.ForPosition(new TextSpan(0, 10));

            var definedCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var errorMethod in errorMethods)
            {
                var mockParams = errorMethod.GetParameters().Select((p, index) => $"<param_{index}>");
                
                var error = errorMethod.Invoke(builder, mockParams.ToArray()) as Diagnostic;

                if (mockParams.Any())
                {
                    // verify that all the params are actually being written in the message
                    error!.Message.Should().ContainAll(mockParams, $"method {errorMethod.Name} should use all of its parameters in the format string.");
                }

                // verify that the Code is unique
                definedCodes.Should().NotContain(error!.Code, $"Method {errorMethod.Name} should be assigned a unique error code.");
                definedCodes.Add(error!.Code);
            }
        }
    }
}