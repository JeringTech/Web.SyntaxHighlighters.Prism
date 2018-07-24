using BenchmarkDotNet.Running;

namespace Jering.WebUtils.SyntaxHighlighters.Prism.Performance
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<Benchmarks>();
        }
    }
}
