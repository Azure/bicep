// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Azure.Deployments.Core.EventSources;
using Bicep.Core.Features;
using Microsoft.WindowsAzure.ResourceStack.Common.BackgroundJobs;
using Microsoft.WindowsAzure.ResourceStack.Common.EventSources;
using Newtonsoft.Json.Linq;

namespace Bicep.Local.Deploy.Engine;

#nullable disable

public class TraceEventSource : ICommonEventSource, IDeploymentEventSource
{
    private static void WriteToTrace(
        TraceVerbosity verbosity,
        object paramsObj,
        [CallerMemberName] string callingMethod = "")
    {
        if (!FeatureProvider.HasTracingVerbosity(verbosity))
        {
            return;
        }

        var sb = new StringBuilder();
        sb.AppendFormat("{0}: ", callingMethod);

#pragma warning disable IL2075 // 'this' argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The return value of the source method does not have matching annotations.
        var properties = paramsObj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
#pragma warning restore IL2075 // 'this' argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The return value of the source method does not have matching annotations.
        foreach (var prop in properties)
        {
            var name = prop.Name;
            var value = prop.GetValue(paramsObj, null);

            if (value is null ||
                (value is string stringValue && stringValue == ""))
            {
                continue;
            }

            sb.AppendFormat("{0}={1}, ", name, value);
        }

        var traceLine = sb.ToString();
        // exclude unnecessarily noisy traces
        if (traceLine.Contains("operationName=EnablementConfigExtensions.") ||
            traceLine.Contains("operationName=DataProviderHolderBase."))
        {
            return;
        }

        Trace.WriteLine(traceLine);
    }

    public void Critical(string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string operationName, string message, string exception, string organizationId, string activityVector, string realPuid, string altSecId, string additionalProperties)
        => WriteToTrace(TraceVerbosity.Basic, new { operationName, message, exception, additionalProperties, });

    public void Critical(string operationName, string message, Exception exception = null)
        => WriteToTrace(TraceVerbosity.Basic, new { operationName, message, exception, });

    public void Debug(string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string operationName, string message, string exception, string organizationId, string activityVector, string realPuid, string altSecId, string additionalProperties)
        => WriteToTrace(TraceVerbosity.Full, new { operationName, message, exception, additionalProperties, });

    public void Debug(string operationName, string message, Exception exception = null)
        => WriteToTrace(TraceVerbosity.Full, new { operationName, message, exception, });

    public void DeploymentOperations(string apiVersion, string resourceGroupName, string resourceGroupLocation, string deploymentName, string deploymentSequenceId, string operationId, DateTime startTime, string executionStatus, string statusCode, string statusMessage, string providerNamespace, string provisioningOperation, string resourceType, string resourceName, string resourceId, string resourceLocation, string resourceExtendedLocation, long totalExecutionCount, string resourceReferenceType, string additionalProperties)
        => WriteToTrace(TraceVerbosity.Full, new { deploymentName, provisioningOperation, startTime, executionStatus, resourceId, additionalProperties });

    public void Deployments(string resourceGroupName, string resourceGroupLocation, string frontdoorLocation, string deploymentName, string deploymentSequenceId, string templateHash, string parametersHash, DateTime startTime, string executionStatus, string onErrorDeploymentName, string deploymentMode, string templateLinkId, string templateLinkUri, long resourceProviderCount, long resourceTypeCount, long locationCount, long dependencyCount, string debugSetting, string generatorName, string generatorVersion, string generatorTemplateHash, string deploymentResourceId, string additionalProperties, SequencerAction[] sequencerActions)
        => WriteToTrace(TraceVerbosity.Full, new { deploymentName, startTime, executionStatus, additionalProperties });

    public void DispatcherCritical(string dispatcherName, string operationName, string message, string queueMessage, string exception, string errorCode, int dequeueCount, string insertionTime, string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string organizationId, string activityVector, string realPuid, string altSecId, string additionalProperties)
        => WriteToTrace(TraceVerbosity.Basic, new { operationName, message, exception, additionalProperties, });

    public void DispatcherDebug(string dispatcherName, string operationName, string message, string queueMessage, string exception, string errorCode, int dequeueCount, string insertionTime, string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string organizationId, string activityVector, string realPuid, string altSecId, string additionalProperties)
        => WriteToTrace(TraceVerbosity.Full, new { operationName, message, exception, additionalProperties, });

    public void DispatcherError(string dispatcherName, string operationName, string message, string queueMessage, string exception, string errorCode, int dequeueCount, string insertionTime, string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string organizationId, string activityVector, string realPuid, string altSecId, string additionalProperties)
        => WriteToTrace(TraceVerbosity.Basic, new { operationName, message, exception, additionalProperties, });

    public void DispatcherOperation(string dispatcherName, string operationName, string message, string queueMessage, string exception, string errorCode, int dequeueCount, string insertionTime, string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string organizationId, string activityVector, string realPuid, string altSecId, string additionalProperties)
        => WriteToTrace(TraceVerbosity.Full, new { operationName, message, exception, additionalProperties, });

    public void DispatcherQueueDepth(string dispatcherName, string storageAccount, string queue, double depth, string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string organizationId, string activityVector, string realPuid, string altSecId, string additionalProperties)
    {
    }

    public void DispatcherWarning(string dispatcherName, string operationName, string message, string queueMessage, string exception, string errorCode, int dequeueCount, string insertionTime, string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string organizationId, string activityVector, string realPuid, string altSecId, string additionalProperties)
        => WriteToTrace(TraceVerbosity.Basic, new { operationName, message, exception, additionalProperties, });

    public void Error(string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string operationName, string message, string exception, string organizationId, string activityVector, string realPuid, string altSecId, string additionalProperties)
        => WriteToTrace(TraceVerbosity.Basic, new { operationName, message, exception, additionalProperties, });

    public void Error(string operationName, string message, Exception exception = null)
        => WriteToTrace(TraceVerbosity.Basic, new { operationName, message, exception, });

    public void EventServiceEntry(string operationName, string resourceProvider, string resourceUri, string eventName, string status, int channels, string jobId, string jobType, string managementGroupHierarchy)
    {
    }

    public void HttpIncomingRequestEndWithClientFailure(string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string authorizationSource, string authorizationAction, string operationName, string httpMethod, string hostName, string targetUri, string userAgent, string clientRequestId, string clientSessionId, string clientIpAddress, string clientApplicationId, string apiVersion, long contentLength, string serviceRequestId, long durationInMilliseconds, int httpStatusCode, string exceptionMessage, string errorCode, string failureCause, string errorMessage, string referer, string commandName, string parameterSetName, string contentType, string contentEncoding, string armServiceRequestId, string organizationId, string activityVector, string locale, string realPuid, string altSecId, string additionalProperties, string targetResourceProvider, string targetResourceType)
    {
    }

    public void HttpIncomingRequestEndWithServerFailure(string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string authorizationSource, string authorizationAction, string operationName, string httpMethod, string hostName, string targetUri, string userAgent, string clientRequestId, string clientSessionId, string clientIpAddress, string clientApplicationId, string apiVersion, long contentLength, string serviceRequestId, long durationInMilliseconds, int httpStatusCode, string exceptionMessage, string errorCode, string failureCause, string errorMessage, string referer, string commandName, string parameterSetName, string contentType, string contentEncoding, string armServiceRequestId, string organizationId, string activityVector, string locale, string realPuid, string altSecId, string additionalProperties, string targetResourceProvider, string targetResourceType)
    {
    }

    public void HttpIncomingRequestEndWithSuccess(string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string authorizationSource, string authorizationAction, string operationName, string httpMethod, string hostName, string targetUri, string userAgent, string clientRequestId, string clientSessionId, string clientIpAddress, string clientApplicationId, string apiVersion, long contentLength, string serviceRequestId, long durationInMilliseconds, int httpStatusCode, string exceptionMessage, string errorCode, string failureCause, string errorMessage, string referer, string commandName, string parameterSetName, string contentType, string contentEncoding, string armServiceRequestId, string organizationId, string activityVector, string locale, string realPuid, string altSecId, string additionalProperties, string targetResourceProvider, string targetResourceType)
    {
    }

    public void HttpIncomingRequestStart(string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string authorizationSource, string authorizationAction, string operationName, string httpMethod, string hostName, string targetUri, string userAgent, string clientRequestId, string clientSessionId, string clientIpAddress, string clientApplicationId, string apiVersion, long contentLength, string serviceRequestId, long durationInMilliseconds, int httpStatusCode, string exceptionMessage, string errorCode, string failureCause, string errorMessage, string referer, string commandName, string parameterSetName, string contentType, string contentEncoding, string armServiceRequestId, string organizationId, string activityVector, string locale, string realPuid, string altSecId, string additionalProperties, string targetResourceProvider, string targetResourceType)
    {
    }

    public void HttpOutgoingRequestEndWithClientFailure(string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string operationName, string httpMethod, string hostName, string targetUri, string clientRequestId, string clientSessionId, string clientApplicationId, string apiVersion, long contentLength, string serviceRequestId, long durationInMilliseconds, int httpStatusCode, string exceptionMessage, string errorCode, string errorMessage, string referer, string failureCause, string contentType, string contentEncoding, string armServiceRequestId, string organizationId, string activityVector, string locale, string realPuid, string altSecId, string additionalProperties, string targetResourceProvider, string targetResourceType)
    {
    }

    public void HttpOutgoingRequestEndWithServerFailure(string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string operationName, string httpMethod, string hostName, string targetUri, string clientRequestId, string clientSessionId, string clientApplicationId, string apiVersion, long contentLength, string serviceRequestId, long durationInMilliseconds, int httpStatusCode, string exceptionMessage, string errorCode, string errorMessage, string referer, string failureCause, string contentType, string contentEncoding, string armServiceRequestId, string organizationId, string activityVector, string locale, string realPuid, string altSecId, string additionalProperties, string targetResourceProvider, string targetResourceType)
    {
    }

    public void HttpOutgoingRequestEndWithSuccess(string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string operationName, string httpMethod, string hostName, string targetUri, string clientRequestId, string clientSessionId, string clientApplicationId, string apiVersion, long contentLength, string serviceRequestId, long durationInMilliseconds, int httpStatusCode, string exceptionMessage, string errorCode, string errorMessage, string referer, string failureCause, string contentType, string contentEncoding, string armServiceRequestId, string organizationId, string activityVector, string locale, string realPuid, string altSecId, string additionalProperties, string targetResourceProvider, string targetResourceType)
    {
    }

    public void HttpOutgoingRequestStart(string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string operationName, string httpMethod, string hostName, string targetUri, string clientRequestId, string clientSessionId, string clientApplicationId, string apiVersion, long contentLength, string serviceRequestId, long durationInMilliseconds, int httpStatusCode, string exceptionMessage, string errorCode, string errorMessage, string referer, string failureCause, string contentType, string contentEncoding, string armServiceRequestId, string organizationId, string activityVector, string locale, string realPuid, string altSecId, string additionalProperties, string targetResourceProvider, string targetResourceType)
    {
    }

    public void JobCritical(string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string operationName, string jobPartition, string jobId, string message, string exception, string organizationId, string activityVector, string realPuid, string altSecId, string additionalProperties)
        => WriteToTrace(TraceVerbosity.Basic, new { operationName, jobPartition, jobId, message, exception, additionalProperties, });

    public void JobDebug(string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string operationName, string jobPartition, string jobId, string message, string exception, string organizationId, string activityVector, string realPuid, string altSecId, string additionalProperties)
        => WriteToTrace(TraceVerbosity.Full, new { operationName, jobPartition, jobId, message, exception, additionalProperties, });

    public void JobDefinition(string jobPartition, string jobId, string version, string callback, string location, string locationsAffinity, string flags, string state, string executionState, string startTime, string endTime, int repeatCount, long repeatInterval, string repeatUnit, string repeatSchedule, int currentRepeatCount, int retryCount, long retryInterval, string retryUnit, int currentRetryCount, int currentExecutionCount, string timeout, string retention, string nextExecutionTime, string lastExecutionTime, string lastExecutionStatus, string createdTime, string changedTime, string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, int totalSucceededCount, int totalCompletedCount, int totalFailedCount, int totalFaultedCount, int totalPostponedCount, string parentJobCompletionTrigger, string organizationId, string activityVector, string realPuid, string altSecId, string additionalProperties)
    {
    }

    public void JobDispatchingError(string operationName, string jobPartition, string jobId, string message, string exception, string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string organizationId, string activityVector, string realPuid, string altSecId, string additionalProperties)
        => WriteToTrace(TraceVerbosity.Basic, new { operationName, jobPartition, jobId, message, exception, additionalProperties, });

    public void JobError(string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string operationName, string jobPartition, string jobId, string message, string exception, string organizationId, string activityVector, string realPuid, string altSecId, string additionalProperties)
        => WriteToTrace(TraceVerbosity.Basic, new { operationName, jobPartition, jobId, message, exception, additionalProperties, });

    public void JobExecutionStatus(string jobType, string jobPartition, string jobId, string operationName, string resourceProvider, string resourceType, string resourceName, string executionInterval, string executionDelay, DateTime expectedStartTime, DateTime startTime, DateTime endTime, string executionStatus, string executionFailureCause, string executionFailureDetails, string nextExecutionTime, string lastDependencyJob, string jobStatus, string jobCompletionStatus, string jobFailureCause, string jobDuration)
        => WriteToTrace(TraceVerbosity.Full, new { operationName, jobPartition, jobId, executionStatus, executionFailureCause, executionFailureDetails, nextExecutionTime, });

    public void JobHistory(string jobPartition, string jobId, string callback, string startTime, string endTime, string executionTimeInMilliseconds, string executionDelayInMilliseconds, string executionIntervalInMilliseconds, string executionStatus, string executionMessage, string executionDetails, string nextExecutionTime, string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string dequeueCount, string advanceVersion, string triggerId, string messageId, string state, string organizationId, string activityVector, string realPuid, string altSecId, string additionalProperties, string jobDurabilityLevel)
        => WriteToTrace(TraceVerbosity.Full, new { jobPartition, jobId, callback, executionStatus, executionMessage, executionDetails, additionalProperties, });

    public void JobOperation(string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string operationName, string jobPartition, string jobId, string message, string exception, string organizationId, string activityVector, string realPuid, string altSecId, string additionalProperties)
        => WriteToTrace(TraceVerbosity.Full, new { operationName, jobPartition, jobId, message, exception, additionalProperties, });

    public void JobStatus(string jobType, string jobPartition, string jobId, string operationName, string resourceProvider, string resourceType, string resourceName, string resourceId, string jobStatus, string jobCompletionStatus, string jobFailureCause, string jobFailureDetails, string jobDuration, double jobDurationMs, string resourceLocation, string message, string customerOperationName = null)
        => WriteToTrace(TraceVerbosity.Full, new { operationName, jobPartition, jobId, message, jobStatus, });

    public void JobWarning(string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string operationName, string jobPartition, string jobId, string message, string exception, string organizationId, string activityVector, string realPuid, string altSecId, string additionalProperties)
        => WriteToTrace(TraceVerbosity.Basic, new { operationName, jobPartition, jobId, message, exception, additionalProperties, });

    public void PreflightEvent(string correlationId, string subscriptionId, string tenantId, string principalOid, string principalPuid, string clientApplicationId, string templateHash, string parametersHash, string startTime, string endTime, double durationInMilliseconds, string validationStatus, string statusCode, string message, string preflightSource, string userAgent, string apiVersion, string operationName)
    {
    }

    public void ProviderContractViolation(string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string operationName, string providerNamespace, string resourceType, string message, string hostName, string exception)
        => WriteToTrace(TraceVerbosity.Basic, new { operationName, message, exception, });

    public void ProviderCritical(string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string operationName, string providerNamespace, string resourceType, string message, string exception)
        => WriteToTrace(TraceVerbosity.Basic, new { operationName, message, exception, });

    public void ProviderDebug(string providerNamespace, string resourceType, string operationName, string message, Exception exception = null)
        => WriteToTrace(TraceVerbosity.Full, new { operationName, message, exception, });

    public void ProviderDebug(string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string operationName, string providerNamespace, string resourceType, string message, string exception, string headers)
        => WriteToTrace(TraceVerbosity.Full, new { operationName, message, exception, });

    public void ProviderError(string providerNamespace, string resourceType, string operationName, string message, Exception exception = null)
        => WriteToTrace(TraceVerbosity.Basic, new { operationName, message, exception, });

    public void ProviderError(string providerNamespace, string resourceType, string operationName, string message, string hostName, Exception exception = null)
        => WriteToTrace(TraceVerbosity.Basic, new { operationName, message, exception, });

    public void ProviderError(string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string operationName, string providerNamespace, string resourceType, string message, string hostName, string exception)
        => WriteToTrace(TraceVerbosity.Basic, new { operationName, message, exception, });

    public void ProviderWarning(string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string operationName, string providerNamespace, string resourceType, string message, string exception)
        => WriteToTrace(TraceVerbosity.Basic, new { operationName, message, exception, });

    public void RedisOperationEndWithFailure(string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string operationName, string accountName, long durationInMilliseconds, string exceptionMessage, string organizationId, string activityVector, string realPuid, string altSecId, string databaseId, string cacheKey, string additionalProperties)
    {
    }

    public void RedisOperationEndWithSuccess(string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string operationName, string accountName, long durationInMilliseconds, string exceptionMessage, string organizationId, string activityVector, string realPuid, string altSecId, string databaseId, string cacheKey, string additionalProperties)
    {
    }

    public void RedisOperationStart(string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string operationName, string accountName, long durationInMilliseconds, string exceptionMessage, string organizationId, string activityVector, string realPuid, string altSecId, string databaseId, string cacheKey, string additionalProperties)
    {
    }

    public void ServiceConfiguration(string serviceName, string version, string name, string value, string message, string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string organizationId, string activityVector, string realPuid, string altSecId, string additionalProperties)
        => WriteToTrace(TraceVerbosity.Full, new { message, additionalProperties, });

    public void ServiceStarted(string serviceName, string version, string name, string value, string message, string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string organizationId, string activityVector, string realPuid, string altSecId, string additionalProperties)
        => WriteToTrace(TraceVerbosity.Full, new { message, additionalProperties, });

    public void ServiceStarting(string serviceName, string name, string value, string version, string message, string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string organizationId, string activityVector, string realPuid, string altSecId, string additionalProperties)
        => WriteToTrace(TraceVerbosity.Full, new { message, additionalProperties, });

    public void ServiceStopped(string serviceName, string version, string name, string value, string message, string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string organizationId, string activityVector, string realPuid, string altSecId, string additionalProperties)
        => WriteToTrace(TraceVerbosity.Full, new { message, additionalProperties, });

    public void ServiceStopping(string serviceName, string version, string name, string value, string message, string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string organizationId, string activityVector, string realPuid, string altSecId, string additionalProperties)
        => WriteToTrace(TraceVerbosity.Full, new { message, additionalProperties, });

    public void StorageOperation(string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string operationName, string accountName, string resourceType, string resourceName, string clientRequestId, string operationStatus, long durationInMilliseconds, string exceptionMessage, int requestsStarted, int requestsCompleted, int requestsTimedout, string requestsDetails, string organizationId, string activityVector, long ingressBytes, long egressBytes, string realPuid, string altSecId, string additionalProperties)
    {
    }

    public void StorageRequestEndWithClientFailure(string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string operationName, string accountName, string resourceType, string resourceName, string clientRequestId, string serviceRequestId, long durationInMilliseconds, int httpStatusCode, string exceptionMessage, string errorCode, string errorMessage, string organizationId, string activityVector, string realPuid, string altSecId, string additionalProperties)
    {
    }

    public void StorageRequestEndWithServerFailure(string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string operationName, string accountName, string resourceType, string resourceName, string clientRequestId, string serviceRequestId, long durationInMilliseconds, int httpStatusCode, string exceptionMessage, string errorCode, string errorMessage, string organizationId, string activityVector, string realPuid, string altSecId, string additionalProperties)
    {
    }

    public void StorageRequestEndWithSuccess(string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string operationName, string accountName, string resourceType, string resourceName, string clientRequestId, string serviceRequestId, long durationInMilliseconds, int httpStatusCode, string exceptionMessage, string errorCode, string errorMessage, string organizationId, string activityVector, string realPuid, string altSecId, string additionalProperties)
    {
    }

    public void StorageRequestStart(string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string operationName, string accountName, string resourceType, string resourceName, string clientRequestId, string serviceRequestId, long durationInMilliseconds, int httpStatusCode, string exceptionMessage, string errorCode, string errorMessage, string organizationId, string activityVector, string realPuid, string altSecId, string additionalProperties)
    {
    }

    public void Warning(string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string operationName, string message, string exception, string organizationId, string activityVector, string realPuid, string altSecId, string additionalProperties)
        => WriteToTrace(TraceVerbosity.Basic, new { operationName, message, exception, additionalProperties, });

    public void Warning(string operationName, string message, Exception exception = null)
        => WriteToTrace(TraceVerbosity.Basic, new { operationName, message, exception, });
}
