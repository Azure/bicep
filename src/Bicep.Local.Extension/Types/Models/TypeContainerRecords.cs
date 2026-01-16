// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Bicep.Local.Extension.Types.Models;

public record ConfigurationTypeContainer(Type Type);
public record FallbackTypeContainer(Type Type);

public record TypesAssemblyContainer(Assembly[]? Assemblies);
