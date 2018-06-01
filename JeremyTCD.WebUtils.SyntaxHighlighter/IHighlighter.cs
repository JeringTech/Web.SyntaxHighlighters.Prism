using System.Threading;
using System.Threading.Tasks;

namespace JeremyTCD.WebUtils.SyntaxHighlighter
{
    public interface IHighlighter
    {
        Task<string> Highlight(string code, string language, CancellationToken cancellationToken = default(CancellationToken));
    }
}
