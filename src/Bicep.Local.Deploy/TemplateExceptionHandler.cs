// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Net;
using Azure.Deployments.Core.Entities;
using Azure.Deployments.Core.ErrorResponses;
using Azure.Deployments.Core.EventSources;
using Azure.Deployments.Core.Exceptions;
using Azure.Deployments.Engine.Host.Azure.Exceptions;
using Azure.Deployments.Engine.Host.Azure.Extensions;
using Azure.Deployments.Engine.Host.External.Helpers;
using Azure.Deployments.Templates.Exceptions;
using Microsoft.Azure.Deployments.Shared.Host;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Microsoft.WindowsAzure.ResourceStack.Common.Instrumentation;
using Microsoft.WindowsAzure.ResourceStack.Common.Storage;
using Microsoft.WindowsAzure.ResourceStack.Common.Utilities;
using Newtonsoft.Json;

namespace Bicep.Local.Deploy;

/// <summary>
/// Utility functions that handle template exceptions for deployment engine.
/// </summary>
public class TemplateExceptionHandler : ITemplateExceptionHandler
{
    private readonly IDeploymentEventSource eventSource;

    public TemplateExceptionHandler(IDeploymentEventSource eventSource)
    {
        this.eventSource = eventSource;
    }

    public Exception CreateErrorMessageException(
        HttpStatusCode httpStatus,
        DeploymentsErrorResponseCode errorCode,
        string errorMessage,
        Exception? innerException = null,
        string? target = null,
        ExtendedErrorInfo[]? details = null,
        TypedErrorInfo[]? additionalInfo = null)
    {
        if (!Enum.TryParse<DeploymentsErrorResponseCode>(errorCode.ToString(), out var error))
        {
            error = DeploymentsErrorResponseCode.NotSpecified;
        }

        return new ErrorResponseMessageException(
            httpStatus: httpStatus,
            errorCode: error,
            errorMessage: errorMessage,
            innerException: innerException,
            target: target,
            details: details,
            additionalInfo: additionalInfo);
    }

    public Exception FlattenErrorResponseException(Exception exception)
        => ErrorResponseHelper.FlattenErrorResponseException(exception, this);

    /// <summary>
    /// Handles the template language exception.
    /// </summary>
    /// <typeparam name="T">The protected method result type</typeparam>
    /// <param name="protectedMethod">The protected method.</param>
    /// <param name="deploymentId">The deployment Id.</param>
    public T HandleTemplateException<T>(Func<T> protectedMethod, string? deploymentId = null)
#pragma warning disable VSTHRD002 // TODO replace this with fully async code
        => this.HandleTemplateExceptionInternal(protectedMethod: () => Task.FromResult(protectedMethod()), deploymentId: deploymentId).Result;
#pragma warning restore VSTHRD002

    public bool IsTransientException(Exception exception)
        => exception.IsConnectivityException();

    /// <summary>
    /// Handles the template exception.
    /// </summary>
    /// <param name="protectedMethod">The protected method.</param>
    /// <param name="deploymentId">The deployment Id.</param>
    public Task HandleTemplateException(Func<Task> protectedMethod, string? deploymentId = null)
        => this.HandleTemplateExceptionInternal(
            protectedMethod: async () =>
            {
                await protectedMethod().ConfigureAwait(continueOnCapturedContext: false);
                return true;
            },
            deploymentId: deploymentId);

    /// <summary>
    /// Handles the template exception.
    /// </summary>
    /// <typeparam name="T">The protected method result type</typeparam>
    /// <param name="protectedMethod">The protected method.</param>
    /// <param name="deploymentId">The deployment Id.</param>
    public Task<T> HandleTemplateException<T>(Func<Task<T>> protectedMethod, string? deploymentId = null)
        => this.HandleTemplateExceptionInternal<T>(
            protectedMethod: async () =>
            {
                return await protectedMethod().ConfigureAwait(continueOnCapturedContext: false);
            },
            deploymentId: deploymentId);

    /// <summary>
    /// Handles the template exception.
    /// </summary>
    /// <typeparam name="T">The protected method result type</typeparam>
    /// <param name="protectedMethod">The protected method.</param>
    /// <param name="deploymentId">The id of the deployment.</param>
    private async Task<T> HandleTemplateExceptionInternal<T>(Func<Task<T>> protectedMethod, string? deploymentId = null)
    {
        try
        {
            return await protectedMethod().ConfigureAwait(continueOnCapturedContext: false);
        }
        catch (ParameterParsingException ex)
        {
            throw new ErrorResponseMessageException(
                httpStatus: HttpStatusCode.BadRequest,
                errorCode: DeploymentsErrorResponseCode.InvalidParameters,
                errorMessage: ErrorResponseMessages.ParameterParsingError.ToLocalizedMessage(ex.Message),
                innerException: ex,
                additionalInfo: new TemplateViolationErrorInfo(ex.TemplateErrorAdditionalInfo).AsArray(),
                target: deploymentId);
        }
        catch (TemplateParsingException ex)
        {
            throw new ErrorResponseMessageException(
                httpStatus: HttpStatusCode.BadRequest,
                errorCode: DeploymentsErrorResponseCode.InvalidTemplate,
                errorMessage: ErrorResponseMessages.TemplateParsingError.ToLocalizedMessage(ex.Message),
                innerException: ex,
                additionalInfo: new TemplateViolationErrorInfo(ex.TemplateErrorAdditionalInfo).AsArray(),
                target: deploymentId);
        }
        catch (TemplateValidationException ex)
        {
            throw new ErrorResponseMessageException(
                httpStatus: HttpStatusCode.BadRequest,
                errorCode: DeploymentsErrorResponseCode.InvalidTemplate,
                errorMessage: ErrorResponseMessages.TemplateValidationError.ToLocalizedMessage(ex.Message),
                innerException: ex,
                additionalInfo: new TemplateViolationErrorInfo(ex.TemplateErrorAdditionalInfo).AsArray(),
                target: deploymentId);
        }
        catch (ExpressionException ex)
        {
            throw new ErrorResponseMessageException(
                httpStatus: HttpStatusCode.BadRequest,
                errorCode: DeploymentsErrorResponseCode.InvalidTemplate,
                errorMessage: ErrorResponseMessages.TemplateExpressionErrorMessage.ToLocalizedMessage(ex.Message),
                innerException: ex,
                additionalInfo: new TemplateViolationErrorInfo(ex.TemplateErrorAdditionalInfo).AsArray(),
                target: deploymentId);
        }
        catch (JsonException ex)
        {
            // Note(shenglol): Both JsonException and ArgumentException are non-custom exceptions. We shouldn't
            // return them as bad requests. Adding logging here and below to check if we can safely remove them.
            this.eventSource.Debug(
                operationName: "TemplateExceptionUtils.HandleTemplateExceptionInternal",
                message: "A JsonException is reported as a bad request.",
                exception: ex);

            throw new ErrorResponseMessageException(
                httpStatus: HttpStatusCode.BadRequest,
                errorCode: DeploymentsErrorResponseCode.InvalidTemplate,
                errorMessage: ErrorResponseMessages.TemplateParsingError.ToLocalizedMessage(ex.Message),
                innerException: ex,
                target: deploymentId);
        }
        catch (ArgumentException ex)
        {
            this.eventSource.Debug(
                operationName: "TemplateExceptionUtils.HandleTemplateExceptionInternal",
                message: "An ArgumentException is reported as a bad request.",
                exception: ex);

            throw new ErrorResponseMessageException(
                httpStatus: HttpStatusCode.BadRequest,
                errorCode: DeploymentsErrorResponseCode.InvalidTemplate,
                errorMessage: ErrorResponseMessages.TemplateValidationError.ToLocalizedMessage(ex.Message),
                innerException: ex,
                target: deploymentId);
        }
        catch (DeploymentValidationException ex)
        {
            throw new ErrorResponseMessageException(
                httpStatus: HttpStatusCode.BadRequest,
                errorCode: DeploymentsErrorResponseCode.InvalidTemplateDeployment,
                errorMessage: ErrorResponseMessages.TemplateDeploymentValidationError.ToLocalizedMessage(ex.Message),
                innerException: ex,
                target: deploymentId);
        }
        catch (DeploymentValidationPolicyException ex)
        {
            // Note(cheggert): If the policy failure was due to a retriable RP error then pass that status code on to the caller to facilitate retries.
            if (ex.HttpStatus.IsRetryableResponse())
            {
                throw new ErrorResponseMessageException(
                    httpStatus: ex.HttpStatus,
                    errorCode: DeploymentsErrorResponseCode.PolicyEvaluationFailure,
                    errorMessage: ErrorResponseMessages.TemplatePolicyEvaluationFailureSeeDetails.ToLocalizedMessage(),
                    innerException: ex,
                    details: [ex.ExtendedErrorInfo],
                    target: deploymentId);
            }

            throw new ErrorResponseMessageException(
                httpStatus: HttpStatusCode.BadRequest,
                errorCode: DeploymentsErrorResponseCode.InvalidTemplateDeployment,
                errorMessage: ErrorResponseMessages.TemplateDeploymentValidationFailureSeeDetails.ToLocalizedMessage(),
                innerException: ex,
                details: [ex.ExtendedErrorInfo],
                target: deploymentId);
        }
        catch (AggregateException ex)
        {
            if (ex.IsFatal())
            {
                throw;
            }

            if (ex.InnerExceptions.All(innerException => innerException is DeploymentValidationException))
            {
                var concatenatedErrorMessage = ex.InnerExceptions.Select(innerException => innerException.Message).ConcatStrings(":");
                throw new ErrorResponseMessageException(
                    httpStatus: HttpStatusCode.BadRequest,
                    errorCode: DeploymentsErrorResponseCode.InvalidTemplateDeployment,
                    errorMessage: ErrorResponseMessages.TemplateDeploymentMultipleErrors.ToLocalizedMessage(concatenatedErrorMessage),
                    innerException: ex,
                    target: deploymentId);
            }

            if (ex.InnerExceptions.Any() && ex.InnerExceptions.All(innerException => innerException is DeploymentValidationPolicyException))
            {
                var details = ex.InnerExceptions
                    .Cast<DeploymentValidationPolicyException>()
                    .SelectArray(innerException => innerException.ExtendedErrorInfo);

                // Note(cheggert): If all policy failures were due to a retriable RP error then pass a retriable status code on to the caller to facilitate retries.
                if (ex.InnerExceptions.Cast<DeploymentValidationPolicyException>().All(innerException => innerException.HttpStatus.IsRetryableResponse()))
                {
                    throw new ErrorResponseMessageException(
                        httpStatus: ((DeploymentValidationPolicyException)ex.InnerExceptions.First()).HttpStatus,
                        errorCode: DeploymentsErrorResponseCode.PolicyEvaluationFailure,
                        errorMessage: ErrorResponseMessages.TemplatePolicyEvaluationFailureSeeDetails.ToLocalizedMessage(),
                        innerException: ex,
                        details: details,
                        target: deploymentId);
                }

                throw new ErrorResponseMessageException(
                    httpStatus: HttpStatusCode.BadRequest,
                    errorCode: DeploymentsErrorResponseCode.InvalidTemplateDeployment,
                    errorMessage: ErrorResponseMessages.TemplateDeploymentMultipleErrorsSeeDetails.ToLocalizedMessage(),
                    innerException: ex,
                    details: details,
                    target: deploymentId);
            }

            throw;
        }
    }

    /// <inheritdoc/>
    public bool IsErrorResponseMessageException(Exception exception, out ErrorResponseMessage errorResponse)
    {
        if (exception is ErrorResponseMessageException errorException)
        {
            errorResponse = errorException.ToErrorResponseMessage();
            return true;
        }

        errorResponse = null!;
        return false;
    }
}
