using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace JeremyTCD.WebUtils.SyntaxHighlighters.Prism
{
    public static class PrismServiceCollectionExtensions
    {
        public static IServiceCollection AddPrism(this IServiceCollection services)
        {
            services.TryAddSingleton<IPrism, Prism>();

            // Third party services
            services.AddNodeServices();

            return services;
        }
    }
}
