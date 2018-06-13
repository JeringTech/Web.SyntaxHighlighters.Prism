using Microsoft.AspNetCore.NodeServices;
using Microsoft.AspNetCore.NodeServices.HostingModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JeremyTCD.WebUtils.SyntaxHighlighters.Prism
{
    public class PrismService : IPrismService, IDisposable
    {
        internal const string INTEROP_FILE = "JeremyTCD.WebUtils.SyntaxHighlighters.Prism.bundle.js";
        private readonly INodeServices _nodeServices;

        /// <summary>
        /// Use <see cref="Lazy{T}"/> for thread safe lazy initialization since invoking a JS method through NodeServices
        /// can take several hundred milliseconds. Wrap in a <see cref="Task{T}"/> for asynchrony.
        /// More information on AsyncLazy - https://blogs.msdn.microsoft.com/pfxteam/2011/01/15/asynclazyt/.
        /// </summary>
        private readonly Lazy<Task<string[]>> _aliases;

        public PrismService(INodeServices nodeServices)
        {
            _nodeServices = nodeServices;
            _aliases = new Lazy<Task<string[]>>(GetAliasesAsync);
        }

        /// <summary>
        /// Highlights <paramref name="code"/>.
        /// </summary>
        /// <param name="code">Code to highlight.</param>
        /// <param name="languageAlias">A Prism language alias. Visit https://prismjs.com/index.html#languages-list for the list of valid language aliases.</param>
        /// <returns>Highlighted <paramref name="code"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="code"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="languageAlias"/> is not a valid Prism language alias.</exception>
        /// <exception cref="NodeInvocationException">Thrown if a Node error occurs.</exception>
        public virtual async Task<string> HighlightAsync(string code, string languageAlias)
        {
            if (code == null)
            {
                throw new ArgumentNullException(Strings.Exception_ParameterCannotBeNull, nameof(code));
            }

            if (string.IsNullOrWhiteSpace(code))
            {
                // Nothing to highlight
                return code;
            }

            if (!await IsValidLanguageAliasAsync(languageAlias).ConfigureAwait(false))
            {
                // languageAlias is invalid
                throw new ArgumentException(string.Format(Strings.Exception_InvalidPrismLanguageAlias, languageAlias));
            }

            try
            {
                return await _nodeServices.InvokeExportAsync<string>(INTEROP_FILE, "highlight", code, languageAlias).ConfigureAwait(false);
            }
            catch (AggregateException exception)
            {
                if (exception.InnerException is NodeInvocationException)
                {
                    throw exception.InnerException;
                }
                throw;
            }
        }

        /// <summary>
        /// Returns true if <paramref name="languageAlias"/> is a valid Prism language alias. Otherwise, returns false.
        /// </summary>
        /// <param name="languageAlias">Language alias to validate. Visit https://prismjs.com/index.html#languages-list for the list of valid language aliases.</param>
        /// <returns>true if <paramref name="languageAlias"/> is a valid Prism language alias. Otherwise, false.</returns>
        /// <exception cref="NodeInvocationException">Thrown if a Node error occurs.</exception>
        public virtual async Task<bool> IsValidLanguageAliasAsync(string languageAlias)
        {
            if (string.IsNullOrWhiteSpace(languageAlias))
            {
                return false;
            }

            string[] aliases;

            try
            {
                aliases = await _aliases.Value.ConfigureAwait(false);
            }
            catch (AggregateException exception)
            {
                if (exception.InnerException is NodeInvocationException)
                {
                    throw exception.InnerException;
                }
                throw;
            }

            return aliases.Contains(languageAlias);
        }

        /// <summary>
        /// Required for lazy initialization.
        /// </summary>
        /// <returns></returns>
        internal virtual Task<string[]> GetAliasesAsync()
        {
            return _nodeServices.InvokeExportAsync<string[]>(INTEROP_FILE, "getAliases");
        }

        public void Dispose()
        {
            _nodeServices.Dispose();
        }
    }
}
