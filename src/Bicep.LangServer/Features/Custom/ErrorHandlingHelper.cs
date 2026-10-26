// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;

namespace Bicep.LanguageServer.Features.Custom;

public class ErrorHandlingHelper<T>
{
    private readonly IWindowLanguageServer window;

    public class ErrorHandlingException(string message, T errorResponse) : Exception(message)
    {
        public T ErrorResponse { get; } = errorResponse;
    }

    public ErrorHandlingHelper(IWindowLanguageServer window)
    {
        this.window = window;
    }

    public ErrorHandlingException CreateException(string message, T errorResponse) =>
        new(message, errorResponse);

    public async Task<T> ExecuteWithErrorHandling(Func<Task<T>> executeFunc)
    {
        try
        {
            return await executeFunc();
        }
        catch (ErrorHandlingException exception)
        {
            window.ShowError(exception.Message);

            return exception.ErrorResponse;
        }
    }
}
