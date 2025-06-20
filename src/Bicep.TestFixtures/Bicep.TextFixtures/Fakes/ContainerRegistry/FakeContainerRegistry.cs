// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.TextFixtures.Fakes.ContainerRegistry
{
    public class FakeContainerRegistry
    {
        private readonly ConcurrentDictionary<string, FakeContainerRepository> repositoriesByName = new();

        public FakeContainerRegistry(string loginServer)
        {
            this.LoginServer = loginServer;
        }

        public string LoginServer { get; }

        public IEnumerable<string> EnumerateRepositoryNames() => this.repositoriesByName.Keys;

        public FakeContainerRepository GetRepository(string repositoryName)
        {
            return repositoriesByName.GetOrAdd(repositoryName, name => new FakeContainerRepository(this, name));
        }
    }
}
