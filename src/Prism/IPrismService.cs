using System.Threading.Tasks;

namespace JeremyTCD.WebUtils.SyntaxHighlighters.Prism
{
    public interface IPrismService
    {
        Task<string> Highlight(string code, string languageAlias);

        Task<bool> IsValidLanguageAlias(string languageAlias);
    }
}
