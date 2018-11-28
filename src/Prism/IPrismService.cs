using Jering.Javascript.NodeJS;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Jering.Web.SyntaxHighlighters.Prism
{
    /// <summary>
    /// An abstraction for performing syntax highlighting using Prism.
    /// </summary>
    public interface IPrismService
    {
        /// <summary>
        /// Highlights code of a specified language.
        /// </summary>
        /// <param name="code">Code to highlight.</param>
        /// <param name="languageAlias">A Prism language alias. Visit https://prismjs.com/index.html#languages-list for the list of valid language aliases.</param>
        /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
        /// <returns>Highlighted code.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="languageAlias"/> is not a valid Prism language alias.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="code"/> is null.</exception>
        /// <exception cref="InvocationException">Thrown if a NodeJS error occurs.</exception>
        /// <exception cref="ObjectDisposedException">Thrown if this instance has been disposed or if an attempt is made to use one of its dependencies that has been disposed.</exception>
        /// <exception cref="OperationCanceledException">Thrown if <paramref name="cancellationToken"/> is cancelled.</exception>
        Task<string> HighlightAsync(string code, string languageAlias, CancellationToken cancellationToken = default);

        /// <summary>
        /// Determines whether a language alias is valid.
        /// </summary>
        /// <param name="languageAlias">Language alias to validate. Visit https://prismjs.com/index.html#languages-list for the list of valid language aliases.</param>
        /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
        /// <returns>true if the specified language alias is a valid Prism language alias. Otherwise, false.</returns>
        /// <exception cref="InvocationException">Thrown if a NodeJS error occurs.</exception>
        /// <exception cref="ObjectDisposedException">Thrown if this instance has been disposed or if an attempt is made to use one of its dependencies that has been disposed.</exception>
        /// <exception cref="OperationCanceledException">Thrown if <paramref name="cancellationToken"/> is cancelled.</exception>
        Task<bool> IsValidLanguageAliasAsync(string languageAlias, CancellationToken cancellationToken = default);
    }
}
