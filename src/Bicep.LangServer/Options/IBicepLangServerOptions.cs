using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.LanguageServer.Options;

public interface IBicepLangServerOptions
{
    public bool VsCompatibilityMode { get; set; }
}
