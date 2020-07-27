using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Cli.CommandLine
{
    public class BuildArguments : Arguments
    {
        public BuildArguments(IEnumerable<string> arguments)
        {
            this.OutputToStdOut = arguments.Where(f => f.ToLowerInvariant() == "--stdout").Count() > 0;
            this.Files = arguments.Where(f => f.ToLowerInvariant() != "--stdout").ToImmutableArray();

            if (this.Files.Any() == false)
            {
                throw new CommandLineException($"At least one file must be specified to the {Command.Build} command.");
            }
        }

        public ImmutableArray<string> Files { get; }

        public bool OutputToStdOut {private set; get;}
    }
}
