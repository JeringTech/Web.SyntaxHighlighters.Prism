using Microsoft.AspNetCore.NodeServices;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JeremyTCD.WebUtils.SyntaxHighlighter
{
    public class Highlighter : IHighlighter, IDisposable
    {
        private readonly INodeServices _nodeServices;

        public Highlighter(INodeServices nodeServices)
        {
            _nodeServices = nodeServices;
        }

        /// <summary>
        /// Highlights <paramref name="code"/>.
        /// </summary>
        /// <param name="code">Code to highlight.</param>
        /// <param name="language">Language of code.</param>
        /// <param name="cancellationToken">Defaults to <see cref="CancellationToken.None"/>.</param>
        /// <returns>
        /// Highlighted code.
        /// </returns>
        /// <exception cref="AggregateException">Thrown if a node error occurs. Exception will contain a <see cref="NodeInvocationException"/> with error details.</exception>
        public virtual Task<string> Highlight(string code, string language, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _nodeServices.InvokeAsync<string>(cancellationToken, "./highlighterInterop.js", code, language);
        }

        public void Dispose()
        {
            _nodeServices.Dispose();
        }
    }
}
