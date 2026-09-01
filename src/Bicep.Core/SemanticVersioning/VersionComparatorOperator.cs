// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.SemanticVersioning;

/// <summary>
/// The comparison operators supported in a version constraint.
/// </summary>
public enum VersionComparatorOperator
{
    Equal,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
}
