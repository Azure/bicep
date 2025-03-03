// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;

namespace Bicep.Core.Utils;

public static class ResultExtensions
{
    public static Result<TOutput, TError> Transform<TInput, TError, TOutput>(this Result<TInput, TError> result, Func<TInput, TOutput> transformFunc)
        => result.IsSuccess(out var success, out var error) ? new(transformFunc(success)) : new(error);

    public static ResultWithDiagnosticBuilder<TOutput> Transform<TInput, TOutput>(this ResultWithDiagnosticBuilder<TInput> result, Func<TInput, TOutput> transformFunc)
        => result.IsSuccess(out var success, out var error) ? new(transformFunc(success)) : new(error);

    public static ResultWithDiagnostic<TOutput> Transform<TInput, TOutput>(this ResultWithDiagnostic<TInput> result, Func<TInput, TOutput> transformFunc)
        => result.IsSuccess(out var success, out var error) ? new(transformFunc(success)) : new(error);

    public static ResultWithDiagnosticBuilder<TOutput> Transform<TInput, TOutput>(this ResultWithDiagnosticBuilder<TInput> result, Func<TInput, ResultWithDiagnosticBuilder<TOutput>> transformFunc)
        => result.IsSuccess(out var success, out var error) ? transformFunc(success) : new(error);

    public static ResultWithDiagnostic<TOutput> Transform<TInput, TOutput>(this ResultWithDiagnostic<TInput> result, Func<TInput, ResultWithDiagnostic<TOutput>> transformFunc)
        => result.IsSuccess(out var success, out var error) ? transformFunc(success) : new(error);
}
