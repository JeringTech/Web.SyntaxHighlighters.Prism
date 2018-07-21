using System.Threading.Tasks;

namespace JeremyTCD.WebUtils.SyntaxHighlighters.Prism
{
    public interface IPrismService
    {
        /// <summary>
        /// Highlights <paramref name="code"/>.
        /// </summary>
        /// <param name="code">Code to highlight.</param>
        /// <param name="languageAlias">A Prism language alias. Visit https://prismjs.com/index.html#languages-list for the list of valid language aliases.</param>
        /// <returns>Highlighted <paramref name="code"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="code"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="languageAlias"/> is not a valid Prism language alias.</exception>
        /// <exception cref="InvocationException">Thrown if a Node error occurs.</exception>
        Task<string> HighlightAsync(string code, string languageAlias);

        /// <summary>
        /// Returns true if <paramref name="languageAlias"/> is a valid Prism language alias. Otherwise, returns false.
        /// </summary>
        /// <param name="languageAlias">Language alias to validate. Visit https://prismjs.com/index.html#languages-list for the list of valid language aliases.</param>
        /// <returns>true if <paramref name="languageAlias"/> is a valid Prism language alias. Otherwise, false.</returns>
        /// <exception cref="InvocationException">Thrown if a Node error occurs.</exception>
        Task<bool> IsValidLanguageAliasAsync(string languageAlias);
    }
}
