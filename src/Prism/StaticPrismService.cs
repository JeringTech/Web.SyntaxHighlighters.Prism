using Jering.Javascript.NodeJS;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Jering.Web.SyntaxHighlighters.Prism
{
    /// <summary>
    /// A class that provides static access to an instance of the default <see cref="IPrismService"/> implementation's public methods.
    /// </summary>
    public static class StaticPrismService
    {
        private static volatile ServiceProvider _serviceProvider;
        private static volatile IServiceCollection _services;
        private static volatile IPrismService _prismService;
        private static readonly object _createLock = new object();

        private static IPrismService GetOrCreatePrismService()
        {
            if (_prismService == null || _services != null)
            {
                lock (_createLock)
                {
                    if (_prismService == null || _services != null)
                    {
                        // Dispose of service provider
                        _serviceProvider?.Dispose();

                        // Create new service provider
                        (_services ?? (_services = new ServiceCollection())).AddPrism();
                        _serviceProvider = _services.BuildServiceProvider();
                        _prismService = _serviceProvider.GetRequiredService<IPrismService>();

                        // Only set to null after new _prismService is initialized, otherwise another thread might skip the lock and try to use the old _prismService
                        _services = null;
                    }
                }
            }

            // PrismService already exists and no configuration pending
            return _prismService;
        }

        /// <summary>
        /// <para>Disposes the underlying <see cref="IServiceProvider"/> used to resolve <see cref="IPrismService"/>.</para>
        /// <para>This method is not thread safe.</para>
        /// </summary>
        public static void DisposeServiceProvider()
        {
            _serviceProvider?.Dispose();
            _serviceProvider = null;
            _prismService = null;
        }

        /// <summary>
        /// <para>Configures options.</para>
        /// <para>This method is not thread safe.</para>
        /// </summary>
        /// <typeparam name="T">The type of options to configure.</typeparam>
        /// <param name="configureOptions">The action that configures the options.</param>
        public static void Configure<T>(Action<T> configureOptions) where T : class
        {
            (_services ?? (_services = new ServiceCollection())).Configure(configureOptions);
        }

        /// <summary>
        /// Highlights code of a specified language.
        /// </summary>
        /// <param name="code">Code to highlight.</param>
        /// <param name="languageAlias">A Prism language alias. Visit https://prismjs.com/index.html#languages-list for the list of valid language aliases.</param>
        /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
        /// <returns>Highlighted code.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="code"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="languageAlias"/> is not a valid Prism language alias.</exception>
        /// <exception cref="InvocationException">Thrown if a NodeJS error occurs.</exception>
        public static Task<string> HighlightAsync(string code, string languageAlias, CancellationToken cancellationToken = default)
        {
            return GetOrCreatePrismService().HighlightAsync(code, languageAlias, cancellationToken);
        }

        /// <summary>
        /// Determines whether a language alias is valid.
        /// </summary>
        /// <param name="languageAlias">Language alias to validate. Visit https://prismjs.com/index.html#languages-list for the list of valid language aliases.</param>
        /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
        /// <returns>true if the specified language alias is a valid Prism language alias. Otherwise, false.</returns>
        /// <exception cref="InvocationException">Thrown if a NodeJS error occurs.</exception>
        public static Task<bool> IsValidLanguageAliasAsync(string languageAlias, CancellationToken cancellationToken = default)
        {
            return GetOrCreatePrismService().IsValidLanguageAliasAsync(languageAlias, cancellationToken);
        }
    }
}
