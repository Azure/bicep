// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.Semantics;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Semantics
{
    [TestClass]
    public class FunctionOverloadBuilderTests
    {
        [TestMethod]
        public void NoParameters_ShouldBuildSuccessfully()
        {
            new FunctionOverloadBuilder("test").Build();
        }

        [TestMethod]
        public void AllOptionalParameters_ShouldBuildSuccessfully()
        {
            new FunctionOverloadBuilder("test")
                .WithOptionalParameter("test", LanguageConstants.Any, string.Empty)
                .WithOptionalParameter("test2", LanguageConstants.String, string.Empty)
                .Build();
        }

        [TestMethod]
        public void AllRequiredParameters_ShouldBuildSuccessfully()
        {
            new FunctionOverloadBuilder("test")
                .WithRequiredParameter("test", LanguageConstants.Any, string.Empty)
                .WithRequiredParameter("test2", LanguageConstants.String, string.Empty)
                .Build();
        }

        [TestMethod]
        public void RequiredParametersBeforeOptionals_ShouldBuildSuccessfully()
        {
            new FunctionOverloadBuilder("test")
                .WithRequiredParameter("test", LanguageConstants.Any, string.Empty)
                .WithOptionalParameter("test2", LanguageConstants.String, string.Empty)
                .Build();
        }

        [TestMethod]
        public void RequiredAndVariableParams_ShouldBuildSuccessfully()
        {
            new FunctionOverloadBuilder("test")
                .WithRequiredParameter("test", LanguageConstants.Any, string.Empty)
                .WithVariableParameter("test2", LanguageConstants.String, 1, string.Empty)
                .Build();
        }

        [TestMethod]
        public void OptionalAndVariableParams_ShouldThrow()
        {
            Action optionalAndVariable = () => new FunctionOverloadBuilder("test")
                .WithOptionalParameter("test", LanguageConstants.Any, string.Empty)
                .WithVariableParameter("test2", LanguageConstants.String, 1, string.Empty)
                .Build();

            optionalAndVariable.Should().Throw<InvalidOperationException>().WithMessage("The function overload 'test' has a variable parameter together with optional parameters, which is not supported.");
        }

        [TestMethod]
        public void RequiredParametersAfterOptional_ShouldThrow()
        {
            Action requiredFollowsOptional = () => new FunctionOverloadBuilder("test")
                .WithOptionalParameter("test", LanguageConstants.Any, string.Empty)
                .WithOptionalParameter("test2", LanguageConstants.String, string.Empty)
                .WithRequiredParameter("req", LanguageConstants.Int, string.Empty)
                .Build();

            requiredFollowsOptional.Should().Throw<InvalidOperationException>().WithMessage("Required parameter of function overload 'test' at index 2 follows an optional argument, which is not supported.");
        }
    }
}
