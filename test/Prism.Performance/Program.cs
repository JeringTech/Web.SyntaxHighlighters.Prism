using BenchmarkDotNet.Running;

namespace JeremyTCD.WebUtils.SyntaxHighlighters.Prism.Performance
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<Benchmarks>();
        }
    }
}
