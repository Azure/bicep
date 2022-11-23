// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Microsoft.VisualStudio.TestTools.UnitTesting;

[assembly: Parallelize(Scope = ExecutionScope.MethodLevel, Workers = 0)]

// From https://developercommunity.visualstudio.com/t/mstest-passing-readonly-struct-as-dynamicdata-fail/1620466:
// Since 2.2.6, test case discovery for date source tests are moved to actual discovery phase from execution phase, this means that each test case data needs to be serializable.
// TextSpan is a struct which is not serializable. To opt-out of this behavior, the attribute below is needed.
[assembly:TestDataSourceDiscovery(TestDataSourceDiscoveryOption.DuringExecution)]
