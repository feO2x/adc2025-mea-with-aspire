using Aspire.Hosting;
using Projects;
using Shared;
using Shared.CompositionRoot;

namespace AspireAppHost;

public static class CompositionRoot
{
    public static IDistributedApplicationBuilder Configure(this IDistributedApplicationBuilder builder)
    {
        builder.Services.AddDefaultLogging(builder.Configuration);
        return builder;
    }

    public static IDistributedApplicationBuilder SetUpProjectStructure(this IDistributedApplicationBuilder builder)
    {
        var gateway = builder.AddProject<Gateway>(Constants.GatewayName);
        var identityServer = builder.AddProject<IdentityServer>(Constants.IdentityServerName);
        var postgresServer = builder.AddPostgres(Constants.PostgresServerName);
        var damageReportsDatabase = postgresServer.AddDatabase(Constants.DamageReportsDatabaseName);
        var aiInformationExtractionDatabase = postgresServer.AddDatabase(Constants.AiInformationExtractionDatabaseName);

        var damageReportsApi = builder
           .AddProject<DamageReportsApi>(Constants.DamageReportsServiceName)
           .WaitFor(damageReportsDatabase)
           .WaitFor(identityServer)
           .WithReference(damageReportsDatabase)
           .WithReference(identityServer);

        var aiInformationExtractionApi = builder
           .AddProject<AiInformationExtractionApi>(Constants.AiInformationExtractionServiceName)
           .WaitFor(aiInformationExtractionDatabase)
           .WaitFor(identityServer)
           .WithReference(aiInformationExtractionDatabase)
           .WithReference(identityServer);

        identityServer.WithReference(gateway);
        gateway
           .WaitFor(identityServer)
           .WithReference(identityServer)
           .WithReference(damageReportsApi)
           .WithReference(aiInformationExtractionApi);

        return builder;
    }
}
