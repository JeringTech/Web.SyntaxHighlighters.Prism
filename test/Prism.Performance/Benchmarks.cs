using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Jering.WebUtils.SyntaxHighlighters.Prism.Performance
{
    [MemoryDiagnoser]
    public class Benchmarks
    {
        private ServiceProvider _serviceProvider;
        private int _counter;
        private IPrismService _prismService;

        [GlobalSetup(Target = nameof(PrismService_Highlight))]
        public void NodeJSService_InvokeFromFile_Setup()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddPrism();
            _serviceProvider = services.BuildServiceProvider();
            _prismService = _serviceProvider.GetRequiredService<IPrismService>();
            _counter = 0;
        }

        [Benchmark]
        public async Task<string> PrismService_Highlight()
        {
            string result = await _prismService.HighlightAsync($@"function exampleFunction(arg) {{
    // Example comment
    return arg + '{_counter++}';
}}",
                "javascript");
            return result;
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _serviceProvider.Dispose();
        }
    }
}
