using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly.Extensions.Http;
using Polly;
using System.Reflection;
using Venta.Domain.Repositories;
using Venta.Domain.Services.WebServices;
using Venta.CrossCutting.Configs;
using Venta.Infrastructure.Repositories.Base;
using Venta.Infrastructure.Services.WebServices;
using Serilog;
using Confluent.Kafka;
using System.Net;
using Venta.Domain.Service.Events;
using Venta.Infrastructure.Services.Events;

namespace Venta.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfraestructure(
            this IServiceCollection services, IConfiguration configInfo
            )
        {
            var appConfiguration = new AppConfiguration(configInfo);

            var httpClientBuilder = services.AddHttpClient<IStocksService, StockService>(
                options =>
                {
                    options.BaseAddress = new Uri(appConfiguration.UrlBaseServicioStock);
                    //options.Timeout = TimeSpan.FromMilliseconds(5000);
                }
                ).SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(GetRetryPolicy());
            //.AddPolicyHandler(GetCircuitBreakerPolicy());
            var httpClientBuilder2 = services.AddHttpClient<IPagoService, PagoService>(
                options =>
                {
                    options.BaseAddress = new Uri(appConfiguration.UrlBaseServicioPago);
                    //options.Timeout = TimeSpan.FromMilliseconds(5000);
                }
                ).SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetCircuitBreakerPolicy());

            services.AddDbContext<VentaDbContext>(
                options => options.UseSqlServer(appConfiguration.ConexionDBVentas)
                );
            services.AddRepositories(Assembly.GetExecutingAssembly());
            services.AddProducer(configInfo);
            services.AddEventServices();
            services.AddLogger(appConfiguration.LogMongoServerDB, appConfiguration.LogMongoDbCollection);
        }
        private static IServiceCollection AddProducer(this IServiceCollection services, IConfiguration configInfo)
        {
            var appConfiguration = new AppConfiguration(configInfo);
            var config = new ProducerConfig
            {
                Acks = Acks.Leader,
                BootstrapServers = appConfiguration.BootstrapServers_Kafka,
                ClientId = Dns.GetHostName(),
            };

            services.AddSingleton<IPublisherFactory>(sp => new PublisherFactory(config));
            return services;
        }
        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(2,
                            retryAttempts => TimeSpan.FromSeconds(Math.Pow(3, retryAttempts)));
        }

        private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            Action<DelegateResult<HttpResponseMessage>, TimeSpan> onBreak = (result, timeSpan) =>
            {
                Console.WriteLine(result);
            }
            ;
            Action onReset = null;
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30),
                onBreak, onReset
                );
        }
        public static void AddRepositories(this IServiceCollection services, Assembly assembly)
        {
            var respositoryTypes = assembly
                .GetExportedTypes().Where(item => item.GetInterface(nameof(IRepository)) != null).ToList();


            foreach (var repositoryType in respositoryTypes)
            {
                var repositoryInterfaceType = repositoryType.GetInterfaces()
                    .Where(item => item.GetInterface(nameof(IRepository)) != null)
                    .First();

                services.AddScoped(repositoryInterfaceType, repositoryType);
            }
        }
        public static void AddLogger(this IServiceCollection services, string connectionStringDbLog, string collectionName)
        {
            var serilogLogger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.MongoDB(connectionStringDbLog, collectionName: collectionName)
                .CreateLogger();


            services.AddLogging(builder =>
            {
                builder.AddSerilog(logger: serilogLogger, dispose: true);
            });
        }
        private static void AddEventServices(this IServiceCollection services)
        {
            services.AddSingleton<IEventSender, EventSender>();
        }
    }
}
