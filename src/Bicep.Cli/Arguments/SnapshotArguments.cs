// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Cli.Helpers;

namespace Bicep.Cli.Arguments;

public class SnapshotArguments : ArgumentsBase, IInputArguments
{
    private const string TenantIdArgument = "--tenant-id";
    private const string SubscriptionIdArgument = "--subscription-id";
    private const string LocationArgument = "--location";
    private const string ResourceGroupArgument = "--resource-group";
    private const string DeploymentNameArgument = "--deployment-name";

    public enum SnapshotMode
    {
        Overwrite,
        Validate,
    }

    public SnapshotArguments(string[] args) : base(Constants.Command.Snapshot)
    {
        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i].ToLowerInvariant())
            {
                case ArgumentConstants.Mode:
                    ArgumentHelper.ValidateNotAlreadySet(ArgumentConstants.Mode, Mode);
                    Mode = ArgumentHelper.GetEnumValueWithValidation<SnapshotMode>(ArgumentConstants.Mode, args, i);
                    i++;
                    break;

                case TenantIdArgument:
                    ArgumentHelper.ValidateNotAlreadySet(TenantIdArgument, TenantId);
                    TenantId = ArgumentHelper.GetValueWithValidation(TenantIdArgument, args, i);
                    i++;
                    break;

                case SubscriptionIdArgument:
                    ArgumentHelper.ValidateNotAlreadySet(SubscriptionIdArgument, SubscriptionId);
                    SubscriptionId = ArgumentHelper.GetValueWithValidation(SubscriptionIdArgument, args, i);
                    i++;
                    break;

                case ResourceGroupArgument:
                    ArgumentHelper.ValidateNotAlreadySet(ResourceGroupArgument, ResourceGroup);
                    ResourceGroup = ArgumentHelper.GetValueWithValidation(ResourceGroupArgument, args, i);
                    i++;
                    break;

                case LocationArgument:
                    ArgumentHelper.ValidateNotAlreadySet(LocationArgument, Location);
                    Location = ArgumentHelper.GetValueWithValidation(LocationArgument, args, i);
                    i++;
                    break;

                case DeploymentNameArgument:
                    ArgumentHelper.ValidateNotAlreadySet(DeploymentNameArgument, DeploymentName);
                    DeploymentName = ArgumentHelper.GetValueWithValidation(DeploymentNameArgument, args, i);
                    i++;
                    break;

                default:
                    if (args[i].StartsWith("--"))
                    {
                        throw new CommandLineException($"Unrecognized parameter \"{args[i]}\"");
                    }
                    if (InputFile is not null)
                    {
                        throw new CommandLineException($"The input file path cannot be specified multiple times");
                    }
                    InputFile = args[i];
                    break;
            }
        }

        if (InputFile is null)
        {
            throw new CommandLineException($"The input file path was not specified");
        }
    }

    public string InputFile { get; }

    public SnapshotMode? Mode { get; }

    public string? TenantId { get; }

    public string? SubscriptionId { get; }

    public string? Location { get; }

    public string? ResourceGroup { get; }

    public string? DeploymentName { get; }
}
