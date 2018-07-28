using Jering.JavascriptUtils.NodeJS;
using System;
using System.Threading.Tasks;

namespace Jering.WebUtils.SyntaxHighlighters.Prism
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
        /// <returns>Highlighted code.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="code"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="languageAlias"/> is not a valid Prism language alias.</exception>
        /// <exception cref="InvocationException">Thrown if a NodeJS error occurs.</exception>
        Task<string> HighlightAsync(string code, string languageAlias);

        /// <summary>
        /// Determines whether a language alias is valid.
        /// </summary>
        /// <param name="languageAlias">Language alias to validate. Visit https://prismjs.com/index.html#languages-list for the list of valid language aliases.</param>
        /// <returns>true if the specified language alias is a valid Prism language alias. Otherwise, false.</returns>
        /// <exception cref="InvocationException">Thrown if a NodeJS error occurs.</exception>
        ValueTask<bool> IsValidLanguageAliasAsync(string languageAlias);
    }
}
