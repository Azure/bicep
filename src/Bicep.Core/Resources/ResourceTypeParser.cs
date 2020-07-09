using System.Linq;
using System.Text.RegularExpressions;

namespace Bicep.Core.Resources
{
    public static class ResourceTypeParser
    {
        private static readonly Regex ResourceTypePattern = new Regex(@"^(?<namespace>[a-z0-9][a-z0-9\.]*)(/(?<type>[a-z0-9]+))+@(?<version>(\d{4}-\d{2}-\d{2})(-(preview|alpha|beta|rc|privatepreview))?$)", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public static ResourceTypeReference? TryParse(string? resourceType)
        {
            if (resourceType == null)
            {
                return null;
            }

            var match = ResourceTypePattern.Match(resourceType);
            if (match.Success == false)
            {
                return null;
            }

            var ns = match.Groups["namespace"].Value;
            var types = match.Groups["type"].Captures.Cast<Capture>().Select(c => c.Value);
            var version = match.Groups["version"].Value;

            return new ResourceTypeReference(ns, types, version);
        }
    }
}
