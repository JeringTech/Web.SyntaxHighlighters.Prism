using Microsoft.Extensions.DependencyInjection;

namespace JeremyTCD.WebUtils.SyntaxHighlighter
{
    public static class SyntaxHighlighterExtensions
    {
        // TODO add options (revise how options work in di)
        public static IServiceCollection AddSyntaxHighlighter(this IServiceCollection services)
        {
            // TODO make sure this doesn't override if already registered
            services.AddNodeServices();

            services.AddSingleton<IHighlighter, Highlighter>();

            return services;
        }
    }
}
