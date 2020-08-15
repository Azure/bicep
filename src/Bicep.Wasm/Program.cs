using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace Bicep.Wasm
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            var jsRuntime = builder.Services.BuildServiceProvider().GetService<IJSRuntime>();
            await jsRuntime.InvokeAsync<object>("BicepInitialize", DotNetObjectReference.Create(new Interop(jsRuntime)));

            await builder.Build().RunAsync();
        }
    }
}