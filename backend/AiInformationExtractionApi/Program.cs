using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using AiInformationExtractionApi.CompositionRoot;
using AiInformationExtractionApi.DatabaseAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;
using Shared.CompositionRoot;

namespace AiInformationExtractionApi;

public static class Program
{
    [Experimental("MEAI001")]
    public static async Task<int> Main(string[] args)
    {
        Log.Logger = Logging.CreateBootstrapLogger();
        try
        {
            await using var app = WebApplication
               .CreateBuilder(args)
               .ConfigureServices()
               .Build()
               .ConfigureMiddleware();

            await app.ApplyDatabaseMigrationsAsync();
            await app.RunAsync();
            return 0;
        }
        catch (HostAbortedException e)
        {
            Log.Debug(e, "The host was aborted, likely by dotnet ef");
            return 0;
        }
        catch (Exception e)
        {
            Log.Fatal(e, "Could not run AI Information Extraction API");
            return 1;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}

