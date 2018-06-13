using System.Threading.Tasks;

namespace JeremyTCD.WebUtils.SyntaxHighlighters.Prism
{
    public interface IPrismService
    {
        Task<string> HighlightAsync(string code, string languageAlias);

        Task<bool> IsValidLanguageAliasAsync(string languageAlias);
    }
}
