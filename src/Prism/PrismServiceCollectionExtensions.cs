using Jering.JavascriptUtils.NodeJS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace JeremyTCD.WebUtils.SyntaxHighlighters.Prism
{
    /// <summary>
    /// Extension methods for setting up Prism in an <see cref="IServiceCollection" />.
    /// </summary>
    public static class PrismServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Prism services to an <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The target <see cref="IServiceCollection"/>.</param>
        public static IServiceCollection AddPrism(this IServiceCollection services)
        {
            services.TryAddSingleton<IPrismService, PrismService>();

            // Third party services
            services.AddNodeJS();

            return services;
        }
    }
}
