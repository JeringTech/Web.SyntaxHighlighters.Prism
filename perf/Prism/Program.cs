using BenchmarkDotNet.Running;

namespace Jering.Web.SyntaxHighlighters.Prism.Performance
{
    public class Program
    {
        public static void Main()
        {
            BenchmarkRunner.Run<Benchmarks>();
        }
    }
}
