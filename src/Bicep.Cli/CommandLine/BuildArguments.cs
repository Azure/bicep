using System.Collections.Generic;
using System.Linq;

namespace Bicep.Cli.CommandLine
{
    public class BuildArguments : Arguments
    {
        public BuildArguments(IEnumerable<string> files)
        {
            this.Files = files.ToList().AsReadOnly();
        }

        public IList<string> Files { get; }
    }
}
