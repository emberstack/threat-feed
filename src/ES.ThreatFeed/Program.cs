using Autofac;
using Autofac.Extensions.DependencyInjection;
using ES.DnsBlockFeed;
using ES.ThreatFeed.Provider.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;


Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("logging.json")
        .AddEnvironmentVariables("ES_")
        .AddCommandLine(args)
        .Build())
    .CreateLogger();


try
{
    Log.Information("Starting host");
    var builder = Host.CreateDefaultBuilder(args);

    builder.UseServiceProviderFactory(new AutofacServiceProviderFactory());
    builder.UseSerilog();
    builder.UseConsoleLifetime();
    builder.ConfigureAppConfiguration((context, _) =>
    {
        context.HostingEnvironment.EnvironmentName =
            Environment.GetEnvironmentVariable($"{nameof(ES)}_{nameof(Environment)}") ??
            Environments.Production;
    }
    );
    builder.ConfigureHostConfiguration(configBuilder =>
    {
        configBuilder.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("logging.json")
            .AddJsonFile("threat-feeds.json")
            .AddEnvironmentVariables("ES_")
            .AddCommandLine(args);
    });
    builder.ConfigureServices((hostContext, services) =>
    {
        services.AddHttpClient();
        services.AddOptions();
        services.Configure<ThreatFeedsConfiguration>(hostContext.Configuration.GetSection("ThreatFeeds"));
    }).ConfigureContainer((ContainerBuilder container) =>
    {
        container.Register(c => c.Resolve<IHttpClientFactory>().CreateClient()).AsSelf();
        container.RegisterType<DomainThreatFeedService>().AsImplementedInterfaces().SingleInstance();
    });

    await builder.Build().RunAsync();
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}
