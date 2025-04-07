// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Numerics;
using System.Text;
using Azure.Deployments.Core.Diagnostics;
using Azure.Deployments.Expression.Expressions;
using Azure.Deployments.Templates.Extensions;
using Azure.Identity;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Intermediate;
using Bicep.Core.Modules;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;
using static Bicep.Core.Semantics.FunctionOverloadBuilder;

namespace Bicep.Core.Semantics.Namespaces;

public static class Ev2NamespaceType
{
    public const string BuiltInName = "ev2";

    public static NamespaceSettings Settings { get; } = new(
        IsSingleton: true,
        BicepExtensionName: BuiltInName,
        ConfigurationType: null,
        TemplateExtensionName: "Ev2",
        TemplateExtensionVersion: "1.0.0");

    public static NamespaceType Create(string aliasName)
    {
        var externalInputFunction = new FunctionOverloadBuilder(LanguageConstants.ExternalInputBicepFunctionName)
            .WithGenericDescription("Resolves input from an external source. The input value is resolved during deployment, not at compile time.")
            .WithRequiredParameter("name", LanguageConstants.String, "The name of the input provided by the external tool.")
            .WithOptionalParameter("config", LanguageConstants.Any, "The configuration for the input. The configuration is specific to the external tool.")
            .WithEvaluator(exp => new FunctionCallExpression(exp.SourceSyntax, LanguageConstants.ExternalInputsArmFunctionName, exp.Parameters))
            .WithReturnType(LanguageConstants.Any)
            .Build();

        var rolloutMetadataType = RolloutSpecType.Properties["rolloutMetadata"].TypeReference.Type;
        var serviceMetadataType = ServiceModelType.Properties["serviceMetadata"].TypeReference.Type;

        return new NamespaceType(
            aliasName,
            Settings,
            [
                new(StageMapType.Name, StageMapType),
                new(ConfigurationSpecType.Name, ConfigurationSpecType),
                new(rolloutMetadataType.Name, rolloutMetadataType),
                new(serviceMetadataType.Name, serviceMetadataType),
            ],
            [
                externalInputFunction,
            ],
            [],
            [],
            new EmptyResourceTypeProvider());
    }

    // sourced from https://ev2schema.azure.net/schemas/2020-01-01/ConfigurationSpecification.json
    public static ObjectType ConfigurationSpecType = new(
        "ConfigSpec",
        TypeSymbolValidationFlags.Default,
        new[]
        {
            new NamedTypeProperty("$schema", LanguageConstants.String, Description: "JSON schema."),
            new NamedTypeProperty("settings", LanguageConstants.Object, Description: "Configuration settings of the cloud."),
            new NamedTypeProperty("geographies", new TypedArrayType(
                new ObjectType(
                    "Geography",
                    TypeSymbolValidationFlags.Default,
                    new[]
                    {
                        new NamedTypeProperty("name", LanguageConstants.String, Description: "Name of the geography."),
                        new NamedTypeProperty("settings", LanguageConstants.Object, Description: "Configuration settings for this geography."),
                        new NamedTypeProperty("regions", new TypedArrayType(
                            new ObjectType(
                                "Region",
                                TypeSymbolValidationFlags.Default,
                                new[]
                                {
                                    new NamedTypeProperty("name", LanguageConstants.String, Description: "Name of the region."),
                                    new NamedTypeProperty("settings", LanguageConstants.Object, Description: "Configuration settings for this region."),
                                }), TypeSymbolValidationFlags.Default), Description: "Region objects in this geography."),
                    }), TypeSymbolValidationFlags.Default), Description: "Geography objects in the cloud."),
        });

    // sourced from https://ev2schema.azure.net/schemas/2020-04-01/RegionAgnosticRolloutSpecification.json
    public static ObjectType RolloutSpecType = new(
        "RolloutSpec",
        TypeSymbolValidationFlags.Default,
        new[]
        {
            new NamedTypeProperty("contentVersion", LanguageConstants.String, Description: "The version of the schema that a document conforms to."),
            new NamedTypeProperty("rolloutMetadata", new ObjectType(
                "RolloutMetadata",
                TypeSymbolValidationFlags.Default,
                new[]
                {
                    new NamedTypeProperty("serviceModelPath", LanguageConstants.String, Description: "The path relative to the Service Group Root that points to the generic service model of the service that is being updated as part of this rollout."),
                    new NamedTypeProperty("scopeBindingsPath", LanguageConstants.String, Description: "The path relative to the Service Group Root that points to the scope bindings file."),
                    new NamedTypeProperty("name", LanguageConstants.String, Description: "The user-specified name of this particular rollout."),
                    new NamedTypeProperty("rolloutType", LanguageConstants.String, Description: "The scope of this particular rollout."),
                    new NamedTypeProperty("buildSource", new ObjectType(
                        "BuildSource",
                        TypeSymbolValidationFlags.Default,
                        new[]
                        {
                            new NamedTypeProperty("parameters", new ObjectType(
                                "BuildSourceParameters",
                                TypeSymbolValidationFlags.Default,
                                new[]
                                {
                                    new NamedTypeProperty("version", LanguageConstants.String, Description: "The version of the build being deployed."), 
                                    // new NamedTypeProperty("versionFile", LanguageConstants.String, Description: "The path relative to the Service Group Root which points to the file whose contents represent the version of the build being deployed."),
                                })),
                        })),
                    new NamedTypeProperty("notification", new ObjectType(
                        "Notification",
                        TypeSymbolValidationFlags.Default,
                        new[]
                        {
                            new NamedTypeProperty("email", new ObjectType(
                                "Email",
                                TypeSymbolValidationFlags.Default,
                                new[]
                                {
                                    new NamedTypeProperty("to", LanguageConstants.StringArray, Description: "To email addresses list."),
                                    new NamedTypeProperty("cc", LanguageConstants.StringArray, Description: "Cc email addresses list."),
                                    // new NamedTypeProperty("to", LanguageConstants.String, Description: "To email addresses list separator with ',;'."),
                                    // new NamedTypeProperty("cc", LanguageConstants.String, Description: "Cc email addresses list separator with ',;'."),
                                }), Description: "Email Notification definitions."),
                            new NamedTypeProperty("incident", new ObjectType(
                                "Incident",
                                TypeSymbolValidationFlags.Default,
                                new[]
                                {
                                    new NamedTypeProperty("providerType", LanguageConstants.String, Description: "The incident provider type."),
                                    new NamedTypeProperty("properties", new ObjectType(
                                        "IncidentProperties",
                                        TypeSymbolValidationFlags.Default,
                                        new[]
                                        {
                                            new NamedTypeProperty("connectorId", LanguageConstants.String, Description: "The connector Id for ICM."),
                                            new NamedTypeProperty("routingId", LanguageConstants.String, Description: "The routing Id for ICM."),
                                            new NamedTypeProperty("environment", LanguageConstants.String, Description: "The environment of the incidents raising location."),
                                            new NamedTypeProperty("correlateBy", LanguageConstants.String, Description: "The incident correlation type."),
                                        })),
                                }), Description: "Incident notification definitions."),
                        })),
                    new NamedTypeProperty("rolloutPolicyReferences", new TypedArrayType(
                        new ObjectType(
                            "PolicyReference",
                            TypeSymbolValidationFlags.Default,
                            new[]
                            {
                                new NamedTypeProperty("name", LanguageConstants.String, Description: "The name of the policy."),
                                new NamedTypeProperty("version", LanguageConstants.String, Description: "The version of the policy to use."),
                            }), TypeSymbolValidationFlags.Default), Description: "List of rollout policy references to use for the rollout."),
                    new NamedTypeProperty("configuration", new ObjectType(
                        "Configuration",
                        TypeSymbolValidationFlags.Default,
                        new[]
                        {
                            new NamedTypeProperty("serviceScope", new ObjectType(
                                "ServiceScope",
                                TypeSymbolValidationFlags.Default,
                                new[]
                                {
                                    new NamedTypeProperty("spec", ConfigurationSpecType, Description: "The service scope configuration specification."),
                                    // new NamedTypeProperty("specPath", LanguageConstants.String, Description: "The path relative to the Service Group Root that points to the service scope configuration specification."),
                                })),
                            new NamedTypeProperty("serviceGroupScope", new ObjectType(
                                "ServiceGroupScope",
                                TypeSymbolValidationFlags.Default,
                                new[]
                                {
                                    new NamedTypeProperty("spec", ConfigurationSpecType, Description: "The service group scope configuration specification."),
                                    // new NamedTypeProperty("specPath", LanguageConstants.String, Description: "The path, relative to the Service Group Root, that points to the service group scope configuration specification."),
                                })),
                            new NamedTypeProperty("ringScope", new TypedArrayType(
                                new ObjectType(
                                    "RingScope",
                                    TypeSymbolValidationFlags.Default,
                                    new[]
                                    {
                                        new NamedTypeProperty("ring", LanguageConstants.String, Description: "The name of the target ring."),
                                        new NamedTypeProperty("serviceLevelSpec", ConfigurationSpecType, Description: "The common ring scope configuration specification across all service groups under the service."),
                                        new NamedTypeProperty("serviceGroupLevelSpec", ConfigurationSpecType, Description: "The ring scope configuration specification for the current service group."),
                                        // new NamedTypeProperty("serviceLevelSpecPath", LanguageConstants.String, Description: "The path, relative to the Service Group Root, that points to the common ring scope configuration specification across all service groups under the service."),
                                        // new NamedTypeProperty("serviceGroupLevelSpecPath", LanguageConstants.String, Description: "The path, relative to the Service Group Root, that points to the ring scope configuration specification for the current service group."),
                                    }),
                                    TypeSymbolValidationFlags.Default), Description: "Configuration spec reference list for ring scope."),
                        })),
                })),
            new NamedTypeProperty("orchestratedSteps", new TypedArrayType(
                new ObjectType(
                    "OrchestratedStep",
                    TypeSymbolValidationFlags.Default,
                    new[]
                    {
                        new NamedTypeProperty("name", LanguageConstants.String, Description: "The name of the rollout step."),
                        new NamedTypeProperty("targetType", LanguageConstants.String, Description: "The type of the intended target of this rollout."),
                        new NamedTypeProperty("targetName", LanguageConstants.String, Description: "The unique identifier of the target that is to be updated."),
                        new NamedTypeProperty("actions", new TypedArrayType(LanguageConstants.String, TypeSymbolValidationFlags.Default), Description: "The actions that must take place as part of this step."),
                        new NamedTypeProperty("dependsOn", new TypedArrayType(LanguageConstants.String, TypeSymbolValidationFlags.Default), Description: "The names of the rollout steps that must be executed prior to the current step being executed."),
                    }), TypeSymbolValidationFlags.Default), Description: "The exact sequence of steps that must be executed as part of this rollout."),
        });

    // sourced from https://ev2schema.azure.net/schemas/2020-04-01/StageMap.json
    public static ObjectType StageMapType = new(
        "StageMap",
        TypeSymbolValidationFlags.Default,
        new[]
        {
            new NamedTypeProperty("contentVersion", LanguageConstants.String, Description: "The version of the schema that a document conforms to."),
            new NamedTypeProperty("name", LanguageConstants.String, Description: "Name of the stage map."),
            new NamedTypeProperty("version", LanguageConstants.String, Description: "Version string for the stage map."),
            new NamedTypeProperty("configuration", new ObjectType(
                "StageMapConfig",
                TypeSymbolValidationFlags.Default,
                new[]
                {
                    new NamedTypeProperty("promotion", new ObjectType(
                        "StagePromotion",
                        TypeSymbolValidationFlags.Default,
                        new[]
                        {
                            new NamedTypeProperty("manual", LanguageConstants.Bool, Description: "Manual intervention is required to move to the next stage if value is true."),
                            new NamedTypeProperty("timeout", LanguageConstants.String, Description: "ISO 8601 format. The promotion will be marked as failed if no manual intervention occurs."),
                            new NamedTypeProperty("validation", new ObjectType(
                                "Validation",
                                TypeSymbolValidationFlags.Default,
                                new[]
                                {
                                    new NamedTypeProperty("waitDuration", LanguageConstants.String, Description: "ISO 8601 format time to wait after the current stage is completed."),
                                    new NamedTypeProperty("maxElasticDuration", LanguageConstants.String, Description: "ISO 8601 format time for elastic duration of health check."),
                                    new NamedTypeProperty("healthyStateDuration", LanguageConstants.String, Description: "ISO 8601 format time for health check."),
                                })),
                        })),
                    new NamedTypeProperty("regionConcurrency", new ObjectType(
                        "RegionConcurrency",
                        TypeSymbolValidationFlags.Default,
                        new[]
                        {
                            new NamedTypeProperty("disablePairedRegions", LanguageConstants.Bool, Description: "Paired regions can run in parallel if value is true."),
                            new NamedTypeProperty("maxParallelCount", LanguageConstants.Int, Description: "How many regions can be run in parallel."),
                            new NamedTypeProperty("inheritPromotion", LanguageConstants.Bool, Description: "Use stage promotion setting for sub-stages of parallel running regions if value is true."),
                            new NamedTypeProperty("promotionOverride", new ObjectType(
                                "StagePromotionOverride",
                                TypeSymbolValidationFlags.Default,
                                new[]
                                {
                                    new NamedTypeProperty("manual", LanguageConstants.Bool, Description: "Manual intervention is required to move to the next stage if value is true."),
                                    new NamedTypeProperty("timeout", LanguageConstants.String, Description: "ISO 8601 format. The promotion will be marked as failed if no manual intervention occurs."),
                                })),
                        })),
                })),
            new NamedTypeProperty("stages", new TypedArrayType(
                new ObjectType(
                    "Stage",
                    TypeSymbolValidationFlags.Default,
                    new[]
                    {
                        new NamedTypeProperty("name", LanguageConstants.String, Description: "Name of the stage."),
                        new NamedTypeProperty("sequence", LanguageConstants.Int, Description: "Sequence order of the current stage."),
                        new NamedTypeProperty("regions", new TypedArrayType(LanguageConstants.String, TypeSymbolValidationFlags.Default), Description: "Regions that can be deployed in this stage."),
                        new NamedTypeProperty("stamps", new TypedArrayType(LanguageConstants.String, TypeSymbolValidationFlags.Default), Description: "Stages apply for all regions in 'regions' property if no individual stages are defined inside."),
                    }), TypeSymbolValidationFlags.Default), Description: "Stage array of the stage map."),
            new NamedTypeProperty("rings", new TypedArrayType(
                new ObjectType(
                    "DeploymentRing",
                    TypeSymbolValidationFlags.Default,
                    new[]
                    {
                        new NamedTypeProperty("name", LanguageConstants.String, Description: "Name of the ring."),
                        new NamedTypeProperty("rolloutInfra", LanguageConstants.String, Description: "Target rollout infra for this ring."),
                        new NamedTypeProperty("tenantId", LanguageConstants.String, Description: "Microsoft Entra tenant id for this ring rollout."),
                        new NamedTypeProperty("stageMap", LanguageConstants.String, Description: "Stage map reference for this ring rollout, e.g., 'myStageMap/1.0.0'."),
                    }), TypeSymbolValidationFlags.Default), Description: "Array of the deployment ring object."),
        });

    // sourced from https://ev2schema.azure.net/schemas/2020-04-01/RegionAgnosticServiceModel.json
    public static ObjectType ServiceModelType = new(
        "ServiceModel",
        TypeSymbolValidationFlags.Default,
        new[]
        {
            new NamedTypeProperty("$schema", LanguageConstants.String, Description: "JSON schema."),
            new NamedTypeProperty("contentVersion", LanguageConstants.String, Description: "The version of the schema that a document conforms to."),
            new NamedTypeProperty("serviceMetadata", new ObjectType(
                "ServiceMetadata",
                TypeSymbolValidationFlags.Default,
                new[]
                {
                    new NamedTypeProperty("serviceIdentifier", LanguageConstants.String, Description: "The service tree identifier for the Azure service."),
                    new NamedTypeProperty("serviceGroup", LanguageConstants.String, Description: "The human-readable name of this Azure service."),
                    new NamedTypeProperty("displayName", LanguageConstants.String, Description: "The name to be used for displaying information about the Azure service."),
                    new NamedTypeProperty("serviceSpecificationPath", LanguageConstants.String, Description: "The path to the service specification file."),
                    new NamedTypeProperty("serviceGroupSpecificationPath", LanguageConstants.String, Description: "The path to the service group specification file."),
                    new NamedTypeProperty("tenantId", LanguageConstants.String, Description: "The identifier of the tenant to which all the subscriptions and resources in the service model belong."),
                    new NamedTypeProperty("environment", LanguageConstants.String, Description: "The environment that this particular service is operating in."),
                    new NamedTypeProperty("buildout", new ObjectType(
                        "Buildout",
                        TypeSymbolValidationFlags.Default,
                        new[]
                        {
                            new NamedTypeProperty("isForAutomatedBuildout", LanguageConstants.String, Description: "The flag to pick up this service for automated buildout."),
                            new NamedTypeProperty("enables", new TypedArrayType(
                                new ObjectType(
                                    "EnabledCapability",
                                    TypeSymbolValidationFlags.Default,
                                    new[]
                                    {
                                        new NamedTypeProperty("type", LanguageConstants.String, Description: "The type of the enabled capability."),
                                        new NamedTypeProperty("properties", new ObjectType(
                                            "CapabilityProperties",
                                            TypeSymbolValidationFlags.Default,
                                            new[]
                                            {
                                                new NamedTypeProperty("namespace", LanguageConstants.String, Description: "The namespace of the ARM resource provider enabled by this service."),
                                                new NamedTypeProperty("resourceTypes", new TypedArrayType(
                                                    new ObjectType(
                                                        "ResourceType",
                                                        TypeSymbolValidationFlags.Default,
                                                        new[]
                                                        {
                                                            new NamedTypeProperty("name", LanguageConstants.String, Description: "The name of the ARM resource type."),
                                                            new NamedTypeProperty("apiVersions", new TypedArrayType(LanguageConstants.String, TypeSymbolValidationFlags.Default), Description: "The list of API versions supported for the ARM resource type."),
                                                        }), TypeSymbolValidationFlags.Default), Description: "The list of ARM resource types enabled by this service."),
                                            }), Description: "The properties for the armResourceProvider type."),
                                    }), TypeSymbolValidationFlags.Default), Description: "The list of resource capabilities enabled by this service."),
                            new NamedTypeProperty("dependencies", new TypedArrayType(
                                new ObjectType(
                                    "Dependency",
                                    TypeSymbolValidationFlags.Default,
                                    new[]
                                    {
                                        new NamedTypeProperty("name", LanguageConstants.String, Description: "The name given to refer to this dependency."),
                                        new NamedTypeProperty("serviceIdentifier", LanguageConstants.String, Description: "The service tree identifier of the dependency."),
                                        new NamedTypeProperty("serviceGroup", LanguageConstants.String, Description: "The service group name of the dependency."),
                                        new NamedTypeProperty("displayName", LanguageConstants.String, Description: "The name to be used to display information about this dependency."),
                                    }), TypeSymbolValidationFlags.Default), Description: "The list of dependent services that should be available before this service can be built in an Azure region."),
                            new NamedTypeProperty("phase", LanguageConstants.String, Description: "The phase of the service buildout. Only accepted values are 'Pre', 'Core', and 'Post'."),
                        }), Description: "The properties that correspond to the build-out of the service."),
                }), Description: "An entity that contains information that can be used to uniquely identify an Azure service."),
            new NamedTypeProperty("serviceResourceGroupDefinitions", new TypedArrayType(
                new ObjectType(
                    "ServiceResourceGroupDefinition",
                    TypeSymbolValidationFlags.Default,
                    new[]
                    {
                        new NamedTypeProperty("name", LanguageConstants.String, Description: "The human-readable name of the definition."),
                        new NamedTypeProperty("subscriptionKey", LanguageConstants.String, Description: "The key to refer to a specific subscription."),
                        new NamedTypeProperty("azureResourceGroupName", LanguageConstants.String, Description: "The Azure Resource Group name."),
                        new NamedTypeProperty("managementGroupId", LanguageConstants.String, Description: "The ID of the management group that management-group-level resources created from this service resource group definition will be deployed to."),
                        new NamedTypeProperty("executionConstraint", new ObjectType(
                            "ExecutionConstraint",
                            TypeSymbolValidationFlags.Default,
                            new[]
                            {
                                new NamedTypeProperty("level", LanguageConstants.String, Description: "The execution constraint scope level."),
                                new NamedTypeProperty("quantifier", LanguageConstants.String, Description: "The quantifier to decide if already deployed resources can be redeployed or not."),
                                new NamedTypeProperty("regions", LanguageConstants.Any, Description: "The specified regions to deploy to."),
                                new NamedTypeProperty("isInCloud", LanguageConstants.Any, Description: "The flag to represent if resources should be deployed to the current cloud."),
                            }), Description: "The constraint defining the scope at which the service resource group and the resources defined should be deployed."),
                        new NamedTypeProperty("stamps", new ObjectType(
                            "Stamps",
                            TypeSymbolValidationFlags.Default,
                            new[]
                            {
                                new NamedTypeProperty("count", LanguageConstants.Any, Description: "The count of the number of stamps to provision for this service resource group definition in a location."),
                            }), Description: "The information about the stamps that should be created for this service resource group definition."),
                    }), TypeSymbolValidationFlags.Default), Description: "The enumeration of the various resource group definitions that represent how to construct the resource groups that constitute this cloud service."),
        });
}