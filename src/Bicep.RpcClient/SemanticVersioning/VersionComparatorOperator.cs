// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.RpcClient.SemanticVersioning;

/// <summary>
/// The comparison operators supported in a Bicep version constraint.
/// </summary>
internal enum VersionComparatorOperator
{
    Equal,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
}
