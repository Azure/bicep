using System.Collections.Generic;
using System.Collections.Immutable;

namespace Bicep.Cli.CommandLine
{
    public class BuildArguments : Arguments
    {
        public BuildArguments(IEnumerable<string> files)
        {
            this.Files = files.ToImmutableArray();
        }

        public ImmutableArray<string> Files { get; }
    }
}
