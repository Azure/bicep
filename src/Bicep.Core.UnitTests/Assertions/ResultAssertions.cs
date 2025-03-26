// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.SourceGraph;
using Bicep.Core.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Formatting;
using FluentAssertions.Primitives;

namespace Bicep.Core.UnitTests.Assertions;

public static class ResultExtensions
{
    public static ResultAssertions<TSuccess, TError> Should<TSuccess, TError>(this Result<TSuccess, TError> result)
    {
        return new ResultAssertions<TSuccess, TError>(result);
    }

    public class ResultAssertions<TSuccess, TError> : ReferenceTypeAssertions<Result<TSuccess, TError>, ResultAssertions<TSuccess, TError>>
    {
        protected override string Identifier => "Result";

        public ResultAssertions(Result<TSuccess, TError> result)
            : base(result)
        {
        }

        public AndConstraint<ResultAssertions<TSuccess, TError>> BeFailure(string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .Given<Result<TSuccess, TError>>(() => Subject)
                .ForCondition(x => !x.IsSuccess())
                .FailWith("Expected result to be a failure{reason}, but it was a success with value {0}", x => x.TryUnwrap());

            return new AndConstraint<ResultAssertions<TSuccess, TError>>(this);

        }
        public AndConstraint<ResultAssertions<TSuccess, TError>> BeFailureWithValue(TError expectedError, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .Given<Result<TSuccess, TError>>(() => Subject)
                .ForCondition(x => !x.IsSuccess())
                .FailWith("Expected result to be a failure with value {0}{reason}, but it was a success with value {1}", _ => expectedError, x => x.TryUnwrap())
                .Then
                .ForCondition(x => GetFailure(x)!.Equals(expectedError))
                .FailWith("Expected result to be a failure with value {0}{reason}, but the failure had value {1}", _ => expectedError, x => GetFailure(x));

            return new AndConstraint<ResultAssertions<TSuccess, TError>>(this);
        }

        public AndConstraint<ResultAssertions<TSuccess, TError>> BeSuccess(string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .Given<Result<TSuccess, TError>>(() => Subject)
                .ForCondition(x => x.IsSuccess())
                .FailWith("Expected result to be a success{reason}, but it was a failure with value {0}", x => GetFailure(x));

            return new AndConstraint<ResultAssertions<TSuccess, TError>>(this);

        }
        public AndConstraint<ResultAssertions<TSuccess, TError>> BeSuccessWithValue(TSuccess expectedValue, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .Given<Result<TSuccess, TError>>(() => Subject)
                .ForCondition(x => x.IsSuccess())
                .FailWith("Expected result to be a success with value {0}{reason}, but it was a failure with value {1}", _ => expectedValue, x => GetFailure(x))
                .Then
                .ForCondition(x => x.IsSuccess(out var value) && value.Equals(expectedValue))
                .FailWith("Expected result to be a success with value {0}{reason}, but the actual value was {1}", _ => expectedValue, x => x.TryUnwrap());

            return new AndConstraint<ResultAssertions<TSuccess, TError>>(this);
        }

        private static TError? GetFailure(Result<TSuccess, TError> result)
        {
            result.IsSuccess(out var _, out var failure);
            return failure;
        }

    }
}
