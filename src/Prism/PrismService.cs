using Jering.JavascriptUtils.NodeJS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace JeremyTCD.WebUtils.SyntaxHighlighters.Prism
{
    /// <summary>
    /// The default implementation of <see cref="IPrismService"/>. This implementation uses <see cref="INodeJSService"/> to send Prism syntax highlighting 
    /// requests to a NodeJS instance.
    /// </summary>
    public class PrismService : IPrismService, IDisposable
    {
        /// <summary>
        /// The identifier used to cache the Prism bundle in NodeJS. This identifier must be unique, so the namespace is used.
        /// </summary>
        internal static string MODULE_CACHE_IDENTIFIER = typeof(PrismService).Namespace;

        internal const string BUNDLE_NAME = "bundle.js";
        private readonly INodeJSService _nodeJSService;
        private readonly IEmbeddedResourcesService _embeddedResourcesService;

        /// <summary>
        /// Use <see cref="Lazy{T}"/> for thread safe lazy initialization since invoking a JS method through NodeJSService
        /// can take several hundred milliseconds. Wrap in a <see cref="Task{T}"/> for asynchrony.
        /// More information on AsyncLazy - https://blogs.msdn.microsoft.com/pfxteam/2011/01/15/asynclazyt/.
        /// </summary>
        private readonly Lazy<Task<HashSet<string>>> _aliases;

        /// <summary>
        /// Creats a <see cref="PrismService"/> instance.
        /// </summary>
        /// <param name="nodeJSService"></param>
        /// <param name="embeddedResourcesService"></param>
        public PrismService(INodeJSService nodeJSService, IEmbeddedResourcesService embeddedResourcesService)
        {
            _nodeJSService = nodeJSService;
            _embeddedResourcesService = embeddedResourcesService;
            _aliases = new Lazy<Task<HashSet<string>>>(GetAliasesAsync);
        }

        /// <inheritdoc />
        public virtual async Task<string> HighlightAsync(string code, string languageAlias)
        {
            if (code == null)
            {
                throw new ArgumentNullException(nameof(code), Strings.Exception_ParameterCannotBeNull);
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

            var args = new object[] { code, languageAlias };
            // Invoke from cache
            (bool success, string result) = await _nodeJSService.TryInvokeFromCacheAsync<string>(MODULE_CACHE_IDENTIFIER, "highlight", args).ConfigureAwait(false);
            if (success)
            {
                return result;
            }

            // Invoke from stream since module is not cached
            using (Stream moduleStream = _embeddedResourcesService.ReadAsStream(typeof(PrismService).Assembly, BUNDLE_NAME))
            {
                // Invoking from stream is 2+x faster than reading the resource as a string and invoking as string. This is because invoking as string causes almost 
                // 1000x more memory to be allocated, resulting in gen 1+ gcs.
                return await _nodeJSService.InvokeFromStreamAsync<string>(moduleStream, MODULE_CACHE_IDENTIFIER, "highlight", args).ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        public virtual async ValueTask<bool> IsValidLanguageAliasAsync(string languageAlias)
        {
            if (string.IsNullOrWhiteSpace(languageAlias))
            {
                return false;
            }

            HashSet<string> aliases = await _aliases.Value.ConfigureAwait(false);

            return aliases.Contains(languageAlias);
        }

        internal async Task<HashSet<string>> GetAliasesAsync()
        {
            string[] aliases;
            // GetAliasesAsync should only ever be called once, before any highlighting is done by NodeJS. So take this oppurtunity to 
            // cache the module.
            using (Stream moduleStream = _embeddedResourcesService.ReadAsStream(typeof(PrismService).Assembly, BUNDLE_NAME))
            {
                aliases = await _nodeJSService.InvokeFromStreamAsync<string[]>(moduleStream, MODULE_CACHE_IDENTIFIER, "getAliases").ConfigureAwait(false);
            }
            return new HashSet<string>(aliases);
        }

        /// <summary>
        /// Disposes of the instance.
        /// </summary>
        public void Dispose()
        {
            _nodeJSService.Dispose();
        }
    }
}
