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
#if DEBUG
            services.AddNodeServices(options =>
            {
                options.LaunchWithDebugging = true;
                options.DebuggingPort = 9229;
                options.InvocationTimeoutMilliseconds = 9999999;
            });
#else
            services.AddNodeServices();
#endif


            return services;
        }
    }
}
