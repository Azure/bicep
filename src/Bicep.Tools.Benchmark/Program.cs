// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using BenchmarkDotNet.Running;

namespace Bicep.Tools.Benchmark;

public class Program
{
    public static void Main(string[] args)
        => BenchmarkSwitcher
            .FromAssembly(typeof(Program).Assembly)
            .Run(args);
}
