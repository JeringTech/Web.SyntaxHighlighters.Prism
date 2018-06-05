using Microsoft.AspNetCore.NodeServices;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JeremyTCD.WebUtils.SyntaxHighlighters.Prism
{
    public class Prism : IPrism, IDisposable
    {
        private const string INTEROP_FILE = "JeremyTCD.WebUtils.SyntaxHighlighters.Prism.Javascript/interop.js";
        private readonly INodeServices _nodeServices;

        /// <summary>
        /// Use <see cref="Lazy{T}"/> for thread safe lazy initialization since invoking a JS method through NodeServices
        /// can take several hundred milliseconds. Wrap in a <see cref="Task{T}"/> for asynchrony.
        /// More information on AsyncLazy - https://blogs.msdn.microsoft.com/pfxteam/2011/01/15/asynclazyt/.
        /// </summary>
        private readonly Lazy<Task<string[]>> _aliases;

        public Prism(INodeServices nodeServices)
        {
            _nodeServices = nodeServices;
            _aliases = new Lazy<Task<string[]>>(GetAliases);
        }

        /// <summary>
        /// Highlights <paramref name="code"/>.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="languageAlias">A Prism language alias. Visit https://prismjs.com/index.html#languages-list for a list of language aliases.</param>
        /// <returns>Highlighted <paramref name="code"/>.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="languageAlias"/> is not a valid Prism language alias.</exception>
        /// <exception cref="AggregateException">Thrown if a Node error occurs. Will contain a <see cref="NodeInvocationException"/> as its inner exception.</exception>
        public virtual async Task<string> Highlight(string code, string languageAlias)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                // Nothing to highlight
                return code;
            }

            if (!await IsValidLanguageAlias(languageAlias).ConfigureAwait(false))
            {
                // languageAlias is invalid
                throw new ArgumentException($"\"{languageAlias}\" is not a valid Prism language alias. Visit https://prismjs.com/index.html#languages-list for a list of language aliases.");
            }

            return await _nodeServices.InvokeExportAsync<string>(INTEROP_FILE, "highlight", code, languageAlias).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns true if <paramref name="languageAlias"/> is a valid Prism language alias. Otherwise, returns false.
        /// Visit https://prismjs.com/index.html#languages-list for a list of language aliases.
        /// </summary>
        /// <param name="languageAlias"></param>
        public virtual async Task<bool> IsValidLanguageAlias(string languageAlias)
        {
            string[] aliases = await _aliases.Value.ConfigureAwait(false);

            return aliases.Contains(languageAlias);
        }

        internal virtual Task<string[]> GetAliases()
        {
            return _nodeServices.InvokeExportAsync<string[]>(INTEROP_FILE, "getAliases");
        }

        public void Dispose()
        {
            _nodeServices.Dispose();
        }
    }
}
