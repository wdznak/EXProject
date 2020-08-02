using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MVCClient.BackgroundHosts
{
    public static class WSScraperCollectionExtention
    {
        public static void AddWSScraper(this IServiceCollection services)
        {
            services.AddSingleton<StatsService>();
            services.AddSingleton<BackgroundWSQueue>();
            //services.AddHostedService<CryptoDataScraperService>();
            services.AddSingleton<CryptoDataScraperService>();
            services.AddSingleton<IHostedService>(p => p.GetService<CryptoDataScraperService>());
            services.AddTransient<IStorage, FileStorage>();
            services.AddTransient<BinanceWSScraper>();
            services.AddSingleton<ConnectionsManager>();
        }
    }
}
